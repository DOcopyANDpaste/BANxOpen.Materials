using BANxOpen.Materials.Core.Assignment;
using BANxOpen.Materials.Core.Rules.SheetMetal;
using BANxOpen.Materials.Core.Bodies;
using BANxOpen.Materials.Contracts;
using BANxOpen.Foundation.Core.RuleEngine;
using static BANxOpen.Materials.Core.Tests.Assignment.TestFixtures;
using BANxOpen.Foundation.Contracts.Bodies;

namespace BANxOpen.Materials.Core.Tests.Rules.SheetMetal;

public class BlockRestrictedBodyTypeRuleTests
{
    private readonly BlockRestrictedBodyTypeRule _rule = new();

    private static Material MakeSheetMetalLibraryMaterial(string name = "Sheet Steel") =>
        MakeMaterial(name) with { LibraryId = new MaterialLibraryId("Sheet Metal Materials") };

    private RuleDecision Evaluate(Material material, BodyKind kind) =>
        _rule.Evaluate(new MaterialAssignmentRuleContext(
            material,
            MakeBody("body1", kind),
            CurrentAssignment: null,
            AllTargetBodiesInBatch: Array.Empty<BodyInfo>())).Decision;

    [Fact]
    public void Evaluate_AllowsSheetMetalLibraryMaterialOnSheetMetalBody()
    {
        Assert.Equal(RuleDecision.Allow, Evaluate(MakeSheetMetalLibraryMaterial(), BodyKind.SheetMetal));
    }

    [Fact]
    public void Evaluate_BlocksNonSheetMetalLibraryMaterialOnSheetMetalBody()
    {
        Assert.Equal(RuleDecision.Block, Evaluate(MakeMaterial("Steel"), BodyKind.SheetMetal));
    }

    [Fact]
    public void Evaluate_BlocksSheetMetalLibraryMaterialOnSolidBody()
    {
        Assert.Equal(RuleDecision.Block, Evaluate(MakeSheetMetalLibraryMaterial(), BodyKind.Solid));
    }

    [Fact]
    public void Evaluate_AllowsNonSheetMetalLibraryMaterialOnSolidBody()
    {
        Assert.Equal(RuleDecision.Allow, Evaluate(MakeMaterial("Steel"), BodyKind.Solid));
    }

    [Fact]
    public void Evaluate_BlocksSheetMetalLibraryMaterialOnSurfaceBody()
    {
        // A zero-thickness surface body is not sheet metal, whatever its NX name suggests.
        Assert.Equal(RuleDecision.Block, Evaluate(MakeSheetMetalLibraryMaterial(), BodyKind.Sheet));
    }

    [Fact]
    public void Evaluate_AllowsNonSheetMetalLibraryMaterialOnSurfaceBody()
    {
        Assert.Equal(RuleDecision.Allow, Evaluate(MakeMaterial("Steel"), BodyKind.Sheet));
    }
}
