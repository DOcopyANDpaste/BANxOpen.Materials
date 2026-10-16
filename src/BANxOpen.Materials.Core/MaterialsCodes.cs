namespace BANxOpen.Materials.Core;

/// <summary>The stable codes the material engine puts on its rule outcomes (<c>RuleOutcome.ReasonCode</c>) and failed
/// operations (<c>OperationResult.ErrorCode</c>). Codes, not wording, are the contract: tools branch on these and show
/// the message as-is. The wording is <see cref="MaterialsText"/>'s.</summary>
public static class MaterialsCodes
{
    // ---- Rule outcomes ----

    /// <summary>Args: current, requested.</summary>
    public const string Reassign = "REASSIGN";

    /// <summary>Args: material, property.</summary>
    public const string CoatingNameMissing = "COATING_NAME_MISSING";

    /// <summary>Args: material, property.</summary>
    public const string CoatingColorInvalid = "COATING_COLOR_INVALID";

    /// <summary>Args: current.</summary>
    public const string CoatingNotApplied = "COATING_NOT_APPLIED";

    /// <summary>Args: libraryName, libraryRgb, currentName, currentRgb.</summary>
    public const string CoatingDisplayMismatch = "COATING_DISPLAY_MISMATCH";

    /// <summary>Args: material.</summary>
    public const string BodyTypeRestricted = "BODY_TYPE_RESTRICTED";

    // ---- Failures ----

    public const string MaterialRulesUnavailable = "MATERIAL_RULES_UNAVAILABLE";

    public const string ApplyAborted = "APPLY_ABORTED";

    public const string ClearAborted = "CLEAR_ABORTED";

    /// <summary>Some bodies changed and some did not; the ones that did are kept. NX does not say which.</summary>
    public const string PartialFailure = "PARTIAL_FAILURE";

    /// <summary>A side-effect executor was handed an instruction type it does not carry out. Shared by every executor,
    /// since <c>ISideEffectExecutor</c> is the engine's.</summary>
    public const string UnsupportedInstruction = "UNSUPPORTED_INSTRUCTION";

    /// <summary>A side-effect instruction is missing a value its executor needs.</summary>
    public const string MalformedInstruction = "MALFORMED_INSTRUCTION";
}
