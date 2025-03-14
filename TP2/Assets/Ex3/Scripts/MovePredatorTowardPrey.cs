using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MovePredatorTowardPrey : MonoBehaviour
{
    private Velocity _velocity;
    [ReadOnly] public NativeArray<float3> PreyPositions;
    [ReadOnly] public NativeArray<float3> PredatorPositions;
    public NativeArray<float3> NearestPreyPositions;
    
    public void Start()
    {
        PreyPositions = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length, Allocator.Persistent);
        PredatorPositions = new NativeArray<float3>(Ex4Spawner.PredatorTransforms.Length, Allocator.Persistent);
        NearestPreyPositions = new NativeArray<float3>(Ex4Spawner.PredatorTransforms.Length, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        PreyPositions.Dispose();
        PredatorPositions.Dispose();
        NearestPreyPositions.Dispose();
    }

    public void Update()
    {
        // Copy every Prey transform to a NativeArray.
        for (int i = 0; i < PreyPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            PreyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
        }

        // Copy every Predator transform to a NativeArray.
        for (int i = 0; i < PredatorPositions.Length; i++)
        {
            // Vector3 is implicitly converted to float3
            PredatorPositions[i] = Ex4Spawner.PredatorTransforms[i].position;
        }

        // To schedule a job, we first need to create an instance and populate its fields.
        MovePredatorTowardPreyJob findJob = new MovePredatorTowardPreyJob
        {
            PreyPositions = PreyPositions,
            PredatorPositions = PredatorPositions,
            NearestPreyPositions = NearestPreyPositions,
        };

        // Schedule() puts the job instance on the job queue.
        JobHandle findHandle = findJob.Schedule(PredatorPositions.Length, 100);

        // The Complete method will not return until the job represented by
        // the handle finishes execution. Effectively, the main thread waits
        // here until the job is done.
        findHandle.Complete();
        for (int i = 0; i < PredatorPositions.Length; i++)
        {
            Velocity predatorVelocity = Ex4Spawner.PredatorTransforms[i].GetComponent<Velocity>();
            if (predatorVelocity != null)
            {
                float3 direction = NearestPreyPositions[i] - (float3)Ex4Spawner.PredatorTransforms[i].position;
                predatorVelocity.velocity = direction * Ex3Config.PredatorSpeed;
            }
        }
    }
}
