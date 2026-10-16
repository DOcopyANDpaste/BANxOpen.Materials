using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Materials.Core.Rules.Features;

namespace BANxOpen.Materials.Core.Rules.Standard;

/// <summary>Rules that apply to every material assignment regardless of material, body or feature.</summary>
public sealed class StandardRuleModule : IMaterialRuleModule
{
    public const string Id = "STANDARD";

    /// <param name="text">Words the rules' outcomes; <see cref="MaterialsText.Default"/> when null.</param>
    public StandardRuleModule(MaterialsText? text = null) =>
        ValidationRules = new IMaterialValidationRule[] { new RequireConfirmationOnReassignmentRule(text) };

    public string ModuleId => Id;

    public IReadOnlyList<IMaterialValidationRule> ValidationRules { get; }

    public IReadOnlyList<IFeatureMaterialConstraintProvider> FeatureConstraints => Array.Empty<IFeatureMaterialConstraintProvider>();

    public IReadOnlyList<IPostAssignmentEffectRule> SideEffectRules => Array.Empty<IPostAssignmentEffectRule>();
}
