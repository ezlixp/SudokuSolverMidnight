
using SudokuSolver.Constraints.Midnight_Custom;


namespace SudokuSolver.Constraints.Midnight_Custom;

public class MidnightProductConstraint : MidnightOrthogonalConstraint
{
    public MidnightProductConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, options)
    {
    }


    protected override OrthogonalValueConstraint createMarkerConstraint(Solver sudokuSolver, int markerValue, (int, int) cell1, (int, int) cell2)
    {
        throw new NotImplementedException();
    }

    protected override OrthogonalValueConstraint createNegativeConstraint(Solver sudokuSolver, int negativeConstraintValue)
    {
        throw new NotImplementedException();
    }

    // this won't be used 
    protected override bool IsPairAllowedAcrossMarker(int markerValue, int v0, int v1) => true;

    protected override List<int> otherCell(int v) => 12 % v == 0 ? [12 / v] : [];
}