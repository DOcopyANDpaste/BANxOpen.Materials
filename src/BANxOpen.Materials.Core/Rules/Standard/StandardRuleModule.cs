using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Materials.Core.Rules.Features;

namespace BANxOpen.Materials.Core.Rules.Standard;

/// <summary>Rules that apply to every material assignment regardless of material, body or feature.</summary>
public sealed class StandardRuleModule : IMaterialRuleModule
{
    public const string Id = "STANDARD";

    public string ModuleId => Id;

    public IReadOnlyList<IMaterialValidationRule> ValidationRules { get; } = new IMaterialValidationRule[]
    {
        new RequireConfirmationOnReassignmentRule(),
    };

    public IReadOnlyList<IFeatureMaterialConstraintProvider> FeatureConstraints => Array.Empty<IFeatureMaterialConstraintProvider>();

    public IReadOnlyList<IPostAssignmentEffectRule> SideEffectRules => Array.Empty<IPostAssignmentEffectRule>();
}
