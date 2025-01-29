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
            if (_componentDatabase.isStaticComponent.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.red);
            }
            else if (_componentDatabase.isCollidingComponent.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.green);
                _componentDatabase.UpdateIsCollidiingComponent(id, false);
            }
            else if (_componentDatabase.isProtected.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.white);
            }
            else
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.blue);
            }
        }
    }
}