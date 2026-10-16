using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Foundation.Core.RuleEngine;

namespace BANxOpen.Materials.Core.Rules.Standard;

/// <summary>If the body already has a different material assigned, require the user to confirm the
/// overwrite before it proceeds.</summary>
public sealed class RequireConfirmationOnReassignmentRule : IMaterialValidationRule
{
    private readonly MaterialsText _text;

    /// <param name="text">Words the outcome; <see cref="MaterialsText.Default"/> when null.</param>
    public RequireConfirmationOnReassignmentRule(MaterialsText? text = null) => _text = text ?? MaterialsText.Default;

    public string RuleId => "CONFIRM_REASSIGNMENT";

    public int Order => MaterialRuleOrder.Validation.Confirmation;

    public RuleOutcome Evaluate(MaterialAssignmentRuleContext context)
    {
        var currentMaterialName = context.CurrentAssignment?.MaterialName;
        var isChangingMaterial = !string.IsNullOrEmpty(currentMaterialName)
            && !string.Equals(currentMaterialName, context.RequestedMaterial.Name, StringComparison.OrdinalIgnoreCase);

        return isChangingMaterial
            ? new RuleOutcome(
                RuleId,
                RuleDecision.RequireConfirmation,
                MaterialsCodes.Reassign,
                _text.Reassign(currentMaterialName!, context.RequestedMaterial.Name))
            {
                Args = new Dictionary<string, string>
                {
                    ["current"] = currentMaterialName!,
                    ["requested"] = context.RequestedMaterial.Name,
                },
            }
            : new RuleOutcome(RuleId, RuleDecision.Allow, null, null);
    }
}
