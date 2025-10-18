namespace SudokuSolver.Constraints.Midnight_Custom;

[Constraint(DisplayName = "Midnight Sum", ConsoleName = "msum")]
public class MidnightSumConstraint : OrthogonalValueConstraint
{
    public MidnightSumConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, options)
    {
    }

    public MidnightSumConstraint(Solver sudokuSolver, int negativeConstraintValue) : base(sudokuSolver, negativeConstraintValue)
    {
    }

    public MidnightSumConstraint(Solver sudokuSolver, int markerValue, (int, int) cell1, (int, int) cell2) : base(sudokuSolver, markerValue, cell1, cell2)
    {
    }

    protected override OrthogonalValueConstraint createNegativeConstraint(Solver sudokuSolver, int negativeConstraintValue)
    {
        return new MidnightSumConstraint(sudokuSolver, negativeConstraintValue);
    }

    protected override OrthogonalValueConstraint createMarkerConstraint(Solver sudokuSolver, int markerValue, (int, int) cell1, (int, int) cell2)
    {
        return new MidnightSumConstraint(sudokuSolver, markerValue, cell1, cell2);
    }

    protected override bool IsPairAllowedAcrossMarker(int markerValue, int v0, int v1) => (v0 + v1 == markerValue);

    protected override int DefaultMarkerValue => 5;
}
