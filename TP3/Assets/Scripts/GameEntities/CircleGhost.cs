using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CircleGhost : NetworkBehaviour
{
    [SerializeField]
    private MovingCircle m_MovingCircle;

    private void Update()
    {
        // Client ghost follows predictions
        if (IsClient)
        {
            transform.position = m_MovingCircle.m_PredictedPosition;
        }
        // Server ghost obeys follows server calculations
        if (IsServer)
        {
            transform.position = m_MovingCircle.Position;
        }
        

    }
}
