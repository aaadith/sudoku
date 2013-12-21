using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    public class SudokuSolver
    {
        private int[,] grid;
        private int rows, columns;

        public SudokuSolver(int[,] grid)
        {
            this.grid = grid;
            rows = grid.GetLength(0);
            columns = grid.GetLength(1);
        }

        public HashSet<int> RowCandidatesBasedOnGrid(int row)
        {
            HashSet<int> candidates = new HashSet<int>();
            
            HashSet<int> gridelements = new HashSet<int>();

            for (int column=0; column< columns; column++)
            {
                int gridelement = grid[row, column];

                if (gridelement != 0)
                {
                    gridelements.Add(gridelement);
                }
            }

            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!gridelements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            return candidates;
        }

        public HashSet<int> ColumnCandidatesBasedOnGrid(int column)
        {
            HashSet<int> candidates = new HashSet<int>();

            HashSet<int> gridelements = new HashSet<int>();

            for(int row=0;row < rows;row++)
            {
                int gridelement = grid[row,column];
                if (gridelement != 0)
                {
                    gridelements.Add(gridelement);
                }
            }

            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!gridelements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            return candidates;

        }




        public HashSet<int> SubgridCandidatesBasedOnGrid(int row, int column)
        {
            HashSet<int> candidates = new HashSet<int>();

            int subgridSize = (int)(Math.Sqrt(rows));


            Func<int, int, bool> isPartOfSubGrid = (x,y)=> (x/subgridSize) == (y/subgridSize);

            List<int> subgridrows = new List<int>();
            List<int> subgridcols = new List<int>();

            for (int i = 0; i < rows; i++)
            {
                if (isPartOfSubGrid(i, row))
                    subgridrows.Add(i);

                if (isPartOfSubGrid(i, column))
                    subgridcols.Add(i);
            }

            HashSet<int> subgridelements = new HashSet<int>();
            foreach (int subgridrow in subgridrows)
            {
                foreach (int subgridcol in subgridcols)
                {
                    subgridelements.Add(grid[subgridrow,subgridcol]);
                }
            }


            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!subgridelements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            return candidates;
        }

        public HashSet<int> RowCandidatesBasedOnSolution(int row,  Dictionary<Tuple<int,int>,int> solution)
        {
            HashSet<int> candidates = new HashSet<int>();

            HashSet<int> solutionElements = new HashSet<int>();

            for (int column = 0; column < rows; column++)
            {
                Tuple<int, int> tuple = new Tuple<int, int>(row, column);

                if (solution.ContainsKey(tuple))
                    solutionElements.Add(solution[tuple]);
            }

            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!solutionElements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }
                
            return candidates;
        }



        public HashSet<int> ColumnCandidatesBasedOnSolution(int column, Dictionary<Tuple<int, int>, int> solution)
        {
            HashSet<int> candidates = new HashSet<int>();

            HashSet<int> solutionElements = new HashSet<int>();

            for (int row = 0; row < rows; row++)
            {
                Tuple<int, int> tuple = new Tuple<int, int>(row, column);

                if (solution.ContainsKey(tuple))
                    solutionElements.Add(solution[tuple]);
            }

            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!solutionElements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            return candidates;
        }



        public HashSet<int> SubgridCandidatesBasedOnSolution(int row, int column, Dictionary<Tuple<int, int>, int> solution)
        {
            HashSet<int> candidates = new HashSet<int>();

            int subgridSize = (int)(Math.Sqrt(rows));


            Func<int, int, bool> isPartOfSubGrid = (x, y) => (x / subgridSize) == (y / subgridSize);

            List<int> subgridrows = new List<int>();
            List<int> subgridcols = new List<int>();

            for (int i = 0; i < rows; i++)
            {
                if (isPartOfSubGrid(i, row))
                    subgridrows.Add(i);

                if (isPartOfSubGrid(i, column))
                    subgridcols.Add(i);
            }

            HashSet<int> subgridelements = new HashSet<int>();
            foreach (int subgridrow in subgridrows)
            {
                foreach (int subgridcol in subgridcols)
                {
                    Tuple<int, int> tuple = new Tuple<int, int>(subgridrow, subgridcol);

                    if (solution.ContainsKey(tuple))
                    {
                        subgridelements.Add(solution[tuple]);
                    }
                }
            }


            for (int candidate = 1; candidate <= 9; candidate++)
            {
                if (!subgridelements.Contains(candidate))
                {
                    candidates.Add(candidate);
                }
            }

            return candidates;
        }


        public HashSet<int> GetCandidates(int row, int column, int[,] grid, Dictionary<Tuple<int, int>, int> solution)
        {
            HashSet<int> allcandidates= new HashSet<int>(){1,2,3,4,5,6,7,8,9};

            HashSet<int> a = RowCandidatesBasedOnGrid(row);
            HashSet<int> b = ColumnCandidatesBasedOnGrid(column);
            HashSet<int> c = SubgridCandidatesBasedOnGrid(row, column);
            HashSet<int> d = RowCandidatesBasedOnSolution(row, solution);
            HashSet<int> e = ColumnCandidatesBasedOnSolution(column, solution);
            HashSet<int> f = SubgridCandidatesBasedOnSolution(row, column, solution);


            IEnumerable<int> candidates = from candidate in allcandidates
                where a.Contains(candidate) && b.Contains(candidate) && c.Contains(candidate) && d.Contains(candidate)
                      && e.Contains(candidate) && f.Contains(candidate)
                select candidate;

            return candidates.ToHashSet();

        }

        public bool Solve(int row, int column, Dictionary<Tuple<int, int>, int> solution)
        {
            if (grid[row, column] == 0)
            {
                HashSet<int> candidates = GetCandidates(row, column, grid, solution);

                if (candidates.Count == 0)
                    return false;

                foreach (int candidate in candidates)
                {
                    Tuple<int, int> tuple = new Tuple<int, int>(row, column);
                    solution[tuple] = candidate;

                    if (row + 1 < rows)
                    {
                        if (Solve(row + 1, column, solution) == false)
                        {
                            solution.Remove(tuple);
                            continue;
                        }
                        else
                            return true;
                    }
                    else if (column + 1 < columns)
                    {
                        if (Solve(0, column + 1, solution) == false)
                        {
                            solution.Remove(tuple);
                            continue;
                        }
                        else
                            return true;
                    }
                    else
                        return true;
                }
                return false;
            }
            else
            {
                if (row + 1 < rows)
                {
                    return Solve(row + 1, column, solution);
                }
                else if (column + 1 < columns)
                {
                    
                    return Solve(0, column + 1, solution);
                }
                else
                    return true;
            }
        }

        public int[,] Solve()
        {
            Dictionary<Tuple<int, int>, int> solution = new Dictionary<Tuple<int, int>, int>();

            if (Solve(0, 0, solution))
            {
                foreach (Tuple<int, int> point in solution.Keys)
                {
                    int row = point.Item1, column = point.Item2;

                    grid[row,column] = solution[point];
                }

                return grid;
            }

            return null;
        }

    }
}
