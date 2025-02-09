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
        float newPositionOffset;
        int newSize;

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id ++){

            List<Vector2> newPositions = new List<Vector2>();
            List<Vector2> newVelocities = new List<Vector2>();

            if (_componentDatabase.sizeComponents[id] == null) continue;

            SizeComponent sizeEntry = _componentDatabase.sizeComponents[id];
            if (_componentDatabase.positionComponents[id].Position.x > 0f ////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            if (sizeEntry.Size < ecsController.Config.explosionSize) continue;

            // Futur update: refactor this explosion code into a static public function //
            newSize = (int) Math.Ceiling(_componentDatabase.sizeComponents[id].Size / 4.0);
            newPositionOffset = (float) (newSize / 1.9); // Instead of 2.0, to prevent unwanted collision
            var currentPosition = _componentDatabase.positionComponents[id].Position;
            var currentVelocityMagnitude = _componentDatabase.velocityComponents[id].Velocity.magnitude;
            var newVelocityOffset = currentVelocityMagnitude; // Instead of "/ Math.Sqrt(2.0);"

            Vector2 positionDelta1 = new Vector2(newPositionOffset, newPositionOffset);
            Vector2 positionDelta2 = new Vector2(- newPositionOffset, newPositionOffset);

            _componentDatabase.sizeComponents[id].Size = 0;

            newPositions.Add(currentPosition + positionDelta1);
            newPositions.Add(currentPosition - positionDelta1);
            newPositions.Add(currentPosition + positionDelta2);
            newPositions.Add(currentPosition - positionDelta2);

            newVelocities.Add(new Vector2((float) newVelocityOffset, (float) newVelocityOffset));
            newVelocities.Add(new Vector2((float) -newVelocityOffset, (float) -newVelocityOffset));
            newVelocities.Add(new Vector2((float) -newVelocityOffset, (float) newVelocityOffset));
            newVelocities.Add(new Vector2((float) newVelocityOffset, (float) -newVelocityOffset));
            //////////////////////////////////////////////////////////////////////////////

            for (int i = 0; i < 4; i++){
                int n = _componentDatabase.availableIds.Count;
                uint newId;
                if (n != 0){
                    newId = _componentDatabase.availableIds[0];
                    _componentDatabase.availableIds.RemoveAt(0);
                }
                else {
                    newId = _componentDatabase.entitiesCounter;
                    _componentDatabase.entitiesCounter++;
                }

                _componentDatabase.UpdateSizeComponent(newId, newSize);
                _componentDatabase.UpdatePositionComponent(newId, newPositions[i]);
                _componentDatabase.UpdateVelocityComponent(newId, newVelocities[i]);
                _componentDatabase.UpdateIsExploded(newId, true);
                ecsController.CreateShape(newId, newSize);
                ecsController.UpdateShapePosition(newId, newPositions[i]);
            }
        }
    }
}
