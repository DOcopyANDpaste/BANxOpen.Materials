using System.Reflection;
using BANxOpen.Foundation.Contracts.Bodies;
using BANxOpen.Foundation.Contracts.Common;
using BANxOpen.Foundation.Core.RuleEngine;
using BANxOpen.Materials.Contracts;
using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Materials.Core.Rules;
using BANxOpen.Materials.Core.Rules.Features;
using BANxOpen.Materials.Core.Rules.Standard;
using BANxOpen.Materials.Core.Tests.Assignment;
using static BANxOpen.Materials.Core.Tests.Rules.Features.ConstraintFixtures;

namespace BANxOpen.Materials.Core.Tests;

/// <summary>The describer is the single source of the engine's wording: its defaults are pinned here, and an
/// overriding describer handed to a rule, the feature gate or the rule set is what ends up in the outcome.</summary>
public class MaterialsTextTests
{
    private sealed class CustomText : MaterialsText
    {
        public override string Reassign(string current, string requested) => $"custom reassign {current}->{requested}";
        public override string RequiredBy(string violation, string source) => $"custom [{source}] {violation}";
    }

    private static MaterialAssignmentRuleContext Reassigning(string from, string to)
    {
        var body = TestFixtures.MakeBody("body-1");
        return new MaterialAssignmentRuleContext(
            TestFixtures.MakeMaterial(to), body, new BodyMaterialAssignment(body.Id, from, null, null), new[] { body });
    }

    [Fact]
    public void Default_wording_is_pinned()
    {
        var text = MaterialsText.Default;

        Assert.Equal("Body already has 'A'. Replace with 'B'?", text.Reassign("A", "B"));
        Assert.Equal("'X' is not available for this body.", text.BodyTypeRestricted("X"));
        Assert.Equal("V (required by S)", text.RequiredBy("V", "S"));
        Assert.Equal(
            "Library: 'L' (1,2,3) vs current: 'C' (4,5,6).\nConfirm to update to the library value, or leave unconfirmed to keep the current one.",
            text.CoatingDisplayMismatch("L", "1,2,3", "C", "4,5,6"));
    }

    [Fact]
    public void A_rule_words_its_outcome_with_the_describer_it_is_given()
    {
        var outcome = new RequireConfirmationOnReassignmentRule(new CustomText()).Evaluate(Reassigning("Old", "New"));

        Assert.Equal(MaterialsCodes.Reassign, outcome.ReasonCode);
        Assert.Equal("custom reassign Old->New", outcome.Message);
    }

    [Fact]
    public void The_feature_gate_attributes_a_violation_with_the_describer_it_is_given()
    {
        var gate = new FeatureConstraintGateRule(
            new IFeatureMaterialConstraintProvider[] { new FakeConstraintProvider("BEAD", AllowOnly("SPEC BA-1", "NOT_ALLOWED", "2024-O")) },
            new CustomText());

        var outcome = gate.Evaluate(Reassigning("Old", "7075-T6"));

        Assert.Equal("custom [SPEC BA-1] '7075-T6' is not allowed.", outcome.Message);
    }

    [Fact]
    public void The_rule_set_hands_its_describer_to_every_baseline_rule()
    {
        var text = new CustomText();
        var planner = MaterialRuleSet.From(MaterialRuleSet.Baseline(null, text), text).CreatePlanner();
        var body = TestFixtures.MakeBody("body-1");

        var plan = planner.Plan(new MaterialAssignmentPlanningInput(
            TestFixtures.MakeMaterial("New"),
            new[] { body },
            new Dictionary<BodyId, BodyMaterialAssignment> { [body.Id] = new(body.Id, "Old", null, null) }));

        var confirmation = Assert.Single(plan.BodyEvaluations.Single().ConfirmationOutcomes);
        Assert.Equal("custom reassign Old->New", confirmation.Message);
    }

    [Fact]
    public void Codes_are_distinct()
    {
        var values = typeof(MaterialsCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => (string)f.GetValue(null)!)
            .ToList();

        Assert.Equal(values.Count, values.Distinct(StringComparer.Ordinal).Count());
    }
}
