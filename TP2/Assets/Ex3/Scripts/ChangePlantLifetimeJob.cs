using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public struct ChangePlantLifetimeJob : IJob
{
    // Input data
    [ReadOnly] public NativeArray<float3> PreyPositions;
    [ReadOnly] public float3 PlantPosition;
    [ReadOnly] public float TouchingDistance;
    
    // Output data
    public NativeReference<float> DecreasingFactor;

    public void Execute()
    {
        // Default decreasing factor
        float factor = 1.0f;
        
        // Check each prey
        for (int i = 0; i < PreyPositions.Length; i++)
        {
            float distSq = math.distancesq(PlantPosition, PreyPositions[i]);
            
            if (distSq < TouchingDistance * TouchingDistance)
            {
                factor *= 2f;
                break;
            }
        }
        
        // Set the result
        DecreasingFactor.Value = factor;
    }
}

