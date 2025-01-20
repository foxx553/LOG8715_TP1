using UnityEngine;

public class HandleCollision : ISystem{
    public string Name => "HandleCollision";
    private ComponentDatabase _componentDatabase;
    public HandleCollision(ComponentDatabase componentDatabase) {
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem(){
        foreach (var entry in _componentDatabase.positionComponent){
            uint id = entry.Key;
            if (_componentDatabase.collisionComponent[id].Id == null){
                foreach (var entry2 in _componentDatabase.positionComponent){
                    uint id2 = entry2.Key;
                    if (id != id2){
                        Vector2 position1 = entry.Value.Position;
                        Vector2 position2 = entry2.Value.Position;

                        float size1 = _componentDatabase.sizeComponent[id].Size;
                        float size2 = _componentDatabase.sizeComponent[id2].Size;

                        float distance = (position1 - position2).magnitude;
                        float radiusSum =  size1/2 +  size2/2;
                        if (distance <= radiusSum){
                            Vector2 velocity1 = _componentDatabase.velocityComponent[id].Velocity;
                            Vector2 velocity2 = _componentDatabase.velocityComponent[id2].Velocity;

                            _componentDatabase.UpdateCollisionComponent(id, id2);
                            _componentDatabase.UpdateCollisionComponent(id2, id);

                            CollisionResult collisionResult = CollisionUtility.CalculateCollision(position1, 
                            velocity1,size1,position2,velocity2,size2);

                            _componentDatabase.UpdatePositionComponent(id, collisionResult.position1);
                            _componentDatabase.UpdateVelocityComponent(id, collisionResult.velocity1);
                            _componentDatabase.UpdatePositionComponent(id2, collisionResult.position2);
                            _componentDatabase.UpdateVelocityComponent(id2, collisionResult.velocity2);
                        }
                    }
                }
            }
        }
    }
}