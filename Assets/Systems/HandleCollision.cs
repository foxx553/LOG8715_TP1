using UnityEngine;
using System.Collections.Generic;

public class HandleCollision : ISystem
{
    public string Name => "HandleCollision";
    private ComponentDatabase _componentDatabase;

    public HandleCollision(ComponentDatabase componentDatabase)
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

        foreach (var entry1 in _componentDatabase.positionComponent)
        {
            uint id1 = entry1.Key;
            Vector2 position1 = entry1.Value.Position;
            Vector2 velocity1 = _componentDatabase.velocityComponent[id1].Velocity;
            float size1 = _componentDatabase.sizeComponent[id1].Size;

            // Calculate borders
            float leftBorder = -(horizontalExtent - size1 / 2);
            float rightBorder = horizontalExtent - size1 / 2;
            float bottomBorder = -(verticalExtent - size1 / 2);
            float topBorder = verticalExtent - size1 / 2;

            // Collision between circles
            foreach (var entry2 in _componentDatabase.positionComponent)
            {
                uint id2 = entry2.Key;

                if (id1 >= id2) continue;

                Vector2 position2 = entry2.Value.Position;
                float size2 = _componentDatabase.sizeComponent[id2].Size;

                float distance = (position1 - position2).magnitude;
                float radiusSum = size1 / 2 + size2 / 2;

                if (distance <= radiusSum)
                {

                    Vector2 velocity2 = _componentDatabase.velocityComponent[id2].Velocity;

                    CollisionResult collisionResult = CollisionUtility.CalculateCollision(position1,
                        velocity1, size1, position2, velocity2, size2);

                    if (deltaPositions.ContainsKey(id1))
                    {
                        deltaPositions[id1] += collisionResult.position1 - position1;
                        deltaVelocities[id1] += collisionResult.velocity1 - velocity1;
                    }
                    else
                    {
                        deltaPositions[id1] = collisionResult.position1 - position1;
                        deltaVelocities[id1] = collisionResult.velocity1 - velocity1;
                    }

                    if (deltaPositions.ContainsKey(id2))
                    {
                        deltaPositions[id2] += collisionResult.position2 - position2;
                        deltaVelocities[id2] += collisionResult.velocity2 - velocity2;
                    }
                    else
                    {
                        deltaPositions[id2] = collisionResult.position2 - position2;
                        deltaVelocities[id2] = collisionResult.velocity2 - velocity2;
                    }

                    // Size adjustement
                    if (_componentDatabase.isImmortal.ContainsKey(id1)
                        || _componentDatabase.isImmortal.ContainsKey(id2))
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

                    if (size1 == size2 && size1 <= ecsController.Config.protectionSize)
                    {
                        if (_componentDatabase.isProtectable[id1].ProtectionCount < ecsController.Config.protectionCollisionCount)
                            _componentDatabase.isProtectable[id1].ProtectionCount++;
                        if (_componentDatabase.isProtectable[id2].ProtectionCount < ecsController.Config.protectionCollisionCount)
                            _componentDatabase.isProtectable[id1].ProtectionCount++;

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
                }
            }

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
        }

        // Apply all the updates to the position and velocity components
        foreach (var update in deltaPositions)
        {
            uint id = update.Key;

            _componentDatabase.positionComponent[id].Position += update.Value;
            _componentDatabase.velocityComponent[id].Velocity += deltaVelocities[id];
            if (deltaSizes.ContainsKey(id))
            {
                _componentDatabase.sizeComponent[id].Size += deltaSizes[id];
                if (_componentDatabase.sizeComponent[id].Size <= ecsController.Config.protectionSize)
                {
                    _componentDatabase.UpdateIsProtectable(id, 0);
                }
            }

            _componentDatabase.UpdateIsCollidiing(id, true);
        }
    }
}
