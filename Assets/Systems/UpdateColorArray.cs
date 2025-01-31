using UnityEngine;

public class UpdateColorArray : ISystem
{
    public string Name => "UpdateColorArray";
    private ComponentDatabaseArray _componentDatabase;
    public UpdateColorArray(ComponentDatabaseArray componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem()
    {
        var ecsController = ECSController.Instance;
        for (uint id = 0; id < _componentDatabase.entitiesCounter; id++){
            if (_componentDatabase.positionComponents[id] == null) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f //////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            if (_componentDatabase.isStatics[id] != null){
                ecsController.UpdateShapeColor(id, UnityEngine.Color.red);
            }
            else if (_componentDatabase.isCollidings[id] != null)
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.green);
                _componentDatabase.isCollidings[id] = null;
            }
            else if (_componentDatabase.isProtecteds[id] != null)
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.white);
            }
            else if (_componentDatabase.isProtectables[id] != null)
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.cyan);
            }
            else
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.blue);
            }

        }
    }
}