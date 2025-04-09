using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerGhost : NetworkBehaviour
{
    // Player's data
    [SerializeField] 
    private Player m_Player;
    private float m_Size = 1;
    [SerializeField] 
    private SpriteRenderer m_SpriteRenderer;    

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

    // Local ghost predictions
    private Vector2 m_PredictedPosition;
    private Dictionary<int, Vector2> m_PredictionHistory = new Dictionary<int, Vector2>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            m_SpriteRenderer.color = Color.red;
            m_PredictedPosition = m_Player.Position;
            m_Player.RegisterPlayerGhost(this);
        }
    }

    private void Update()
    {
        // Owned ghost follows predictions
        if (IsOwner)
        {
            transform.position = m_PredictedPosition;
        }
        // Non-owned ghosts follow server positions
        else
        {
            transform.position = m_Player.Position;
        }
    }

    // Local prediction for the owned ghost
    public void PredictMovement(Vector2 direction, float deltaTime)
    {
        if (!IsOwner || NetworkManager.Singleton == null) return;

        // Updating predicted position
        int currentTick = NetworkUtility.GetLocalTick();
        m_PredictedPosition += direction * m_Player.Velocity * deltaTime;
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
        m_PredictionHistory[currentTick] = m_PredictedPosition;
        
    }

    // Local reconciliation for the owned ghost
    public void Reconcile(Vector2 serverPosition, int serverTick)
    {
        if (!IsOwner) return;

        if (m_PredictionHistory.TryGetValue(serverTick, out var predictedPos))
        {
            Vector2 error = serverPosition - predictedPos;
            m_PredictedPosition += error;
            m_PredictionHistory.Clear();
        }
    }
}
