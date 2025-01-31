using UnityEngine;
using System.Collections.Generic;
using System;

public class HandleExplosionArray : ISystem{
    public string Name => "HandleExplosionArray";
    private ComponentDatabaseArray _componentDatabase;

    public HandleExplosionArray(ComponentDatabaseArray componentDatabase) {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){

        var ecsController = ECSController.Instance;
        var newSize = (int) Math.Ceiling(ecsController.Config.explosionSize / 4.0);
        var newPositionOffset = (float) (newSize / 1.9); // Instead of 2.0, to prevent unwanted collision

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id ++){

            List<Vector2> newPositions = new List<Vector2>();
            List<Vector2> newVelocities = new List<Vector2>();

            if (_componentDatabase.sizeComponents[id] == null) continue;

            SizeComponent sizeEntry = _componentDatabase.sizeComponents[id];

            if (sizeEntry.Size < ecsController.Config.explosionSize) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f ////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            var currentPosition = _componentDatabase.positionComponents[id].Position;
            var currentVelocityMagnitude = _componentDatabase.velocityComponents[id].Velocity.magnitude;
            var newVelocityOffset = currentVelocityMagnitude / Math.Sqrt(2.0);

            Vector2 positionDelta1 = new Vector2(newPositionOffset, newPositionOffset);
            Vector2 positionDelta2 = new Vector2(- newPositionOffset, newPositionOffset);

            Vector2 velocityDelta1 = new Vector2((float) newVelocityOffset, (float) newVelocityOffset);
            Vector2 velocityDelta2 = new Vector2((float) -newVelocityOffset, (float) newVelocityOffset);

            _componentDatabase.sizeComponents[id].Size = 0;

            newPositions.Add(currentPosition + positionDelta1);
            newPositions.Add(currentPosition - positionDelta1);
            newPositions.Add(currentPosition + positionDelta2);
            newPositions.Add(currentPosition - positionDelta2);

            newVelocities.Add(velocityDelta1);
            newVelocities.Add(-velocityDelta1);
            newVelocities.Add(velocityDelta2);
            newVelocities.Add(-velocityDelta2);

            for (int i = 0; i < 4; i++){
                int n = _componentDatabase.availableIds.Count;
                uint newId = 0;
                if (n != 0){
                    newId = _componentDatabase.availableIds[n - 1];
                    _componentDatabase.availableIds.RemoveAt(n - 1);
                }
                else {
                    newId = _componentDatabase.entitiesCounter;
                    _componentDatabase.entitiesCounter++;
                }

                _componentDatabase.UpdateSizeComponent(newId, newSize);
                _componentDatabase.UpdatePositionComponent(newId, newPositions[i]);
                _componentDatabase.UpdateVelocityComponent(newId, newVelocities[i]);
                ecsController.CreateShape(newId, newSize);

                if (newSize == ecsController.Config.protectionSize)
                    _componentDatabase.UpdateIsProtectable(newId, 0);
            }
        }
    }
}
