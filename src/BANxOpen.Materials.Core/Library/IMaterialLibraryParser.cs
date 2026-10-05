using BANxOpen.Materials.Contracts;

namespace BANxOpen.Materials.Core.Library;

/// <summary>Parses material library XML content into domain objects. Pure text-in, data-out — no file
/// I/O, no NXOpen. The adapter layer reads the file and hands the content here.</summary>
public interface IMaterialLibraryParser
{
    BANxOpen.Materials.Contracts.MaterialLibrary Parse(MaterialLibraryId id, string displayName, string xmlContent);
}
