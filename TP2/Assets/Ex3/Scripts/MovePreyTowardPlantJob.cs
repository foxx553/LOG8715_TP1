// using Unity.Burst;
// using Unity.Collections;
// using Unity.Jobs;
// using Unity.Mathematics;

// // Include the BurstCompile attribute to Burst compile the job.
// [BurstCompile]
// public struct MovePreyTowardPlantJob : IJobParallelFor
// {
//     [ReadOnly] public NativeArray<float3> PlantPositions;
//     [ReadOnly] public NativeArray<float3> PreyPositions;

//     public NativeArray<float3> NearestPlantPositions;

//     public void Execute(int index)
//     {
//         float3 preyPos = PreyPositions[index];
//         float nearestDistSq = float.MaxValue;
//         for (int j = 0; j < PlantPositions.Length; j++)
//         {
//             float3 plantPos = PlantPositions[j];
//             float distSq = math.distancesq(preyPos, plantPos);
//             if (distSq < nearestDistSq)
//             {
//                 nearestDistSq = distSq;
//                 NearestPlantPositions[index] = plantPos;
//             }
//         }
//     }
// }