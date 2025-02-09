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

            Color color = Color.blue;

            if (_componentDatabase.sizeComponents[id].Size >= ecsController.Config.explosionSize - 1){
                color = new Color(1.0f, 0.64f, 0.0f);
            }
            if (_componentDatabase.cooldownComponents[id] != null)
            {
                color = Color.yellow;
            }
            if (_componentDatabase.isProtectables[id] != null){
                if (_componentDatabase.isProtectables[id].ProtectionCount
                    == (ecsController.Config.protectionCollisionCount - 1)){
                        color = Color.cyan;
                    }
            }
            if (_componentDatabase.isProtecteds[id] != null)
            {
                color = Color.white;
            }
            if (_componentDatabase.isExplodeds[id] != null)
            {
                color = Color.magenta;
                _componentDatabase.isExplodeds[id] = null;
            }
            if (_componentDatabase.isCollidings[id] != null)
            {
                color = Color.green;
                _componentDatabase.isCollidings[id] = null;
            }
            if (_componentDatabase.isStatics[id] != null){
                color = Color.red;
            }
            ecsController.UpdateShapeColor(id, color);
        }
    }
}