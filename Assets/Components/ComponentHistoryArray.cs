using Unity;
using System.Collections.Generic;

public class ComponentHistoryArray {

    public Queue<ComponentDatabaseArray> saves = new();
    private const int MAX_SAVES = 3 * 120; // 3 seconds at 30 FPS

    public void addNewSave(ComponentDatabaseArray frameComponents) {
        if (this.saves.Count >= MAX_SAVES) {
            this.saves.Dequeue();
        }
        this.saves.Enqueue((ComponentDatabaseArray) frameComponents.Clone());
    }

    public ComponentDatabaseArray getThreeSecondsSave() {
        ComponentDatabaseArray res = this.saves.Dequeue();
        this.saves.Clear();
        return res;
    }

}