using LZStringCSharp;
using System.Text.Json;

namespace SudokuSolver.Constraints.Helpers.Midnight_Custom;

public static class MidnightCellHelper
{
    private static bool initialized = false;
    public static bool[,] isMidnight = new bool[9, 9];

    static bool[,] allowed = new bool[9, 9]; // true = allowed cell
    static List<bool[,]> allGrids = new();
    static bool[] usedCol = new bool[9];
    static bool[] usedBox = new bool[9];
    static int[] choice = new int[9]; // chosen column for each row
    public static int num = 0;
    public static int curIdx = 0;
    static string lastStartingData;

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
        {
            for (int j = 0; j < 9; j++)
            {
                if (isMidnight[i, j])
                {
                    flattenedCells[idx] ^= 1 << 10;
                }
                ++idx;
            }
        }


        return flattenedCells;
    }
    public static bool NextMidnight()
    {
        if (curIdx == num - 1) return false;
        isMidnight = allGrids[++curIdx];
        return true;
    }

    public static void Init(string messageData)
    {
        if (messageData.Contains("?load="))
        {
            int trimStart = messageData.IndexOf("?load=") + "?load=".Length;
            messageData = messageData[trimStart..];
        }

        string fpuzzlesJson = LZString.DecompressFromBase64(messageData);
        var fpuzzlesData = JsonSerializer.Deserialize(fpuzzlesJson, FpuzzlesJsonContext.Default.FPuzzlesBoard);
        curIdx = 0;
        if (initialized && messageData == lastStartingData)
        {
            isMidnight = allGrids[curIdx];
            return;
        }
        lastStartingData = messageData;
        initialized = true;
        num = 0;
        for (int r = 0; r < 9; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                allowed[r, c] = true;
            }
        }
        if (fpuzzlesData.midnightkropkisequence != null)
        {
            foreach (var kropkiSequence in fpuzzlesData.midnightkropkisequence)
            {
                foreach (var cell in kropkiSequence.cells)
                {
                    int row = int.Parse(cell[1].ToString()) - 1;
                    int column = int.Parse(cell[3].ToString()) - 1;
                    allowed[row, column] = false;
                }
            }
        }
        if (fpuzzlesData.midnightsum != null)
        {
            foreach (var sum in fpuzzlesData.midnightsum)
            {
                foreach (var cell in sum.cells)
                {
                    int row = int.Parse(cell[1].ToString()) - 1;
                    int column = int.Parse(cell[3].ToString()) - 1;
                    allowed[row, column] = false;
                }
            }
        }

        Solve(0);
        // debug code that makes the only possible midnight orientation the correct one:
        // allGrids.Clear();
        // allGrids.Add(new bool[,]
        // {
        //     {false, false, false, true, false, false, false, false, false},
        //     {true, false, false, false, false, false, false, false, false},
        //     {false, false, false, false, false, false, true, false, false},
        //     {false, true, false, false, false, false, false, false, false},
        //     {false, false, false, false, false, false, false, false, true},
        //     {false, false, false, false, true, false, false, false, false},
        //     {false, false, true, false, false, false, false, false, false},
        //     {false, false, false, false, false, true, false, false, false},
        //     {false, false, false, false, false, false, false, true, false},
        // });
        // num = 1;
        isMidnight = allGrids[0];
    }
}