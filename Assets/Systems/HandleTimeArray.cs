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

    public void UpdateSystem(){ // ?????????????????????? Consider it fixed please
        _componentDatabase.frameCounter += 1;
        
        long currentTime = (((DateTime.UtcNow.Hour * 60 
                            + DateTime.UtcNow.Minute) * 60 
                            + DateTime.UtcNow.Second) * 1000  
                            + DateTime.UtcNow.Millisecond);
        int timeToWait = (int) (1000f*_componentDatabase.deltaTime/4 - currentTime + _componentDatabase.startTime);
        if (timeToWait > 0){
            Thread.Sleep(timeToWait);
        }
        _componentDatabase.startTime = (((DateTime.UtcNow.Hour * 60 
                                        + DateTime.UtcNow.Minute) * 60 
                                        + DateTime.UtcNow.Second) * 1000  
                                        + DateTime.UtcNow.Millisecond);
    }
}