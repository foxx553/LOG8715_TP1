using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System;

public class HandleTime : ISystem {
    public string Name => "HandleTime ";
    private ComponentDatabase _componentDatabase;

    public HandleTime (ComponentDatabase componentDatabase) {
        _componentDatabase = componentDatabase;
    }

    public void UpdateSystem(){
        _componentDatabase.frameCounter += 1;
        _componentDatabase.totalTime += Time.deltaTime;
    }
}