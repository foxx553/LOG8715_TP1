using System;
using System.Collections.Generic;
using UnityEngine;

public class HandleMouseClick : ISystem
{
    public string Name => "HandleMouseClick";
    private ComponentDatabaseArray _componentDatabase;

    public HandleMouseClick(ComponentDatabaseArray componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem()
    {
        var ecsController = ECSController.Instance;

        if (Input.GetMouseButtonDown(0)) {
            var mouse3DScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var mouse2DScreenPosition = new Vector2(mouse3DScreenPosition.x, mouse3DScreenPosition.y);
            List<Vector2> newPositions = new List<Vector2>();
            List<Vector2> newVelocities = new List<Vector2>();
            var newSize = 0;
            bool explodedCircle = false;

            for (uint id = 0; id < _componentDatabase.entitiesCounter; id ++) {
                if (_componentDatabase.positionComponents[id] == null) continue;

                if (_componentDatabase.isStatics[id] != null) continue;

                var currentCenter = _componentDatabase.positionComponents[id].Position;
                var currentSize = _componentDatabase.sizeComponents[id].Size;
                var distanceToCenter = Math.Pow(currentCenter.x - mouse2DScreenPosition.x, 2) + Math.Pow(currentCenter.y - mouse2DScreenPosition.y, 2);

                if (distanceToCenter <= Math.Pow(currentSize, 2)) {
                    if (currentSize < 4) {
                        // Will be destroyed later in the tick by UpdateSize system
                        _componentDatabase.sizeComponents[id].Size = 0; 
                    } else {

                        // Futur update: refactor this explosion code into a static public function //
                        newSize = (int) Math.Ceiling(_componentDatabase.sizeComponents[id].Size / 4.0);
                        var newPositionOffset = (float) (newSize / 1.9); // Instead of 2.0, to prevent unwanted collision
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
                        
                        explodedCircle = true;
                    }
                    break;
                }
            }

            if (explodedCircle) {
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
}