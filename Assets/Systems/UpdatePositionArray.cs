using UnityEngine;

public class UpdatePositionArray : ISystem {
    public string Name => "UpdatePositionArray";
    private ComponentDatabaseArray _componentDatabase;
    public UpdatePositionArray(ComponentDatabaseArray componentDatabase){
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        var ecsController = ECSController.Instance;

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id++){

            if (_componentDatabase.positionComponents[id] == null) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f /////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            ecsController.UpdateShapePosition(id, _componentDatabase.positionComponents[id].Position);
        }
    }
}