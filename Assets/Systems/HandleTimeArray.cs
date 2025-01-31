using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

public class HandleTimeArray : ISystem {
    public string Name => "HandleTimeArray";
    private ComponentDatabaseArray _componentDatabase;

    public HandleTimeArray (ComponentDatabaseArray componentDatabase) {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){ // À compléter pour ajouter les cooldowns
        _componentDatabase.frameCounter += 1;
        _componentDatabase.totalTime += Time.deltaTime;
        Debug.Log(Time.deltaTime);
    }
}