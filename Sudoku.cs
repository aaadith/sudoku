using System;
using System.Collections.Generic;
using System.Linq;


namespace Sudoku
{
    public class SudokuSolver
    {
        private int[,] grid;
        private int gridSize, subgridSize;
        private Dictionary<int, List<int>> subgridIndicesLookup; 


        public SudokuSolver(int[,] grid)
        {
            this.grid = grid;
            gridSize = grid.GetLength(0);
            subgridSize = (int)(Math.Sqrt(gridSize));
            subgridIndicesLookup = new Dictionary<int, List<int>>();
        }


        IEnumerable<int> GetCandidatesBasedOnExclusionSet(IEnumerable<int> exclusionSet)
        {
            IEnumerable<int> candidates = from candidate in Enumerable.Range(1, 9)
                                          where !exclusionSet.Contains(candidate) 
                                          select candidate;

            return candidates;
        }

        List<int> GetSubgridIndices(int index)
        {            
            if (subgridIndicesLookup.ContainsKey(index))
                return subgridIndicesLookup[index];

            List<int> subgridIndices = new List<int>();

            Func<int, int, bool> isPartOfSubGrid = (x, y) => (x / subgridSize) == (y / subgridSize);

            for (int i = 0; i < gridSize; i++)
            {
                if (isPartOfSubGrid(i, index))
                    subgridIndices.Add(i);
            }

            subgridIndicesLookup[index] = subgridIndices;

            return subgridIndices;
        }


        public IEnumerable<int> RowCandidatesBasedOnGrid(int row)
        {            
            HashSet<int> gridelements = new HashSet<int>();

            for (int column=0; column< gridSize; column++)
            {
                int gridelement = grid[row, column];

                if (gridelement != 0)
                {
                    gridelements.Add(gridelement);
                }
            }

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(gridelements);

            return candidates;
        }

        public IEnumerable<int> ColumnCandidatesBasedOnGrid(int column)
        {
            HashSet<int> gridelements = new HashSet<int>();

            for(int row=0;row < gridSize;row++)
            {
                int gridelement = grid[row,column];
                if (gridelement != 0)
                {
                    gridelements.Add(gridelement);
                }
            }

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(gridelements);

            return candidates;
        }

        public IEnumerable<int> SubgridCandidatesBasedOnGrid(int row, int column)
        {
            List<int> subgridrows = GetSubgridIndices(row);       
            List<int> subgridcols = GetSubgridIndices(column);    

            HashSet<int> subgridelements = new HashSet<int>();
            foreach (int subgridrow in subgridrows)
            {
                foreach (int subgridcol in subgridcols)
                {
                    subgridelements.Add(grid[subgridrow,subgridcol]);
                }
            }

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(subgridelements);

            return candidates;
        }

        public IEnumerable<int> RowCandidatesBasedOnSolution(int row, Dictionary<Tuple<int, int>, int> solution)
        {
            HashSet<int> solutionElements = new HashSet<int>();

            for (int column = 0; column < gridSize; column++)
            {
                Tuple<int, int> tuple = new Tuple<int, int>(row, column);

                if (solution.ContainsKey(tuple))
                    solutionElements.Add(solution[tuple]);
            }

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(solutionElements);
                
            return candidates;
        }



        public IEnumerable<int> ColumnCandidatesBasedOnSolution(int column, Dictionary<Tuple<int, int>, int> solution)
        {
            HashSet<int> solutionElements = new HashSet<int>();

            for (int row = 0; row < gridSize; row++)
            {
                Tuple<int, int> tuple = new Tuple<int, int>(row, column);

                if (solution.ContainsKey(tuple))
                    solutionElements.Add(solution[tuple]);
            }

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(solutionElements);

            return candidates;
        }



        public IEnumerable<int> SubgridCandidatesBasedOnSolution(int row, int column, Dictionary<Tuple<int, int>, int> solution)
        {            
            List<int> subgridrows = GetSubgridIndices(row);        
            List<int> subgridcols = GetSubgridIndices(column);     

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

            IEnumerable<int> candidates = GetCandidatesBasedOnExclusionSet(subgridelements);

            return candidates;
        }


        public IEnumerable<int> GetCandidates(int row, int column, int[,] grid, Dictionary<Tuple<int, int>, int> solution)
        {
            IEnumerable<int> a = RowCandidatesBasedOnGrid(row);
            IEnumerable<int> b = ColumnCandidatesBasedOnGrid(column);
            IEnumerable<int> c = SubgridCandidatesBasedOnGrid(row, column);
            IEnumerable<int> d = RowCandidatesBasedOnSolution(row, solution);
            IEnumerable<int> e = ColumnCandidatesBasedOnSolution(column, solution);
            IEnumerable<int> f = SubgridCandidatesBasedOnSolution(row, column, solution);


            IEnumerable<int> candidates = from candidate in Enumerable.Range(1,9)
                where a.Contains(candidate) && b.Contains(candidate) && c.Contains(candidate) && d.Contains(candidate)
                      && e.Contains(candidate) && f.Contains(candidate)
                select candidate;

            return candidates;

        }

        public bool Solve(int row, int column, Dictionary<Tuple<int, int>, int> solution)
        {
            if (grid[row, column] == 0)
            {
                IEnumerable<int> candidates = GetCandidates(row, column, grid, solution);
                
                if (!candidates.Any()) //if there is no candidate
                    return false;

                foreach (int candidate in candidates)
                {
                    Tuple<int, int> tuple = new Tuple<int, int>(row, column);
                    solution[tuple] = candidate;

                    if (row + 1 < gridSize)
                    {
                        if (Solve(row + 1, column, solution) == false)
                        {
                            solution.Remove(tuple);
                            continue;
                        }
                        else
                            return true;
                    }
                    else if (column + 1 < gridSize)
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
                if (row + 1 < gridSize)
                {
                    return Solve(row + 1, column, solution);
                }
                else if (column + 1 < gridSize)
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
