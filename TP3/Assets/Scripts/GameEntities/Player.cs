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
    private Dictionary<int, Vector2> m_PredictionHistory = new Dictionary<int, Vector2>();

    // Managing several inputs pressed together (example: up AND down) 
    private Queue<KeyValuePair<int, Vector2>> m_InputQueue = new Queue<KeyValuePair<int, Vector2>>();

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
        if (GameState == null || GameState.IsStunned)
        {
            return;
        }

        // Server updating player's position and broadcasting it
        if (IsServer)
        {
            UpdatePositionServer(out int tickCounter);

            if (tickCounter != -1)
            {
                BroadcastPositionClientRpc(m_Position.Value, tickCounter);
            }
        }

        // Client sending its own inputs
        if (IsClient && IsOwner)
        {
            m_TickCounter++;
            UpdateInputClient();
        }
    }

    #region Server/Client updates
    private void UpdatePositionServer(out int tickCounter)
    {
        tickCounter = -1;
        // Consuming all inputs and updating server's position
        if (m_InputQueue.Count > 0)
        {
            var inputPair = m_InputQueue.Dequeue();
            var input = inputPair.Value;
            tickCounter = inputPair.Key;
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
            PredictMovement(inputDirection, Time.fixedDeltaTime);

            // Sending the inputs to the server
            SendInputServerRpc(inputDirection.normalized, m_TickCounter);
        }
    }
    #endregion

    #region Rpc
    [ServerRpc]
    private void SendInputServerRpc(Vector2 input, int tickCounter)
    {
        m_InputQueue.Enqueue(new KeyValuePair<int, Vector2>(tickCounter, input));
    }

    [ClientRpc]
    private void BroadcastPositionClientRpc(Vector2 position, int serverTick)
    {
        
        Reconcile(position, serverTick);
    }
    #endregion

    #region Prediction/Reconciliation
    // Local prediction for the owned ghost
    public void PredictMovement(Vector2 direction, float deltaTime)
    {
        if (!IsOwner || NetworkManager.Singleton == null) return;

        // Updating predicted position
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
        m_PredictionHistory[m_TickCounter] = m_PredictedPosition;
        
    }

    // Local reconciliation for the owned ghost
    public void Reconcile(Vector2 serverPosition, int tickCounter)
    {
        if (!IsOwner) return;

        if (m_PredictionHistory.TryGetValue(tickCounter, out var predictedPos))
        {
            Vector2 error = serverPosition - predictedPos;
            m_PredictedPosition += error;
            m_PredictionHistory.Clear();
        }
    }
    #endregion

}
