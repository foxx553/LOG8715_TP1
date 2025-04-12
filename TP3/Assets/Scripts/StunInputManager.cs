using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StunInputManager : NetworkBehaviour
{
    [SerializeField]
    private GameState m_GameState;

    private void Update()
    {
        if (IsClient)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_GameState.PredictedStun();
                ActivateStunServerRpc();
            }
        }
    }

    [ServerRpc (RequireOwnership = false)]
    private void ActivateStunServerRpc(ServerRpcParams rpcParams = default)
    {
        // Keeping the ID of the client who triggered the stun
        ulong stunClientId = rpcParams.Receive.SenderClientId;
        m_GameState.Stun(stunClientId);
    }
}
