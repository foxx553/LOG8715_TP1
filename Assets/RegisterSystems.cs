﻿using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        var componentDatabase = new ComponentDatabaseArray();
        var componentHistory = new ComponentHistoryArray();
        // Add your systems here
        var initialization = new InitializationArray(componentDatabase);
        var calculatePosition = new CalculatePositionArray(componentDatabase);
        var updatePosition = new UpdatePositionArray(componentDatabase);
        var handleCollision = new HandleCollisionArray(componentDatabase);
        var updateSize = new UpdateSizeArray(componentDatabase);
        var handleExplosion = new HandleExplosionArray(componentDatabase);
        var handleMouseClick = new HandleMouseClick(componentDatabase);
        var updateColor = new UpdateColorArray(componentDatabase);
        var handleProtection = new HandleProtectionArray(componentDatabase);
        var handleCooldown = new HandleCooldownArray(componentDatabase);
        var handleTime = new HandleTimeArray(componentDatabase);
        var handleSave = new HandleSave(componentDatabase, componentHistory);

        toRegister.Add(initialization);
        toRegister.Add(calculatePosition);
        toRegister.Add(handleCollision);
        toRegister.Add(handleExplosion);
        toRegister.Add(handleMouseClick);
        toRegister.Add(handleProtection);
        toRegister.Add(handleCooldown);
        toRegister.Add(handleSave);

        toRegister.Add(updateSize);
        toRegister.Add(updatePosition);
        toRegister.Add(updateColor);
        
        toRegister.Add(handleTime);
        return toRegister;
    }
}