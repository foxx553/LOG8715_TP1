using UnityEngine;

public class UpdateColor : ISystem
{
    public string Name => "UpdateColor";
    private ComponentDatabase _componentDatabase;
    public UpdateColor(ComponentDatabase componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem()
    {
        var ecsController = ECSController.Instance;
        foreach (uint id in _componentDatabase.positionComponent.Keys)
        {
            if (_componentDatabase.positionComponent[id].Position.x > 0f //////
                && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            if (_componentDatabase.isStatic.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.red);
            }
            else if (_componentDatabase.isColliding.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.green);
                _componentDatabase.UpdateIsCollidiing(id, false);
            }
            else if (_componentDatabase.isProtected.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.white);
            }
            else if (_componentDatabase.isProtectable.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.cyan);
            }
            // else if (_componentDatabase.cooldownComponent)
            else
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.blue);
            }
        }
    }
}