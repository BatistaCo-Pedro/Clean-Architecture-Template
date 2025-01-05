namespace Application.Enums;

/// <summary>
/// Example of a smart enum.
/// </summary>
/// <param name="Name"></param>
/// <param name="Value"></param>
public record SmartEnum(string Name, int Value)
{
    public bool Validate() => !string.IsNullOrWhiteSpace(Name) && Value >= 0;

    private class SmartEnumUsage()
    {
        public static readonly SmartEnum Example = new("Example", 1);
        public static readonly SmartEnum Example2 = new("Example2", 1);
    }
}