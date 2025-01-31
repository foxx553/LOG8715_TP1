using UnityEngine;
using System.Collections.Generic;

public class UpdateSizeArray : ISystem {
    public string Name => "UpdateSizeArray";
    private ComponentDatabaseArray _componentDatabase;
    public UpdateSizeArray(ComponentDatabaseArray componentDatabase){
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        var ecsController = ECSController.Instance;

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id ++){

            if (_componentDatabase.sizeComponents[id] == null) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f /////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            if (_componentDatabase.sizeComponents[id].Size <= 0){
                    ecsController.DestroyShape(id);
                    _componentDatabase.DestroyId(id);
                    _componentDatabase.availableIds.Add(id);
                }
            else ecsController.UpdateShapeSize(id, _componentDatabase.sizeComponents[id].Size);
        }
    }
}