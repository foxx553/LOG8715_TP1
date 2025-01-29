using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        // Add your systems here
        var initialization = new Initialization();
        toRegister.Add(initialization);

        var componentDatabase = initialization.componentDatabase;
        var calculatePosition = new CalculatePosition(componentDatabase);
        var updatePosition = new UpdatePosition(componentDatabase);
        var handleCollision = new HandleCollision(componentDatabase);
        var updateSize = new UpdateSize(componentDatabase);
        var handleExplosion = new HandleExplosion(componentDatabase);
        var updateColor = new UpdateColor(componentDatabase);
        var handleTime = new HandleTime(componentDatabase);

        toRegister.Add(calculatePosition);
        toRegister.Add(handleCollision);
        toRegister.Add(handleExplosion);
        toRegister.Add(updateSize);
        toRegister.Add(updatePosition);
        toRegister.Add(updateColor);
        toRegister.Add(handleTime);
        return toRegister;
    }
}