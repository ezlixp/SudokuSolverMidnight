namespace SudokuSolver.Constraints.Helpers.Midnight_Custom;

public static class MidnightCellHelper
{
    private static bool initialized = false;
    public static bool[,] isMidnight = { };

    static bool[,] allowed = new bool[9, 9]; // true = allowed cell
    static List<bool[,]> allGrids = new();
    static bool[] usedCol = new bool[9];
    static bool[] usedBox = new bool[9];
    static int[] choice = new int[9]; // chosen column for each row
    static int num = 0;
    public static int curIdx = 0;

    static int BoxId(int r, int c)
    {
        return r / 3 * 3 + c / 3;
    }

    static void PrintGrid(int num)
    {
    }

    static void Solve(int row)
    {
        if (row == 9)
        {
            allGrids.Add(new bool[9, 9]);
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    allGrids[num][r, c] = (choice[r] == c);
                }
            }
            num++;
            return;
        }

        for (int c = 0; c < 9; c++)
        {
            if (!allowed[row, c]) continue;

            int b = BoxId(row, c);
            if (usedCol[c] || usedBox[b]) continue;

            usedCol[c] = true;
            usedBox[b] = true;
            choice[row] = c;

            Solve(row + 1);

            usedCol[c] = false;
            usedBox[b] = false;
        }
    }
    public static int[] applyMidnight(int[] flattenedCells)
    {
        int idx = 0;
        for (int i = 0; i < 9; i++)
            for (int j = 0; j < 9; j++)
            {
                if (isMidnight[i, j])
                    flattenedCells[idx] ^= 1 << 10;
                ++idx;
            }
        return flattenedCells;
    }
    public static bool NextMidnight()
    {
        if (curIdx == 46655) return false;
        isMidnight = allGrids[++curIdx];
        return true;
    }

    public static void Init()
    {
        curIdx = 0;
        if (initialized) return;
        initialized = true;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                allowed[r, c] = true;
            }
        }

        Solve(0);
        isMidnight = allGrids[0];
    }
}