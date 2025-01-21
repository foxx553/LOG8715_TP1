using System.Collections.Generic; // generic collection of unity
using UnityEngine; // to have access to Vector2 type

[System.Serializable]
public class ComponentDatabase {

    public float startTime = 0;
    public readonly Dictionary<uint, PositionComponent> positionComponent = new();
    public readonly Dictionary<uint, VelocityComponent> velocityComponent = new();
    public readonly Dictionary<uint, SizeComponent> sizeComponent = new();

    private void UpdateComponent<T>(Dictionary<uint, T> componentDict, uint id, T newComponent){
        if (componentDict.ContainsKey(id)){
            componentDict[id] = newComponent;
        }
        else{
            componentDict.Add(id, newComponent);
        }
    }
    public void UpdatePositionComponent(uint id, Vector2 position){
        UpdateComponent(positionComponent, id, new PositionComponent { Position = position });
    }
    public void UpdateVelocityComponent(uint id, Vector2 velocity){
        UpdateComponent(velocityComponent, id, new VelocityComponent { Velocity = velocity });
    }
    public void UpdateSizeComponent(uint id, int size){
        UpdateComponent(sizeComponent, id, new SizeComponent { Size = size });        
    }
}