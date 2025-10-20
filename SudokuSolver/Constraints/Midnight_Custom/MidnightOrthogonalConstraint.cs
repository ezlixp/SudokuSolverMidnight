using SudokuSolver.Constraints.Helpers.Midnight_Custom;

namespace SudokuSolver.Constraints.Midnight_Custom;

public abstract class MidnightOrthogonalConstraint : OrthogonalValueConstraint
{

    // we call base with an empty string to ensure the grandparent class Constraint remains called, but nothing runs in orthogonalvalueconstraint.cs
    public MidnightOrthogonalConstraint(Solver sudokuSolver, string options) : base(sudokuSolver, "")
    {
        HashSet<int> markerValues = new();
        options = options.ToLowerInvariant();
        foreach (string optionGroup in options.Split(separator: ';', options: StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            Match match;
            match = twoCellsRegex.Match(optionGroup);
            if (match.Success)
            {
                string valueStr = match.Groups[1].Value;
                int i0 = int.Parse(match.Groups[2].Value) - 1;
                int j0 = int.Parse(match.Groups[3].Value) - 1;
                int i1 = int.Parse(match.Groups[4].Value) - 1;
                int j1 = int.Parse(match.Groups[5].Value) - 1;
                markers.Add(CellPair((i0, j0), (i1, j1)), 12);
                markerValues.Add(12);
                continue;
            }

            throw new ArgumentException($"[{GetType().Name}] Unrecognized options group: {optionGroup}");
        }

        // do not run these as they will be ran in super
        // clearValuesNegative = initClearValuesNegative();
        // initClearValuesPositiveByMarker(markerValues);
    }
    // these constructors aren't used in the context of fpuzzles imports 
    public MidnightOrthogonalConstraint(Solver sudokuSolver, int negativeConstraintValue) : base(sudokuSolver, negativeConstraintValue)
    {
    }

    public MidnightOrthogonalConstraint(Solver sudokuSolver, int markerValue, (int, int) cell1, (int, int) cell2) : base(sudokuSolver, markerValue, cell1, cell2)
    {
    }

    protected override int DefaultMarkerValue => 12;

    protected override void initClearValuesPositiveByMarker(IEnumerable<int> markerValues)
    {
        // do nothing, since midnight orthogonal constraints will not use this arary.
    }

    // do nothing, since midnight otrhogonal constraints does not use this.
    protected override bool IsPairAllowedAcrossMarker(int markerValue, int v0, int v1) => true;

    public override LogicResult InitCandidates(Solver sudokuSolver)
    {
        var board = sudokuSolver.Board;
        bool changed = false;
        foreach (var (markerCells, markerVal) in markers)
        {
            var (i0, j0, i1, j1) = markerCells;
            uint cellMask0 = board[i0, j0] & ~valueSetMask;
            uint cellMask1 = board[i1, j1] & ~valueSetMask;

            // Find which values are compatable between these masks
            for (int v = 1; v <= MAX_VALUE; v++)
            {
                uint valueMask = ValueMask(v);
                // clear values are values it can't be

                // If cell0 has this value and setting it would clear all values from cell1,
                // then remove this value as a candidate from cell0.
                if ((cellMask0 & valueMask) != 0)
                {
                    int v1 = MidnightCellHelper.isMidnight[i0, j0] ? 12 : v;
                    List<int> needstobe = otherCell(v1);
                    if (MidnightCellHelper.isMidnight[i1, j1] && !needstobe.Contains(12) || !MidnightCellHelper.isMidnight[i1, j1] && !needstobe.Any(val => (ValueMask(val) & cellMask1) != 0)) {
                        if (!sudokuSolver.ClearValue(i0, j0, v))
                        {
                            return LogicResult.Invalid;
                        }
                        changed = true;
                    }
                }

                // If cell1 has this value and setting it would clear all values from cell0,
                // then remove this value as a candidate from cell1.
                if ((cellMask1 & valueMask) != 0)
                {
                    int v1 = MidnightCellHelper.isMidnight[i1, j1] ? 12 : v;
                    List<int> needstobe = otherCell(v1);
                    if (MidnightCellHelper.isMidnight[i0, j0] && !needstobe.Contains(12) || !MidnightCellHelper.isMidnight[i0, j0] && !needstobe.Any(val => (ValueMask(val) & cellMask0) != 0))
                    {
                        if (!sudokuSolver.ClearValue(i1, j1, v))
                        {
                            return LogicResult.Invalid;
                        }
                        changed = true;
                    }
                }
            }
        }
        return changed ? LogicResult.Changed : LogicResult.None;

    }

    // skip implementation, as puzzles with midnight constraints will only be solved through brute forcing
    public override LogicResult StepLogic(Solver sudokuSolver, List<LogicalStepDesc> logicalStepDescription, bool isBruteForcing)
    {
        return LogicResult.None;
    }
    public override LogicResult InitLinks(Solver sudokuSolver, List<LogicalStepDesc> logicalStepDescription, bool isInitializing)
    {
        if (!isInitializing)
        {
            return LogicResult.None;
        }

        var overrideMarkers = GetRelatedConstraints(sudokuSolver).SelectMany(x => x.Markers.Keys).ToHashSet();

        for (int i0 = 0; i0 < HEIGHT; i0++)
        {
            for (int j0 = 0; j0 < WIDTH; j0++)
            {
                var cell0 = (i0, j0);
                int cellIndex0 = FlatIndex(cell0);
                foreach (var cell1 in AdjacentCells(i0, j0))
                {
                    int cellIndex1 = FlatIndex(cell1);
                    var pair = CellPair(cell0, cell1);
                    if (markers.TryGetValue(pair, out int markerValue))
                    {
                        for (int v0 = 1; v0 <= MAX_VALUE; v0++)
                        {
                            List<int> needstobe = otherCell((i0, j0), v0);
                                int candIndex0 = cellIndex0 * MAX_VALUE + v0 - 1;
                                for (int v1 = 1; v1 <= MAX_VALUE; v1++)
                                {
                                    if (MidnightCellHelper.isMidnight[cell1.Item1, cell1.Item2] && !needstobe.Contains(12) || !MidnightCellHelper.isMidnight[cell1.Item1, cell1.Item2] && !needstobe.Contains(v1))
                                    {
                                        int candIndex1 = cellIndex1 * MAX_VALUE + v1 - 1;
                                        sudokuSolver.AddWeakLink(candIndex0, candIndex1);
                                    }
                                }
                            }
                        }
                    else if (negativeConstraint && !overrideMarkers.Contains(pair))
                    {
                        // for midnights, no negative impl yet
                    }
                }
            }
        }
        return LogicResult.None;
    }

    /// <summary>
    /// Get the list of integers that the other cell's value could be, given a cell's value (after applying midnight).
    /// </summary>
    /// <param name="v">The cell value, which is 12 if it came from a midnight cell.</param>
    /// <returns>The list of integers.</returns>
    protected abstract List<int> otherCell(int v);
    protected List<int> otherCell((int, int) cell, int v)
    {
        return MidnightCellHelper.isMidnight[cell.Item1, cell.Item2] ? otherCell(12) : otherCell(v);
    }
}
