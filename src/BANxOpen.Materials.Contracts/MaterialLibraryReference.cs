
namespace BANxOpen.Materials.Contracts;

/// <summary>A library the user can pick from the dropdown, before its XML has been read/parsed.</summary>
public sealed record MaterialLibraryReference(
    MaterialLibraryId Id,
    string DisplayName,
    string FilePath);
