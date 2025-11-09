namespace SudokuSolver.Constraints;

[Constraint(DisplayName = "Midnight Cells Toggle", ConsoleName = "midnightcellstoggle")]
public class MidnightCellsToggleConstraint : Constraint
{
    public MidnightCellsToggleConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, options) { }

    // This constraint is always satisfied as it is just a toggle
    public override bool EnforceConstraint(Solver sudokuSolver, int i, int j, int val) => true;
}
