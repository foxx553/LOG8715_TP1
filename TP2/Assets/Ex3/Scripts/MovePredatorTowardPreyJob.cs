using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

// Include the BurstCompile attribute to Burst compile the job.
[BurstCompile]
public struct MovePredatorTowardPreyJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> PreyPositions;
    [ReadOnly] public NativeArray<float3> PredatorPositions;

    public NativeArray<float3> NearestPreyPositions;

    public void Execute(int index)
    {
        float3 predatorPos = PredatorPositions[index];
        float nearestDistSq = float.MaxValue;
        for (int i = 0; i < PreyPositions.Length; i++)
        {
            float3 preyPos = PreyPositions[i];
            float distSq = math.distancesq(predatorPos, preyPos);
            if (distSq < nearestDistSq)
            {
                nearestDistSq = distSq;
                NearestPreyPositions[index] = preyPos;
            }
        }
    }
}