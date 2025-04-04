public class HandleProtectionArray : ISystem
{
    public string Name => "HandleProtectionArray";
    private ComponentDatabaseArray _componentDatabase;

    public HandleProtectionArray(ComponentDatabaseArray componentDatabase)
    {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){

        var ecsController = ECSController.Instance;

        for (uint id = 0; id < _componentDatabase.entitiesCounter; id++){
            if (_componentDatabase.positionComponents[id] == null) continue;

            if (_componentDatabase.positionComponents[id].Position.x > 0f ////
            && ((_componentDatabase.frameCounter % 4) != 0)) continue;
            
            if (_componentDatabase.cooldownComponents[id] != null
                || (_componentDatabase.sizeComponents[id].Size > ecsController.Config.protectionSize)) {
                _componentDatabase.isProtectables[id] = null;
            } 
            else if (_componentDatabase.isProtectables[id] == null 
                    && _componentDatabase.sizeComponents[id].Size <= ecsController.Config.protectionSize) {
                _componentDatabase.UpdateIsProtectable(id,0);
            }
            else if (_componentDatabase.isProtectables[id].ProtectionCount >= ecsController.Config.protectionCollisionCount) {
                _componentDatabase.isProtectables[id] = null;
                _componentDatabase.UpdateIsProtected(id, ecsController.Config.protectionDuration);
            }
        }
    }
}