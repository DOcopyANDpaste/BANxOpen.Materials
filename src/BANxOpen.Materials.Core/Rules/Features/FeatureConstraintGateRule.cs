using BANxOpen.Materials.Contracts;
using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Foundation.Core.RuleEngine;

namespace BANxOpen.Materials.Core.Rules.Features;

/// <summary>Blocks an assignment that the features already on the target body forbid.
///
/// Ordered at <see cref="MaterialRuleOrder.Validation.FeatureConstraints"/>, after the body-type restriction and
/// before the reassignment confirmation: a material the body's features rule out should be rejected outright
/// rather than confirmed, so this has to run before anything that asks the user a question.
///
/// <see cref="MaterialRuleSet"/> adds exactly one of these, fed by every module's feature constraints. With no
/// providers registered it is a no-op that always allows, so it costs nothing until a feature domain opts in.</summary>
public sealed class FeatureConstraintGateRule : IMaterialValidationRule
{
    public const string Id = "FEATURE_MATERIAL_CONSTRAINT";

    private readonly IReadOnlyList<IFeatureMaterialConstraintProvider> _providers;
    private readonly MaterialsText _text;

    /// <param name="text">Words how a violation is attributed to its source and how warnings combine;
    /// <see cref="MaterialsText.Default"/> when null. The violation itself is worded by its provider.</param>
    public FeatureConstraintGateRule(IEnumerable<IFeatureMaterialConstraintProvider> providers, MaterialsText? text = null)
    {
        _providers = providers.ToList();
        _text = text ?? MaterialsText.Default;
    }

    public FeatureConstraintGateRule(params IFeatureMaterialConstraintProvider[] providers)
        : this((IEnumerable<IFeatureMaterialConstraintProvider>)providers) { }

    public string RuleId => Id;

    public int Order => MaterialRuleOrder.Validation.FeatureConstraints;

    /// <summary>The first unsatisfied Block constraint refuses the assignment, regardless of where it sits
    /// relative to warnings — a warning must never mask a refusal. With no refusal, unsatisfied Warn
    /// constraints produce a single Warn outcome carrying every warning message, so the user sees all of
    /// them at once rather than one per attempt. Each warning is also kept as its own outcome in
    /// <see cref="RuleOutcome.Inner"/>, with its own code.
    ///
    /// Every outcome from a constraint carries the constraint's <see cref="MaterialConstraint.ViolationArgs"/> plus
    /// <c>source</c> (its <see cref="MaterialConstraint.SourceLabel"/>) and <c>material</c>.</summary>
    public RuleOutcome Evaluate(MaterialAssignmentRuleContext context)
    {
        var material = context.RequestedMaterial;
        var warnings = new List<RuleOutcome>();

        foreach (var provider in _providers)
        {
            foreach (var constraint in provider.ConstraintsFor(context.TargetBody.Id))
            {
                if (constraint.IsSatisfiedBy(material))
                    continue;

                var decision = constraint.Severity == ConstraintSeverity.Block ? RuleDecision.Block : RuleDecision.Warn;
                var outcome = new RuleOutcome(
                    RuleId,
                    decision,
                    constraint.ReasonCode,
                    _text.RequiredBy(constraint.DescribeViolation(material), constraint.SourceLabel))
                {
                    Args = ArgsFor(constraint, material),
                };

                if (decision == RuleDecision.Block)
                    return outcome;

                warnings.Add(outcome);
            }
        }

        return warnings.Count == 0
            ? new RuleOutcome(RuleId, RuleDecision.Allow, null, null)
            : new RuleOutcome(RuleId, RuleDecision.Warn, warnings[0].ReasonCode, _text.CombineWarnings(warnings.Select(w => w.Message ?? string.Empty)))
            {
                Args = warnings[0].Args,
                Inner = warnings,
            };
    }

    private static IReadOnlyDictionary<string, string> ArgsFor(MaterialConstraint constraint, Material material)
    {
        var args = constraint.ViolationArgs is { } describe
            ? describe(material).ToDictionary(p => p.Key, p => p.Value)
            : new Dictionary<string, string>();
        args["source"] = constraint.SourceLabel;
        args["material"] = material.Name;
        return args;
    }
}