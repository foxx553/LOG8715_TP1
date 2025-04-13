using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingCircle : NetworkBehaviour
{

    #region Attributes
    [SerializeField]
    private float m_Radius = 1;

    public Vector2 Position => m_Position.Value;

    public Vector2 Velocity => m_Velocity.Value;

    public Vector2 InitialPosition;
    public Vector2 InitialVelocity;

    private NetworkVariable<Vector2> m_Position = new NetworkVariable<Vector2>();
    private NetworkVariable<Vector2> m_Velocity = new NetworkVariable<Vector2>();

    // Local ghost predictions
    public Vector2 m_PredictedPosition;
    public Vector2 m_PredictedVelocity;
    private Dictionary<int, Vector2> m_PredictedPositionHistory = new Dictionary<int, Vector2>();
    private Dictionary<int, Vector2> m_PredictedVelocityHistory = new Dictionary<int, Vector2>();

    // History counter
    public int m_TickCounter = 0;
    public int SERVER_RECONCILIATION_RATE = 500;

    private GameState m_GameState;
    #endregion

    private void Awake()
    {
        m_GameState = FindObjectOfType<GameState>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            m_Position.Value = InitialPosition;
            m_Velocity.Value = InitialVelocity;
        }
        else
        {
            m_PredictedPosition = m_Position.Value;
            m_PredictedVelocity = m_Velocity.Value;
        }
    }

    private void FixedUpdate()
    {
        // Server updating its positions
        if (IsServer)
        {
            if (m_GameState.IsStunned) return;

            UpdatePositionServer();
        }
        // Client updating its predictions
        else
        {
            ulong myClientId = NetworkManager.Singleton.LocalClientId;
            
            // Applying the immediate stun to the client who triggered it
            if (m_GameState.m_PredictedIsStunned) return;

            // Applying the server stun for other clients
            if (m_GameState.IsStunned && m_GameState.StunClientId != myClientId) return;

            m_TickCounter++;
            PredictMovement();
        }
    }

    #region Server updates
    private void UpdatePositionServer()
    {
        m_Position.Value += m_Velocity.Value * Time.deltaTime;

        var size = m_GameState.GameSize;
        if (m_Position.Value.x - m_Radius < -size.x)
        {
            m_Position.Value = new Vector2(-size.x + m_Radius, m_Position.Value.y);
            m_Velocity.Value *= new Vector2(-1, 1);
        }
        else if (m_Position.Value.x + m_Radius > size.x)
        {
            m_Position.Value = new Vector2(size.x - m_Radius, m_Position.Value.y);
            m_Velocity.Value *= new Vector2(-1, 1);
        }

        if (m_Position.Value.y + m_Radius > size.y)
        {
            m_Position.Value = new Vector2(m_Position.Value.x, size.y - m_Radius);
            m_Velocity.Value *= new Vector2(1, -1);
        }
        else if (m_Position.Value.y - m_Radius < -size.y)
        {
            m_Position.Value = new Vector2(m_Position.Value.x, -size.y + m_Radius);
            m_Velocity.Value *= new Vector2(1, -1);
        }
    }
    #endregion

    #region Client prediction/reconciliation
    private void PredictMovement()
    {
        m_PredictedPosition += m_PredictedVelocity * Time.deltaTime;

        var size = m_GameState.GameSize;
        if (m_PredictedPosition.x - m_Radius < -size.x)
        {
            m_PredictedPosition = new Vector2(-size.x + m_Radius, m_PredictedPosition.y);
            m_PredictedVelocity *= new Vector2(-1, 1);
        }
        else if (m_PredictedPosition.x + m_Radius > size.x)
        {
            m_PredictedPosition = new Vector2(size.x - m_Radius, m_PredictedPosition.y);
            m_PredictedVelocity *= new Vector2(-1, 1);
        }

        if (m_PredictedPosition.y + m_Radius > size.y)
        {
            m_PredictedPosition = new Vector2(m_PredictedPosition.x, size.y - m_Radius);
            m_PredictedVelocity *= new Vector2(1, -1);
        }
        else if (m_PredictedPosition.y - m_Radius < -size.y)
        {
            m_PredictedPosition = new Vector2(m_PredictedPosition.x, -size.y + m_Radius);
            m_PredictedVelocity *= new Vector2(1, -1);
        }

        m_PredictedPositionHistory[m_TickCounter] = m_PredictedPosition;
        m_PredictedVelocityHistory[m_TickCounter] = m_PredictedVelocity;

        if (m_TickCounter % SERVER_RECONCILIATION_RATE == 0) SendTickValueServerRpc(m_TickCounter);
    }

    public void Reconcile(Vector2 serverPosition, Vector2 serverVelocity, int tickCounter)
    {
        // If there was a prediction, checking it and correcting the client's history if necessary
        if (m_PredictedPositionHistory.TryGetValue(tickCounter, out var predictedPosition)
            && m_PredictedVelocityHistory.TryGetValue(tickCounter, out var predictedVelocity))
        {
            Debug.Log("Reconciliation");
            Vector2 errorPosition = serverPosition - predictedPosition;
            Vector2 errorVelocity = serverVelocity - predictedVelocity;
            if (errorPosition.sqrMagnitude > 0.001f || errorVelocity.sqrMagnitude > 0.001f)
            {
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
                    m_PredictedVelocityHistory.Remove(key);
                }
                keysToSimulate.Sort();

                // Recalculating the whole history
                m_PredictedPositionHistory[tickCounter] = serverPosition;
                m_PredictedVelocityHistory[tickCounter] = serverVelocity;
                for (int i = 1; i < keysToSimulate.Count; i++)
                {
                    m_PredictedPositionHistory[keysToSimulate[i]] = m_PredictedPositionHistory[keysToSimulate[i - 1]] + m_PredictedVelocityHistory[keysToSimulate[i - 1]] * Time.deltaTime;
                    m_PredictedVelocityHistory[keysToSimulate[i]] = m_PredictedVelocityHistory[keysToSimulate[i - 1]];

                    var size = m_GameState.GameSize;
                    if (m_PredictedPositionHistory[keysToSimulate[i]].x - m_Radius < -size.x)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(-size.x + m_Radius, m_PredictedPositionHistory[keysToSimulate[i]].y);
                        m_PredictedVelocityHistory[keysToSimulate[i]] *= new Vector2(-1, 1);
                    }
                    else if (m_PredictedPositionHistory[keysToSimulate[i]].x + m_Radius > size.x)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(size.x - m_Radius, m_PredictedPositionHistory[keysToSimulate[i]].y);
                        m_PredictedVelocityHistory[keysToSimulate[i]] *= new Vector2(-1, 1);
                    }

                    if (m_PredictedPositionHistory[keysToSimulate[i]].y + m_Radius > size.y)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(m_PredictedPositionHistory[keysToSimulate[i]].x, size.y - m_Radius);
                        m_PredictedVelocityHistory[keysToSimulate[i]] *= new Vector2(1, -1);
                    }
                    else if (m_PredictedPositionHistory[keysToSimulate[i]].y - m_Radius < -size.y)
                    {
                        m_PredictedPositionHistory[keysToSimulate[i]] = new Vector2(m_PredictedPositionHistory[keysToSimulate[i]].x, -size.y + m_Radius);
                        m_PredictedVelocityHistory[keysToSimulate[i]] *= new Vector2(1, -1);
                    }
                }
                m_PredictedPosition = m_PredictedPositionHistory[keysToSimulate[keysToSimulate.Count - 1]];
                m_PredictedVelocity = m_PredictedVelocityHistory[keysToSimulate[keysToSimulate.Count - 1]];
            }
        }
    }
    #endregion

    #region Rpc

    [ServerRpc (RequireOwnership = false)]
    private void SendTickValueServerRpc(int tickCounter)
    {
        // Sync with client by sending immediately back the server calculated position/velocity
        SendPositionVelocityClientRpc(m_Position.Value, m_Velocity.Value, tickCounter);
    }

    [ClientRpc]
    private void SendPositionVelocityClientRpc(Vector2 serverPosition, Vector2 serverVelocity, int tickCounter)
    {
        // Reconciliating using server's answer
        Reconcile(serverPosition, serverVelocity, tickCounter);
    }

    #endregion

}
