using System.Collections.Generic; // generic collection of unity
using UnityEngine; // to have access to Vector2 type

[System.Serializable]
public class ComponentDatabase {

    public float startTime = new();
    public readonly Dictionary<uint, PositionComponent> positionComponent = new();

    public readonly Dictionary<uint, VelocityComponent> velocityComponent = new();

    public readonly Dictionary<uint, SizeComponent> sizeComponent = new();

    public void UpdateTime(float time){
        startTime = time;
    }

    public void UpdatePositionComponent(uint id, Vector2 position){
        if (positionComponent.ContainsKey(id)){
            positionComponent[id].Position = position;
        }
        else {
            positionComponent.Add(id, new PositionComponent { Position = position });
        }
    }

    public void UpdateVelocityComponent(uint id, Vector2 velocity){
        if (velocityComponent.ContainsKey(id)){
            velocityComponent[id].Velocity = velocity;
        }
        else {
            velocityComponent.Add(id, new VelocityComponent { Velocity = velocity });
        }
    }

    public void UpdateSizeComponent(uint id, int size){
        if (sizeComponent.ContainsKey(id)){
            sizeComponent[id].Size = size;
        }
        else {
            sizeComponent.Add(id, new SizeComponent { Size = size });
        }
    }

}