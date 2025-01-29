using UnityEngine;
using System.Collections.Generic;
using System;

public class HandleExplosion : ISystem{
    public string Name => "HandleExplosion";
    private ComponentDatabase _componentDatabase;

    public HandleExplosion(ComponentDatabase componentDatabase) {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){

        var ecsController = ECSController.Instance;
        var newSize = (int) Math.Ceiling(ecsController.Config.explosionSize / 4.0);
        var newPositionOffset = (float) (newSize / 1.9); // Instead of 2.0, to prevent unwanted collision
        List<uint> idsToRemove = new List<uint>();
        List<Vector2> newPositions = new List<Vector2>();
        List<Vector2> newVelocities = new List<Vector2>();

        foreach (var sizeEntry in _componentDatabase.sizeComponent)
        {
            if (sizeEntry.Value.Size >= ecsController.Config.explosionSize) {
                var currentId = sizeEntry.Key;
                idsToRemove.Add(sizeEntry.Key);
                var currentPosition = _componentDatabase.positionComponent[currentId].Position;
                var currentVelocity = _componentDatabase.velocityComponent[currentId].Velocity;
                var newVelocityOffset = Math.Sqrt(Math.Pow(currentVelocity.x,2) + Math.Pow(currentVelocity.y,2)) * Math.Sqrt(2.0);
                newPositions.AddRange(new List<Vector2>(){
                    new Vector2(currentPosition.x + newPositionOffset, currentPosition.y + newPositionOffset),
                    new Vector2(currentPosition.x + newPositionOffset, currentPosition.y - newPositionOffset),
                    new Vector2(currentPosition.x - newPositionOffset, currentPosition.y + newPositionOffset),
                    new Vector2(currentPosition.x - newPositionOffset, currentPosition.y - newPositionOffset),
                });
                newVelocities.AddRange(new List<Vector2>(){
                    new Vector2((float) newVelocityOffset, (float) newVelocityOffset),
                    new Vector2((float) newVelocityOffset, (float) -newVelocityOffset),
                    new Vector2((float) -newVelocityOffset, (float) newVelocityOffset),
                    new Vector2((float) -newVelocityOffset, (float) -newVelocityOffset),
                });
            }            
        }

        foreach (var id in idsToRemove){
            ecsController.DestroyShape(id);
            _componentDatabase.DestroyId(id);
        }

        for (int newCircleIndex = 0; newCircleIndex < newPositions.Count; newCircleIndex++) {
            _componentDatabase.UpdatePositionComponent(_componentDatabase.entitiesCounter, newPositions[newCircleIndex]);
            _componentDatabase.UpdateVelocityComponent(_componentDatabase.entitiesCounter, newVelocities[newCircleIndex]);
            _componentDatabase.UpdateSizeComponent(_componentDatabase.entitiesCounter, newSize);
            if (newSize == ecsController.Config.protectionSize)
                _componentDatabase.UpdateIsProtectable(_componentDatabase.entitiesCounter, 0);
            // _componentDatabase.UpdateProtectionComponent(_componentDatabase.entitiesCounter, 0);
            ecsController.CreateShape(_componentDatabase.entitiesCounter, _componentDatabase.sizeComponent[_componentDatabase.entitiesCounter].Size);
            ecsController.UpdateShapePosition(_componentDatabase.entitiesCounter, _componentDatabase.positionComponent[_componentDatabase.entitiesCounter].Position);
            _componentDatabase.entitiesCounter++;
        }
    }
}
