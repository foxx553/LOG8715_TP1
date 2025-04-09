using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    // Size, velocity, position
    [SerializeField]
    private float m_Velocity;
    public float Velocity => m_Velocity;
    [SerializeField]
    private float m_Size = 1;
    private NetworkVariable<Vector2> m_Position = new NetworkVariable<Vector2>();
    public Vector2 Position => m_Position.Value;

    // Local player
    private PlayerGhost m_PlayerGhost;
    public void RegisterPlayerGhost(PlayerGhost playerGhost)
    {
        m_PlayerGhost = playerGhost;
    }

    // Managing several inputs pressed together (example: up AND down) 
    private Queue<Vector2> m_InputQueue = new Queue<Vector2>();

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
            UpdatePositionServer();

            int currentTick = NetworkUtility.GetLocalTick();
            if (currentTick % 10 == 0)
            {
                BroadcastPositionClientRpc(m_Position.Value, currentTick);
            }
        }

        // Client sending its own inputs
        if (IsClient && IsOwner)
        {
            UpdateInputClient();
        }
    }

    private void UpdatePositionServer()
    {
        // Consuming all inputs and updating server's position
        if (m_InputQueue.Count > 0)
        {
            var input = m_InputQueue.Dequeue();
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
            m_PlayerGhost.PredictMovement(inputDirection, Time.fixedDeltaTime);

            // Sending the inputs to the server
            SendInputServerRpc(inputDirection.normalized);
        }
    }


    [ServerRpc]
    private void SendInputServerRpc(Vector2 input)
    {
        m_InputQueue.Enqueue(input);
    }

    [ClientRpc]
    private void BroadcastPositionClientRpc(Vector2 position, int serverTick)
    {
        if (m_PlayerGhost != null)
        {
            // Reconciliation for the local ghost
            m_PlayerGhost.Reconcile(position, serverTick);
        }
    }

}
