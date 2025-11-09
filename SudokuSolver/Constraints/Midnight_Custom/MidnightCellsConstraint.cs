namespace SudokuSolver.Constraints.Midnight_Custom;

[Constraint(DisplayName = "Midnight Cell", ConsoleName = "midnightcell")]
public class MidnightCellsConstraint : Constraint
{
    public readonly List<(int, int)> cells;
    public MidnightCellsConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, options)
    {
        var cellGroups = ParseCells(options);
        if (cellGroups.Count != 1)
        {
            throw new ArgumentException($"Midnight Cells expects 1 cell group, got {cellGroups.Count} groups.");
        }
        cells = cellGroups[0];
    }

    public override LogicResult InitCandidates(Solver sudokuSolver) => LogicResult.None;

    public override bool NeedsEnforceConstraint => false;
    public override bool EnforceConstraint(Solver sudokuSolver, int i, int j, int val) => true;

    public override LogicResult StepLogic(Solver sudokuSolver, StringBuilder logicalStepDescription, bool isBruteForcing) => LogicResult.None;

    public override List<(int, int)> Group => cells;

}