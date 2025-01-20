using UnityEngine;

public class UpdatePosition : ISystem {
    public string Name => "UpdatePosition";
    private ComponentDatabase _componentDatabase;
    public UpdatePosition(ComponentDatabase componentDatabase){
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem() {
        var ecsController = ECSController.Instance;
        foreach (var entry in _componentDatabase.velocityComponent){
            if (entry.Value != null) {
                uint id = entry.Key;
                ecsController.UpdateShapePosition(id, _componentDatabase.positionComponent[id].Position);
            }
        }
    }
}