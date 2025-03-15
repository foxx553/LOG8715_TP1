// using Unity.Collections;
// using Unity.Jobs;
// using Unity.Mathematics;
// using UnityEngine;

// public class MovePreyTowardPlant : MonoBehaviour
// {
//     private Velocity _velocity;
//     [ReadOnly] public NativeArray<float3> PlantPositions;
//     [ReadOnly] public NativeArray<float3> PreyPositions;
//     public NativeArray<float3> NearestPlantPositions;

//     public void Start()
//     {
//         PlantPositions = new NativeArray<float3>(Ex4Spawner.PlantTransforms.Length, Allocator.Persistent);
//         PreyPositions = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length, Allocator.Persistent);
//         NearestPlantPositions = new NativeArray<float3>(Ex4Spawner.PreyTransforms.Length, Allocator.Persistent);
//     }

//     public void OnDestroy()
//     {
//         PlantPositions.Dispose();
//         PreyPositions.Dispose();
//         NearestPlantPositions.Dispose();
//     }

//     public void Update()
//     {
//         // Copy every Plant transform to a NativeArray.
//         for (int i = 0; i < PlantPositions.Length; i++)
//         {
//             // Vector3 is implicitly converted to float3
//             PlantPositions[i] = Ex4Spawner.PlantTransforms[i].position;
//         }

//         // Copy every Prey transform to a NativeArray.
//         for (int i = 0; i < PreyPositions.Length; i++)
//         {
//             // Vector3 is implicitly converted to float3
//             PreyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
//         }

//         // To schedule a job, we first need to create an instance and populate its fields.
//         MovePreyTowardPlantJob findJob = new MovePreyTowardPlantJob
//         {
//             PlantPositions = PlantPositions,
//             PreyPositions = PreyPositions,
//             NearestPlantPositions = NearestPlantPositions,
//         };

//         // Schedule() puts the job instance on the job queue.
//         JobHandle findHandle = findJob.Schedule(PreyPositions.Length, 100);

//         // The Complete method will not return until the job represented by
//         // the handle finishes execution. Effectively, the main thread waits
//         // here until the job is done.
//         findHandle.Complete();
//         for (int i = 0; i < PreyPositions.Length; i++)
//         {
//             Velocity preyVelocity = Ex4Spawner.PreyTransforms[i].GetComponent<Velocity>();
//             if (preyVelocity != null)
//             {
//                 float3 direction = NearestPlantPositions[i] - (float3)Ex4Spawner.PreyTransforms[i].position;
//                 preyVelocity.velocity = direction * Ex3Config.PreySpeed;
//             }
//         }
//     }

// }
