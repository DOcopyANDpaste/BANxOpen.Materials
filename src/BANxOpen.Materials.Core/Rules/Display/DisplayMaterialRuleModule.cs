using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Materials.Core.Rules.Features;

namespace BANxOpen.Materials.Core.Rules.Display;

/// <summary>Display (coating/studio) material: validates the library's coating data against the body, then syncs
/// the display material and body color. The executor for <see cref="SyncCoatingDisplayMaterialEffectRule"/> is
/// <c>DisplayMaterialHelper</c> in BANxOpen.Foundation.NxAdapters, registered by <c>MaterialEngine</c>.</summary>
public sealed class DisplayMaterialRuleModule : IMaterialRuleModule
{
    public const string Id = "DISPLAY";

    /// <param name="text">Words the rules' outcomes; <see cref="MaterialsText.Default"/> when null.</param>
    public DisplayMaterialRuleModule(MaterialsText? text = null) =>
        ValidationRules = new IMaterialValidationRule[] { new ValidateCoatingDisplayMaterialRule(text) };

    public string ModuleId => Id;

    public IReadOnlyList<IMaterialValidationRule> ValidationRules { get; }

    public IReadOnlyList<IFeatureMaterialConstraintProvider> FeatureConstraints => Array.Empty<IFeatureMaterialConstraintProvider>();

    public IReadOnlyList<IPostAssignmentEffectRule> SideEffectRules { get; } = new IPostAssignmentEffectRule[]
    {
        new SyncCoatingDisplayMaterialEffectRule(),
    };
}
