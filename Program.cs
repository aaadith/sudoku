using System;
using System.Threading;
using System.IO;
using System.Reflection;

namespace Sudoku
{
    class Program
    {
        static void Main()
        {
            string executableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string inputFile = Path.Combine(executableLocation, "sudoku.xlsx");

            IOHelper ioHelper = new IOHelper(inputFile);

            int[,] grid = ioHelper.GetGrid();
            SudokuSolver solver = new SudokuSolver(grid);
            grid = solver.Solve();

            if (grid != null)
            {            
                PrintSolution(grid);            
                ioHelper.WriteSolution(grid);
            }
            else
            {
                Console.WriteLine("invalid input");
            }
        }


        private static void PrintSolution(int[,] grid)
        {
            int rows = grid.GetLength(0), columns = grid.GetLength(1);
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Console.Write(grid[i, j] + " ");
                }
                Console.WriteLine("\n");
            }
        }
    }
}
