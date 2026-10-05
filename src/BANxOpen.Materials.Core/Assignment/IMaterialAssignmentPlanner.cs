namespace BANxOpen.Materials.Core.Assignment;

public interface IMaterialAssignmentPlanner
{
    AssignmentPlan Plan(MaterialAssignmentPlanningInput input);
}
