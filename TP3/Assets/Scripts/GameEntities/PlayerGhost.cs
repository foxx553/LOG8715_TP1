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
    }

    // Called when server updates arrive
    public void Reconcile(Vector2 serverPosition)
    {
        if (!IsOwner) return;

        m_IsPredicting = false;
        m_PredictedPosition = serverPosition;
    }
}
