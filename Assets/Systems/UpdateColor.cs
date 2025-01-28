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
            if (_componentDatabase.isCollidingComponent.ContainsKey(id))
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.green);
                _componentDatabase.UpdateIsCollidiingComponent(id, false);
            }
            else
            {
                ecsController.UpdateShapeColor(id, UnityEngine.Color.white);
            }
        }

        // for (uint i = 0; i < _componentDatabase.entitiesCounter; i++)
        // {
        //     uint id = (uint)i;
        //     if (_componentDatabase.isCollidingComponent.ContainsKey(i))
        //     {
        //         ecsController.UpdateShapeColor(id, UnityEngine.Color.green);
        //         _componentDatabase.UpdateIsCollidiingComponent(id, false);
        //     }
        //     else
        //     {
        //         ecsController.UpdateShapeColor(id, UnityEngine.Color.white);
        //     }
        // }
    }
}