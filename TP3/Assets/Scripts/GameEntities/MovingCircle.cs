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

    public int m_TickCounter = 0;
    public int SERVER_RECONCILIATION_RATE = 100;

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

        if (m_TickCounter % SERVER_RECONCILIATION_RATE == 0) Reconcile();
    }

    public void Reconcile()
    {
        m_PredictedPosition = m_Position.Value;
        m_PredictedVelocity = m_Velocity.Value;
    }
    #endregion
}
