using UnityEngine;
using System.Collections.Generic;

public class HandleCollisionArray : ISystem
{
    public string Name => "HandleCollisionArray";
    private ComponentDatabaseArray _componentDatabase;

    public HandleCollisionArray(ComponentDatabaseArray componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem()
    {
        var ecsController = ECSController.Instance;

        Dictionary<uint, Vector2> deltaPositions = new();
        Dictionary<uint, Vector2> deltaVelocities = new();
        Dictionary<uint, int> deltaSizes = new();

        float verticalExtent = Camera.main.orthographicSize;
        float horizontalExtent = verticalExtent * Camera.main.aspect;

        for (uint id1 = 0; id1 < _componentDatabase.entitiesCounter; id1++){
            
            if (_componentDatabase.positionComponents[id1] == null) continue;

            Vector2 position1 = _componentDatabase.positionComponents[id1].Position;

            if (_componentDatabase.velocityComponents[id1] == null) continue;

            Vector2 velocity1 = _componentDatabase.velocityComponents[id1].Velocity;
            float size1 = _componentDatabase.sizeComponents[id1].Size;

            // Calculate borders
            float leftBorder = -(horizontalExtent - size1 / 2);
            float rightBorder = horizontalExtent - size1 / 2;
            float bottomBorder = -(verticalExtent - size1 / 2);
            float topBorder = verticalExtent - size1 / 2;
            
            // Wall collision (Horizontal)

            if (position1.x < leftBorder || position1.x > rightBorder)
            {
                float newPosition = Mathf.Clamp(position1.x, leftBorder, rightBorder);

                if (deltaPositions.ContainsKey(id1))
                {
                    Vector2 currentDelta = deltaPositions[id1];
                    Vector2 currentDeltaVelocity = deltaVelocities[id1];

                    currentDelta.x += newPosition - position1.x;
                    deltaPositions[id1] = currentDelta;

                    currentDeltaVelocity.x -= 2 * velocity1.x;
                    deltaVelocities[id1] = currentDeltaVelocity;
                }
                else
                {
                    deltaPositions[id1] = new Vector2(newPosition - position1.x, 0f);
                    deltaVelocities[id1] = new Vector2(-2 * velocity1.x, 0f);
                }
            }

            // Wall collision (Vertical)
            if (position1.y < bottomBorder || position1.y > topBorder)
            {
                float newPosition = Mathf.Clamp(position1.y, bottomBorder, topBorder);

                if (deltaPositions.ContainsKey(id1))
                {
                    Vector2 currentDelta = deltaPositions[id1];
                    Vector2 currentDeltaVelocity = deltaVelocities[id1];

                    currentDelta.y += newPosition - position1.y;
                    deltaPositions[id1] = currentDelta;

                    currentDeltaVelocity.y -= 2 * velocity1.y;
                    deltaVelocities[id1] = currentDeltaVelocity;
                }
                else
                {
                    deltaPositions[id1] = new Vector2(0f, newPosition - position1.y);
                    deltaVelocities[id1] = new Vector2(0f, -2 * velocity1.y);
                }
            }

            for (uint id2 = id1+1; id2 < _componentDatabase.entitiesCounter; id2++){

                if (_componentDatabase.positionComponents[id2] == null) continue;

                Vector2 position2 = _componentDatabase.positionComponents[id2].Position;

                if (_componentDatabase.velocityComponents[id2] == null) continue;

                Vector2 velocity2 = _componentDatabase.velocityComponents[id2].Velocity;
                float size2 = _componentDatabase.sizeComponents[id2].Size;

                float distance = (position1 - position2).magnitude;
                float radiusSum = size1 / 2 + size2 / 2;

                if (distance > radiusSum) continue;
                CollisionResult collisionResult = CollisionUtility.CalculateCollision(position1,
                velocity1, size1, position2, velocity2, size2);

                if (!deltaPositions.ContainsKey(id1)){
                    deltaPositions[id1] = Vector2.zero;
                    deltaVelocities[id1] = Vector2.zero;
                }
                deltaPositions[id1] += collisionResult.position1 - position1;
                deltaVelocities[id1] += collisionResult.velocity1 - velocity1;

                if (!deltaPositions.ContainsKey(id2)){
                    deltaPositions[id2] = Vector2.zero;
                    deltaVelocities[id2] = Vector2.zero;
                }
                deltaPositions[id2] += collisionResult.position2 - position2;
                deltaVelocities[id2] += collisionResult.velocity2 - velocity2;

                // Size adjustement
                if ((_componentDatabase.isImmortals[id1] != null)
                    || (_componentDatabase.isImmortals[id2] != null))
                {
                    continue;
                }

                if (!deltaSizes.ContainsKey(id1))
                {
                    deltaSizes[id1] = 0;
                }
                if (!deltaSizes.ContainsKey(id2))
                {
                    deltaSizes[id2] = 0;
                }

                if (_componentDatabase.isProtecteds[id1] != null && _componentDatabase.isProtecteds[id2] != null) {
                    // do nothing
                } else if (_componentDatabase.isProtecteds[id1] != null) {
                    if (size2 > size1) {
                        deltaSizes[id2]--;
                    }
                } else if (_componentDatabase.isProtecteds[id2] != null) {
                    if (size1 > size2) {
                        deltaSizes[id1]--;
                    }
                }
                else if (size1 > size2)
                {
                    deltaSizes[id1]++;
                    deltaSizes[id2]--;
                }
                else if (size1 < size2)
                {
                    deltaSizes[id1]--;
                    deltaSizes[id2]++;
                }

                if (size1 == size2)
                {
                    if (_componentDatabase.isProtectables[id1] != null){
                        _componentDatabase.isProtectables[id1].ProtectionCount++;
                    }
                    if (_componentDatabase.isProtectables[id2] != null){
                        _componentDatabase.isProtectables[id2].ProtectionCount++;
                    }  
                }
            }
        }

        // Apply all the updates to the position and velocity components
        foreach (var update in deltaPositions)
        {
            uint id = update.Key;
            if (_componentDatabase.positionComponents[id].Position.x > 0f ////
            && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            _componentDatabase.positionComponents[id].Position += update.Value;
            _componentDatabase.velocityComponents[id].Velocity += deltaVelocities[id];
            if (deltaSizes.ContainsKey(id))
            {
                _componentDatabase.sizeComponents[id].Size += deltaSizes[id];
                if (_componentDatabase.sizeComponents[id].Size <= ecsController.Config.protectionSize)
                {
                    _componentDatabase.UpdateIsProtectable(id, 0);
                }
            }
            _componentDatabase.UpdateIsColliding(id, true);
        }
    }
}
