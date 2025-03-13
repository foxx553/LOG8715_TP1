using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class ChangePlantLifetime : MonoBehaviour
{
    private Lifetime _lifetime;
    private NativeArray<float3> _preyPositions;
    private NativeReference<float> _decreasingFactor;
    
    public void Start()
    {
        _lifetime = GetComponent<Lifetime>();
        _decreasingFactor = new NativeReference<float>(1.0f, Allocator.Persistent);
    }

    public void OnDestroy()
    {
        // Clean up native collections
        if (_preyPositions.IsCreated)
            _preyPositions.Dispose();
        
        if (_decreasingFactor.IsCreated)
            _decreasingFactor.Dispose();
    }

    public void Update()
    {
        // Initialize or resize prey positions array if needed
        int preyCount = Ex4Spawner.PreyTransforms.Count;
        if (!_preyPositions.IsCreated || _preyPositions.Length > preyCount)
        {
            if (_preyPositions.IsCreated)
                _preyPositions.Dispose();
                
            _preyPositions = new NativeArray<float3>(preyCount, Allocator.Persistent);
        }

        // Copy prey positions to native array
        for (int i = 0; i < preyCount; i++)
        {
            _preyPositions[i] = Ex4Spawner.PreyTransforms[i].position;
        }

        // Create and schedule job
        var job = new ChangePlantLifetimeJob
        {
            PreyPositions = _preyPositions,
            PlantPosition = transform.position,
            TouchingDistance = Ex3Config.TouchingDistance,
            DecreasingFactor = _decreasingFactor
        };

        // Schedule and complete job
        JobHandle handle = job.Schedule();
        handle.Complete();

        // Apply job result to lifetime component
        _lifetime.decreasingFactor = _decreasingFactor.Value;
    }
}