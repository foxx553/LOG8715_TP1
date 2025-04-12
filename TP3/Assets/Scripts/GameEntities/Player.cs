using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    #region Attributes
    // Size, velocity, position
    [SerializeField]
    private float m_Velocity;
    public float Velocity => m_Velocity;
    [SerializeField]
    private float m_Size = 1;
    private NetworkVariable<Vector2> m_Position = new NetworkVariable<Vector2>();
    public Vector2 Position => m_Position.Value;

    // Local ghost predictions
    public Vector2 m_PredictedPosition;
    private Dictionary<int, Vector2> m_PredictedPositionHistory = new Dictionary<int, Vector2>();
    private Dictionary<int, Vector2> m_InputHistory = new Dictionary<int, Vector2>();

    // Queue of the client requests
    private Queue<KeyValuePair<int, Vector2>> m_RequestQueue = new Queue<KeyValuePair<int, Vector2>>();

    // Game data
    private GameState m_GameState;
    private GameState GameState
    {
        get
        {
            if (m_GameState == null)
            {
                m_GameState = FindObjectOfType<GameState>();
            }
            return m_GameState;
        }
    }

    // History counter
    private int m_TickCounter = 0;
    #endregion

    private void FixedUpdate()
    {
        // No updates if no game data or during a stun
        /*if (GameState == null || GameState.IsStunned)
        {
            return;
        }*/

        
        ulong myClientId = NetworkManager.Singleton.LocalClientId;

        if (GameState == null || GameState.IsStunned)
        {
            if (GameState.StunClientId != myClientId)
            {
                return;
            }
        }
        else if (IsClient && IsOwner)
        {
            if (GameState.m_PredictedIsStunned)
            {
                return;
            }
        }
        

        // Server updating player's position and broadcasting it
        if (IsServer)
        {
            UpdatePositionServer();
        }

        // Client sending its own inputs
        if (IsClient && IsOwner)
        {
            m_TickCounter++;
            UpdateInputClient();
        }
    }

    #region Server/Client updates
    private void UpdatePositionServer()
    {
        // Consuming the request, updating server position, and sending position to client
        if (m_RequestQueue.Count > 0)
        {
            var inputPair = m_RequestQueue.Dequeue();
            var input = inputPair.Value;
            int tickCounter = inputPair.Key;
            m_Position.Value += input * m_Velocity * Time.deltaTime;
            var size = GameState.GameSize;
            if (m_Position.Value.x - m_Size < -size.x)
            {
                m_Position.Value = new Vector2(-size.x + m_Size, m_Position.Value.y);
            }
            else if (m_Position.Value.x + m_Size > size.x)
            {
                m_Position.Value = new Vector2(size.x - m_Size, m_Position.Value.y);
            }

            if (m_Position.Value.y + m_Size > size.y)
            {
                m_Position.Value = new Vector2(m_Position.Value.x, size.y - m_Size);
            }
            else if (m_Position.Value.y - m_Size < -size.y)
            {
                m_Position.Value = new Vector2(m_Position.Value.x, -size.y + m_Size);
            }

            // Sending the position to the client
            SendPositionClientRpc(m_Position.Value, tickCounter);
        }
    }

    private void UpdateInputClient()
    {

        // Getting the resulting direction
        Vector2 inputDirection = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputDirection += Vector2.up;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection += Vector2.left;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection += Vector2.right;
        }
        
        if (inputDirection != Vector2.zero)
        {
            // Prediction for the local ghost
            PredictMovement(inputDirection);

            // Sending the inputs to the server
            SendInputServerRpc(inputDirection.normalized, m_TickCounter);
        }
    }
    #endregion

    #region Rpc
    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, int tickCounter)
    {
        m_RequestQueue.Enqueue(new KeyValuePair<int, Vector2>(tickCounter, input));
    }

    [ClientRpc]
    private void SendPositionClientRpc(Vector2 position, int serverTick)
    {
        
        Reconcile(position, serverTick);
    }
    #endregion

    #region Prediction/Reconciliation
    // Local prediction for the owned ghost
    public void PredictMovement(Vector2 direction)
    {
        if (!IsOwner || NetworkManager.Singleton == null) return;

        // Updating predicted position
        float deltaTime = Time.fixedDeltaTime;
        m_PredictedPosition += direction * Velocity * deltaTime;
        var size = GameState.GameSize;
        if (m_PredictedPosition.x - m_Size < -size.x)
        {
            m_PredictedPosition = new Vector2(-size.x + m_Size, m_PredictedPosition.y);
        }
        else if (m_PredictedPosition.x + m_Size > size.x)
        {
            m_PredictedPosition = new Vector2(size.x - m_Size, m_PredictedPosition.y);
        }
        if (m_PredictedPosition.y + m_Size > size.y)
        {
            m_PredictedPosition = new Vector2(m_PredictedPosition.x, size.y - m_Size);
        }
        else if (m_PredictedPosition.y - m_Size < -size.y)
        {
            m_PredictedPosition = new Vector2(m_PredictedPosition.x, -size.y + m_Size);
        }

        // Updating history
        m_InputHistory[m_TickCounter] = direction;
        m_PredictedPositionHistory[m_TickCounter] = m_PredictedPosition;
    }

    // Local reconciliation for the owned ghost
    public void Reconcile(Vector2 serverPosition, int tickCounter)
    {
        if (!IsOwner) return;

        // If there was a prediction, checking it and correcting the client's history if necessary
        if (m_PredictedPositionHistory.TryGetValue(tickCounter, out var predictedPos))
        {
            Vector2 error = serverPosition - predictedPos;
            if (error.sqrMagnitude > 0.001f)
            {
                float deltaTime = Time.fixedDeltaTime;

                // Checking which ticks to simulate, and removing older history
                List<int> keysToSimulate = new List<int>();
                List<int> keysToRemove = new List<int>();
                foreach (var key in m_PredictedPositionHistory.Keys)
                {
                    if (key >= tickCounter) {
                        keysToSimulate.Add(key);
                    }
                    else
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    m_PredictedPositionHistory.Remove(key);
                    m_InputHistory.Remove(key);
                }
                keysToSimulate.Sort();

                // Recalculating the whole history
                m_PredictedPositionHistory[tickCounter] = serverPosition;
                for (int i = 1; i < keysToSimulate.Count; i++)
                {
                    m_PredictedPositionHistory[keysToSimulate[i]] = m_PredictedPositionHistory[keysToSimulate[i - 1]] + m_InputHistory[keysToSimulate[i]] * Velocity * deltaTime * (keysToSimulate[i] - keysToSimulate[i - 1]);
                    var size = GameState.GameSize;
                    if (m_PredictedPositionHistory[keysToSimulate[i]].x - m_Size < -size.x)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(-size.x + m_Size, m_PredictedPositionHistory[keysToSimulate[i]].y);
                    }
                    else if (m_PredictedPositionHistory[keysToSimulate[i]].x + m_Size > size.x)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(size.x - m_Size, m_PredictedPositionHistory[keysToSimulate[i]].y);
                    }
                    if (m_PredictedPositionHistory[keysToSimulate[i]].y + m_Size > size.y)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(m_PredictedPositionHistory[keysToSimulate[i]].x, size.y - m_Size);
                    }
                    else if (m_PredictedPositionHistory[keysToSimulate[i]].y - m_Size < -size.y)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(m_PredictedPositionHistory[keysToSimulate[i]].x, -size.y + m_Size);
                    }
                }
                m_PredictedPosition = m_PredictedPositionHistory[keysToSimulate[keysToSimulate.Count - 1]];
            }
        }
    }
    #endregion

}
