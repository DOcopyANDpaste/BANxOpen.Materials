
using BANxOpen.Materials.Contracts;

namespace BANxOpen.Materials.Core.Library;

/// <summary>Reads (via <see cref="IMaterialLibraryRepository"/>, the adapter-implemented seam) and parses
/// a library only the first time it's requested; every subsequent request for the same
/// <see cref="MaterialLibraryId"/> returns the cached result without touching the repository or parser
/// again. Not thread-safe — callers are expected to be single-threaded/modal, so a plain dictionary is
/// enough; revisit if that assumption ever changes.</summary>
public sealed class CachingMaterialLibraryLoader
{
    private readonly IMaterialLibraryRepository _repository;
    private readonly IMaterialLibraryParser _parser;
    private readonly Dictionary<MaterialLibraryId, BANxOpen.Materials.Contracts.MaterialLibrary> _cache = new();

    public CachingMaterialLibraryLoader(IMaterialLibraryRepository repository, IMaterialLibraryParser parser)
    {
        _repository = repository;
        _parser = parser;
    }

    public BANxOpen.Materials.Contracts.MaterialLibrary GetOrLoad(MaterialLibraryReference reference)
    {
        if (_cache.TryGetValue(reference.Id, out var cached))
            return cached;

        var xmlContent = _repository.ReadLibraryXml(reference.Id);
        var parsed = _parser.Parse(reference.Id, reference.DisplayName, xmlContent);

        var bmpDirectory = Path.Combine(Path.GetDirectoryName(reference.FilePath) ?? string.Empty, "BMPs");
        var materials = parsed.Materials
            .Select(material => material with { ImagePath = ResolveImagePath(bmpDirectory, material.Name) })
            .ToList();

        var library = parsed with { FilePath = reference.FilePath, Materials = materials };
        _cache[reference.Id] = library;
        return library;
    }

    private static string ResolveImagePath(string bmpDirectory, string materialName)
    {
        var candidate = Path.Combine(bmpDirectory, $"{materialName}.bmp");
        return File.Exists(candidate) ? candidate : Path.Combine(bmpDirectory, "default.bmp");
    }
}
