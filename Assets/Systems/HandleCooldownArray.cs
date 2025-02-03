using UnityEngine;
using System.Collections.Generic;

public class HandleCooldownArray : ISystem
{
    public string Name => "HandleCooldownArray";
    private ComponentDatabaseArray _componentDatabase;

    public HandleCooldownArray(ComponentDatabaseArray componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){

        var ecsController = ECSController.Instance;

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id++){
            if (_componentDatabase.positionComponents[id] == null) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f ////
            && ((_componentDatabase.frameCounter % 4) != 0)) continue;

            if (_componentDatabase.isProtecteds[id] != null){
                _componentDatabase.isProtecteds[id].DeltaTime -= _componentDatabase.deltaTime;
                if (_componentDatabase.isProtecteds[id].DeltaTime <= 0){
                    _componentDatabase.isProtecteds[id] = null;
                    _componentDatabase.UpdateCooldown(id, ecsController.Config.protectionCooldown);
                }
            }

            if (_componentDatabase.cooldownComponents[id] != null){
                _componentDatabase.cooldownComponents[id].DeltaTime -= _componentDatabase.deltaTime;
                if (_componentDatabase.cooldownComponents[id].DeltaTime <= 0){
                    _componentDatabase.cooldownComponents[id] = null;
                }
            }
        }
    }
}