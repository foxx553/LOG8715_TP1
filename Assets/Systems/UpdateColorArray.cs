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
                ecsController.UpdateShapeColor(id, Color.red);
            }
            else if (_componentDatabase.isExplodeds[id] != null)
            {
                ecsController.UpdateShapeColor(id, Color.magenta);
                _componentDatabase.isExplodeds[id] = null;
            }
            else if (_componentDatabase.isCollidings[id] != null)
            {
                ecsController.UpdateShapeColor(id, Color.green);
                _componentDatabase.isCollidings[id] = null;
            }
            else if (_componentDatabase.sizeComponents[id].Size >= ecsController.Config.explosionSize - 1)
            {
                ecsController.UpdateShapeColor(id, new Color(1.0f, 0.5f, 0.0f));
            }
            else if (_componentDatabase.isProtecteds[id] != null)
            {
                ecsController.UpdateShapeColor(id, Color.white);
            }
            else if (_componentDatabase.isProtectables[id] != null)
            {
                ecsController.UpdateShapeColor(id, Color.cyan);
            }
            else if (_componentDatabase.cooldownComponents[id] != null)
            {
                ecsController.UpdateShapeColor(id, Color.yellow);
            }
            else
            {
                ecsController.UpdateShapeColor(id, Color.blue);
            }

        }
    }
}