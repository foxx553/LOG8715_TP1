using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : NetworkBehaviour
{
    [SerializeField]
    private GameObject m_GameArea;

    [SerializeField]
    private float m_StunDuration = 3.0f;

    [SerializeField]
    private Vector2 m_GameSize;

    public Vector2 GameSize { get => m_GameSize; }

    private NetworkVariable<bool> m_IsStunned = new NetworkVariable<bool>();

    public bool IsStunned { get => m_IsStunned.Value; }

    private Coroutine m_StunCoroutine;

    private float m_CurrentRtt;

    public float CurrentRTT { get => m_CurrentRtt / 1000f; }

    public NetworkVariable<float> ServerTime;

    // Local prediction of the stun
    private NetworkVariable<ulong> m_StunClientId = new NetworkVariable<ulong>();
    public ulong StunClientId { get => m_StunClientId.Value; }
    public bool m_PredictedIsStunned = false;

    private void Start()
    {
        m_GameArea.transform.localScale = new Vector3(m_GameSize.x * 2, m_GameSize.y * 2, 1);
        m_StunClientId.Value = ulong.MaxValue;
    }

    private void FixedUpdate()
    {
        if (IsSpawned)
        {
            m_CurrentRtt = NetworkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.ServerClientId);
        }

        if (IsServer)
        {
            ServerTime.Value = Time.time;
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (!IsServer)
        {
            SceneManager.LoadScene("StartupScene");
        }
    }

    #region Server stun
    public void Stun(ulong clientId)
    {
        if (m_StunCoroutine != null)
        {
            StopCoroutine(m_StunCoroutine);
        }
        if (IsServer)
        {
            m_StunCoroutine = StartCoroutine(StunCoroutine(clientId));
        }
    }

    private IEnumerator StunCoroutine(ulong clientId)
    {
        m_StunClientId.Value = clientId;
        m_IsStunned.Value = true;
        yield return new WaitForSeconds(m_StunDuration);
        m_IsStunned.Value = false;
        m_StunClientId.Value = ulong.MaxValue;
    }
    #endregion

    #region Predicted stun
    public void PredictedStun()
    {
        if (m_StunCoroutine != null)
        {
            StopCoroutine(m_StunCoroutine);
        }
        if (IsClient)
        {
            m_StunCoroutine = StartCoroutine(PredictedStunCoroutine());
        }
    }

    private IEnumerator PredictedStunCoroutine()
    {
        m_PredictedIsStunned = true;
        yield return new WaitForSeconds(m_StunDuration);
        m_PredictedIsStunned = false;
    }
    #endregion
}
