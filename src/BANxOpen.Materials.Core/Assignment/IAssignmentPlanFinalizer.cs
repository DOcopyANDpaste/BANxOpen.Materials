using BANxOpen.Foundation.Contracts.Common;

namespace BANxOpen.Materials.Core.Assignment;

public interface IAssignmentPlanFinalizer
{
    /// <param name="confirmedBodyIds">The bodies the user confirmed, of those whose rules asked for it.</param>
    ExecutablePlan Finalize(AssignmentPlan plan, MaterialAssignmentPlanningInput input, HashSet<BodyId> confirmedBodyIds);
}
