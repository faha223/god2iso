namespace God2IsoCli;

public class Arguments
{
    public required List<string> Packages { get; init; }
    public required string OutputDirectory { get; init; }
    public required bool CreateGoodIsoHeader { get; init; }
    public required bool Silent { get; init; }
}