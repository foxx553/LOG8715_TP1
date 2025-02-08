using UnityEngine;

public class HandleSave : ISystem {
    public string Name => "HandleSave";
    private ComponentDatabaseArray _componentDatabase;
    private ComponentHistoryArray _componentHistory;
    private int CooldownCounter = 1; // Starting to 1 to prevent immediate "Space"
    private const int COOLDOWN_THRESHOLD = 360;

    public HandleSave (ComponentDatabaseArray componentDatabase, ComponentHistoryArray componentHistory) {
        _componentDatabase = componentDatabase;
        _componentHistory = componentHistory;
    }

    public void UpdateSystem(){

        bool saveRequested = false;
        if (Input.GetKeyDown(KeyCode.Space) && CooldownCounter == 0) {
            saveRequested = true;
            CooldownCounter++;
        } else if (CooldownCounter > 0 && CooldownCounter < COOLDOWN_THRESHOLD) {
            CooldownCounter++;
            if (Input.GetKeyDown(KeyCode.Space))
                Debug.Log("Rewind permission denied! Cooldown not over");
        } else if (CooldownCounter >= COOLDOWN_THRESHOLD) {
            CooldownCounter = 0;
        }

        if (saveRequested) {

            var ecsController = ECSController.Instance;

            for (uint i = 0; i < _componentDatabase.entitiesCounter; i++) {
                if (_componentDatabase.positionComponents[i] != null)
                    ecsController.DestroyShape(i);
            }
            this._componentDatabase.ApplySave(this._componentHistory.getThreeSecondsSave());
            for (uint i = 0; i < _componentDatabase.entitiesCounter; i++) {
                if (_componentDatabase.positionComponents[i] != null)
                    ecsController.CreateShape(i, _componentDatabase.sizeComponents[i].Size);
            }
        } else {
            this._componentHistory.addNewSave((ComponentDatabaseArray) _componentDatabase.Clone());
        }
    }
}