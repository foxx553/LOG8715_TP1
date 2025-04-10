using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerGhost : NetworkBehaviour
{
    // Player's data
    [SerializeField] 
    private Player m_Player;
    [SerializeField] 
    private SpriteRenderer m_SpriteRenderer;    

    

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            m_SpriteRenderer.color = Color.red;
        }
    }

    private void Update()
    {
        // Owned ghost follows predictions
        if (IsOwner)
        {
            transform.position = m_Player.m_PredictedPosition;
        }
        // Non-owned ghosts follow server positions
        else
        {
            transform.position = m_Player.Position;
        }
    }
}
