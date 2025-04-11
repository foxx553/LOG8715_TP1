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
        // Seul le serveur peut mettre a jour la position et la vitesse des cercles.
        if (IsServer)
        {
            UpdatePositionServer();
        }
        else
        {
            UpdatePositionClient();
        }
    }

    #region Server/Client updates
    private void UpdatePositionServer()
    {
        if (m_GameState.IsStunned) return;

        // Mise a jour de la position du cercle selon sa vitesse
        m_Position.Value += m_Velocity.Value * Time.deltaTime;

        // Gestion des collisions avec l'exterieur de la zone de simulation
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

    private void UpdatePositionClient()
    {
        ulong myClientId = NetworkManager.Singleton.LocalClientId;
        if (m_GameState.m_PredictedIsStunned) return;
        if (m_GameState.IsStunned && m_GameState.StunClientId != myClientId) return;

        // Mise a jour de la position du cercle selon sa vitesse
        m_PredictedPosition += m_PredictedVelocity * Time.deltaTime;

        // Gestion des collisions avec l'exterieur de la zone de simulation
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
    }
    #endregion
}
