using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerGhost : NetworkBehaviour
{
    [SerializeField] 
    private Player m_Player;
    [SerializeField] 
    private SpriteRenderer m_SpriteRenderer;

    // For client prediction
    private Vector2 m_PredictedPosition;
    private bool m_IsPredicting = false;
    private float m_Size = 1;

    private GameState m_GameState;

    // GameState peut etre nul si l'entite joueur est instanciee avant de charger MainScene
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

    public override void OnNetworkSpawn()
    {
        // L'entite qui appartient au client est recoloriee en rouge
        if (IsOwner)
        {
            m_SpriteRenderer.color = Color.red;
            
            // Initialize predicted position to current position
            m_PredictedPosition = m_Player.Position;

            // Register the ghost with the Player component
            m_Player.RegisterPlayerGhost(this);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            // Use predicted position if we're predicting
            transform.position = m_IsPredicting ? m_PredictedPosition : m_Player.Position;
        }
        else
        {
            // Non-owned clients just follow server position
            transform.position = m_Player.Position;
        }
    }


    // Called by Player when input is detected
    public void PredictMovement(Vector2 direction, float deltaTime)
    {
        if (!IsOwner) return;

        m_IsPredicting = true;
        m_PredictedPosition += direction * m_Player.Velocity * deltaTime;
        // Gestion des collisions avec l'exterieur de la zone de simulation
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
        
    }

    // Called when server updates arrive
    public void Reconcile(Vector2 serverPosition)
    {
        if (!IsOwner) return;

        m_IsPredicting = false;
        m_PredictedPosition = serverPosition;
    }
}
