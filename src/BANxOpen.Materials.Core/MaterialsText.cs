namespace BANxOpen.Materials.Core;

/// <summary>The wording of every message the material engine can put in front of a user — rule outcomes, the failures
/// of the part material service, and configuration errors shown at start-up — as default text a tool may override (the
/// ASP.NET Identity <c>IdentityErrorDescriber</c> pattern): one virtual member per message, and a <see cref="Default"/>
/// instance. Rules and services take a <c>MaterialsText?</c> and fall back to <see cref="Default"/>; the codes that go
/// with these messages are <see cref="MaterialsCodes"/>. Log lines are not user text and are not here. See the
/// nxopen-csharp-patterns skill, common.md §8.
///
/// Parameters that carry an NX error are already formatted (<c>NxDialogText.NxError</c>): Core has no NXOpen.</summary>
public class MaterialsText
{
    public static MaterialsText Default { get; } = new();

    // ---- Rule outcomes ----

    public virtual string Reassign(string current, string requested) =>
        $"Body already has '{current}'. Replace with '{requested}'?";

    public virtual string CoatingNameMissing(string material, string property) =>
        $"Material '{material}' has no {property} defined — a default display material will be applied instead.";

    public virtual string CoatingColorInvalid(string material, string property) =>
        $"Material '{material}' has an invalid or missing {property}. Contact your material library administrator.";

    public virtual string CoatingNotApplied(string? current) =>
        $"Body already has material '{current}' assigned but no display material is currently associated with it — it will be assigned automatically.";

    public virtual string CoatingDisplayMismatch(string libraryName, string libraryRgb, string currentName, string currentRgb) =>
        $"Library: '{libraryName}' ({libraryRgb}) vs current: '{currentName}' ({currentRgb}).\nConfirm to update to the library value, or leave unconfirmed to keep the current one.";

    public virtual string BodyTypeRestricted(string material) => $"'{material}' is not available for this body.";

    /// <summary>A feature constraint's violation, naming what on the body or part imposed it.</summary>
    public virtual string RequiredBy(string violation, string source) => $"{violation} (required by {source})";

    /// <summary>Several advisory warnings on one body, shown together.</summary>
    public virtual string CombineWarnings(IEnumerable<string> warnings) => string.Join(" ", warnings);

    // ---- Material library rules file (shown at start-up) ----

    public virtual string LibraryRulesListEmpty =>
        "sheetMetalLibraries must list at least one library; delete the file to use the name-based default instead.";

    public virtual string LibraryRulesInvalidJson(string path, string detail) =>
        $"Material library rules at '{path}' are not valid JSON: {detail}";

    public virtual string LibraryRulesMissingList(string path) =>
        $"Material library rules at '{path}' have no \"sheetMetalLibraries\" list. " +
        "Add one, or delete the file to use the name-based default.";

    public virtual string LibraryRulesInvalid(string path, string detail) => $"Material library rules at '{path}': {detail}";

    public virtual string MaterialRulesUnavailable(string? detail) => WithDetails("Material rules could not be loaded.", detail);

    // ---- Part material service ----

    public virtual string ConfirmLoadPhysicalMaterial(string material, string library) =>
        $"'{material}' is not in this part yet.{Environment.NewLine}" +
        $"Load it from the NX material library '{library}'?{Environment.NewLine}{Environment.NewLine}" +
        "This can take a while for a large library.";

    public virtual string ApplyAborted(string? detail) =>
        WithDetails("The material could not be applied; nothing was changed.", detail);

    public virtual string ClearAborted(string? detail) =>
        WithDetails("The material could not be removed; nothing was changed.", detail);

    /// <summary>The detail of an abort when none of the target bodies could be found in the part.</summary>
    public virtual string NoTargetBodiesResolved => "No target bodies could be resolved.";

    public virtual string PartialFailure =>
        "Some bodies failed. The ones that succeeded were kept; see the Listing Window for details.";

    // ---- Shared ----

    /// <summary>A worded sentence with the technical detail that caused it below, or the sentence alone.</summary>
    protected virtual string WithDetails(string text, string? detail) =>
        string.IsNullOrWhiteSpace(detail) ? text : $"{text}{Environment.NewLine}{Environment.NewLine}Details: {detail}";
}
