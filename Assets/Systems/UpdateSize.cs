using UnityEngine;
using System.Collections.Generic;

public class UpdateSize : ISystem {
    public string Name => "UpdateSize";
    private ComponentDatabase _componentDatabase;
    public UpdateSize(ComponentDatabase componentDatabase){
        _componentDatabase = componentDatabase;
    }
    public void UpdateSystem() {
        var ecsController = ECSController.Instance;
        List<uint> idsToRemove = new List<uint>();
        foreach (var entry in _componentDatabase.sizeComponent){
            if (entry.Value != null) {
                uint id = entry.Key;
                
                if (_componentDatabase.positionComponent[id].Position.x > 0f /////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

                ecsController.UpdateShapeSize(id, _componentDatabase.sizeComponent[id].Size);
                if (_componentDatabase.sizeComponent[id].Size <= 0){
                    idsToRemove.Add(id);
                }
            }
        }
        foreach (var id in idsToRemove){
            ecsController.DestroyShape(id);
            _componentDatabase.DestroyId(id);
        }
    }
}