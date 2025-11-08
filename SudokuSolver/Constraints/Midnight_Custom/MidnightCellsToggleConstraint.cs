namespace SudokuSolver.Constraints;

[Constraint(DisplayName = "Midnight Cells Toggle", ConsoleName = "midnightcellstoggle")]
public class MidnightCellsToggleConstraint : Constraint
{
    public MidnightCellsToggleConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, options) { }

    public override bool EnforceConstraint(Solver sudokuSolver, int i, int j, int val) => false;
}
