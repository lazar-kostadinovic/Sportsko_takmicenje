using System;

public class IdGenerator
{
    public static string GenerateUniqueId()
    {
        return Guid.NewGuid().ToString();
    }
}
