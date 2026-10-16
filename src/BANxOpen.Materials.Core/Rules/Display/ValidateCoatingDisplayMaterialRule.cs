using BANxOpen.Materials.Contracts;
using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Foundation.Core.RuleEngine;

namespace BANxOpen.Materials.Core.Rules.Display;

/// <summary>Validates the coating/studio display-material data on the requested material, and, when the
/// target body already has a material assigned, cross-checks that against the body's actual current
/// display-material state:
/// <list type="bullet">
/// <item>Name present, color present and valid, and (if the body already has a material assigned) no
/// conflict with the body's current display material → Allow.</item>
/// <item>Name present, color missing or malformed → Block (bad library data; not something the end user
/// can fix, so the message directs them to the material library administrator).</item>
/// <item>Name missing, regardless of color → Warn (non-blocking; the effect rule falls back to the shared
/// default display material for this assignment instead of the library's own coating data). This applies
/// even when neither property is present at all — deliberately not special-cased.</item>
/// <item>Body already has a material assigned but no display material currently associated with it →
/// Allow (this is the expected first-sync case, not a problem to flag; the sync effect rule will apply
/// the display material automatically). A reason code and message are still attached for reference/
/// logging even though the decision itself doesn't require the user's attention.</item>
/// <item>Body already has a material assigned, and its current display material's name and/or color
/// differs from what the requested material's library data specifies → RequireConfirmation, so the user
/// can choose to keep the body's current display material (decline) or update it to match the library
/// (confirm) — per direct confirmation with the user, this is a decision, not just information.</item>
/// </list></summary>
public sealed class ValidateCoatingDisplayMaterialRule : IMaterialValidationRule
{
    private readonly MaterialsText _text;

    /// <param name="text">Words the outcomes; <see cref="MaterialsText.Default"/> when null.</param>
    public ValidateCoatingDisplayMaterialRule(MaterialsText? text = null) => _text = text ?? MaterialsText.Default;

    public string RuleId => "VALIDATE_COATING_DISPLAY_MATERIAL";

    public int Order => MaterialRuleOrder.Validation.Appearance;

    public RuleOutcome Evaluate(MaterialAssignmentRuleContext context)
    {
        var material = context.RequestedMaterial;
        var displayMaterialName = CoatingPropertyReader.GetDisplayMaterialName(material);

        if (displayMaterialName is null)
        {
            return new RuleOutcome(
                RuleId,
                RuleDecision.Warn,
                MaterialsCodes.CoatingNameMissing,
                _text.CoatingNameMissing(material.Name, CoatingPropertyReader.MaterialNamePropertyName))
            {
                Args = new Dictionary<string, string>
                {
                    ["material"] = material.Name,
                    ["property"] = CoatingPropertyReader.MaterialNamePropertyName,
                },
            };
        }

        if (!CoatingPropertyReader.TryGetRgb(material, out var r, out var g, out var b))
        {
            return new RuleOutcome(
                RuleId,
                RuleDecision.Block,
                MaterialsCodes.CoatingColorInvalid,
                _text.CoatingColorInvalid(material.Name, CoatingPropertyReader.ColorPropertyName))
            {
                Args = new Dictionary<string, string>
                {
                    ["material"] = material.Name,
                    ["property"] = CoatingPropertyReader.ColorPropertyName,
                },
            };
        }

        if (context.CurrentAssignment is not null)
        {
            var currentDisplayMaterial = context.CurrentAssignment.CurrentDisplayMaterial;

            if (currentDisplayMaterial is null)
            {
                return new RuleOutcome(
                    RuleId,
                    RuleDecision.Allow,
                    MaterialsCodes.CoatingNotApplied,
                    _text.CoatingNotApplied(context.CurrentAssignment.MaterialName))
                {
                    Args = new Dictionary<string, string> { ["current"] = context.CurrentAssignment.MaterialName ?? string.Empty },
                };
            }

            var nameMatches = string.Equals(displayMaterialName, currentDisplayMaterial.Name, StringComparison.OrdinalIgnoreCase);
            var colorMatches = CoatingPropertyReader.ColorsApproximatelyEqual(r, g, b, currentDisplayMaterial.Rgb);

            if (!nameMatches || !colorMatches)
            {
                var libraryRgb = $"{Math.Round(r * 255)},{Math.Round(g * 255)},{Math.Round(b * 255)}";
                var currentRgb = $"{currentDisplayMaterial.Rgb.R},{currentDisplayMaterial.Rgb.G},{currentDisplayMaterial.Rgb.B}";

                return new RuleOutcome(
                    RuleId,
                    RuleDecision.RequireConfirmation,
                    MaterialsCodes.CoatingDisplayMismatch,
                    _text.CoatingDisplayMismatch(displayMaterialName, libraryRgb, currentDisplayMaterial.Name, currentRgb))
                {
                    Args = new Dictionary<string, string>
                    {
                        ["libraryName"] = displayMaterialName,
                        ["libraryRgb"] = libraryRgb,
                        ["currentName"] = currentDisplayMaterial.Name,
                        ["currentRgb"] = currentRgb,
                    },
                };
            }
        }

        return new RuleOutcome(RuleId, RuleDecision.Allow, null, null);
    }
}
