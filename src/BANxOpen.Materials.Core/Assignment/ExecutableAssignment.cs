using BANxOpen.Materials.Contracts;
using BANxOpen.Foundation.Contracts.Common;

namespace BANxOpen.Materials.Core.Assignment;

public sealed record ExecutableAssignment(
    BodyId BodyId,
    MaterialId MaterialId,
    IReadOnlyList<SideEffectInstruction> SideEffects);
