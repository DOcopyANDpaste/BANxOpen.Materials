
namespace BANxOpen.Materials.Contracts;

public sealed record MaterialLibrary(
    MaterialLibraryId Id,
    string DisplayName,
    IReadOnlyList<Material> Materials,
    string FilePath = "");
