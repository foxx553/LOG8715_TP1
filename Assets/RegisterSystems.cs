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

        var updatePosition = new UpdatePosition(componentDatabase);
        toRegister.Add(updatePosition);

        return toRegister;
    }
}