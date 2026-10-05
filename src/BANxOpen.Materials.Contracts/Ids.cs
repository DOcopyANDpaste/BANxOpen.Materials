namespace BANxOpen.Materials.Contracts;

public readonly record struct MaterialId(string Value)
{
    public override string ToString() => Value;
}

public readonly record struct MaterialLibraryId(string Value)
{
    public override string ToString() => Value;
}
