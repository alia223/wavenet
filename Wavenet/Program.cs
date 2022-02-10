using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private enum RT
    {
        Start,
        End,
        Open,
        Closed,
    }

    private static RT[,] grid1 = new RT[,]
    {
        { RT.Start, RT.Open,    RT.Open,    RT.Open },
        { RT.Open,  RT.Open,    RT.Open,    RT.Open },
        { RT.Open,  RT.Open,    RT.Open,    RT.Open },
        { RT.Open,  RT.Open,    RT.Open,    RT.End  },
    };

    private static RT[,] grid2 = new RT[,]
    {
        { RT.Start,     RT.Open,    RT.Open,    RT.Open     },
        { RT.Closed,    RT.Closed,  RT.Open,    RT.Open     },
        { RT.Open,      RT.Open,    RT.Open,    RT.Open     },
        { RT.Open,      RT.Closed,  RT.Closed,  RT.Closed   },
        { RT.Open,      RT.Open,    RT.Open,    RT.End      },
    };

    private static RT[,] grid3 = new RT[,]
    {
        { RT.Start,     RT.Closed,  },
        { RT.Closed,    RT.End,     },
    };

    private struct RouteResult
    {
        public bool RouteFound;
        public int Distance;

        public void Write()
        {
            Console.WriteLine(string.Format("{0} {1}", RouteFound, Distance));
        }
    }

    private static void MoveToNewCell(ref List<int[]> visitedCells, ref List<int[]> currentPath, ref int[] currentPosition, int[] nextPosition, ref int distance)
    {
        visitedCells.Add(currentPosition);
        currentPosition = nextPosition;
        currentPath.Add(currentPosition);
        distance++;
    }

    private static RouteResult GetRoute(RT[,] grid)
    {
        bool routeFound = false;
        int distance = 0;
        int[] currentPosition = { 0, 0 };
        List<int[]> currentPath = new() { }; //All cells in current valid path we have traversed so far
        List<int[]> visitedCells = new() { new int[] { 0, 0 } };

        //While RT.End has not been reached, traverse through matrix
        while (!routeFound)
        {
            //Starting from currentPosition[i, j], in a matrix grid[i, j]: Move up: [i - 1, j]; Move Right: [i, j + 1]; Move Down: [i + 1, j]; Move Left: [i, j - 1];
            //Need to check these indices, if out of bounds, set to -1, otherwise set to actual value
            int indexRequiredToMoveUp = currentPosition[0] - 1 >= 0 ? currentPosition[0] - 1 : -1;
            int indexRequiredToMoveRight = currentPosition[1] + 1 < grid.GetLength(1) ? currentPosition[1] + 1 : -1;
            int indexRequiredToMoveDown = currentPosition[0] + 1 < grid.GetLength(0) ? currentPosition[0] + 1 : -1;
            int indexRequiredToMoveLeft = currentPosition[1] - 1 >= 0 ? currentPosition[1] - 1 : -1;

            //Check all directions to ensure index for the next cell to move to is valid AND next cell to move to is not closed AND next cell to move to has not been visited already
            bool canGoUp = indexRequiredToMoveUp != -1 && grid[indexRequiredToMoveUp, currentPosition[1]] != RT.Closed && !visitedCells.Any(x => x.SequenceEqual(new int[] { indexRequiredToMoveUp, currentPosition[1] }));
            bool canGoRight = indexRequiredToMoveRight != -1 && grid[currentPosition[0], indexRequiredToMoveRight] != RT.Closed && !visitedCells.Any(x => x.SequenceEqual(new int[] { currentPosition[0], indexRequiredToMoveRight }));
            bool canGoDown = indexRequiredToMoveDown != -1 && grid[indexRequiredToMoveDown, currentPosition[1]] != RT.Closed && !visitedCells.Any(x => x.SequenceEqual(new int[] { indexRequiredToMoveDown, currentPosition[1] }));
            bool canGoLeft = indexRequiredToMoveLeft != -1 && grid[currentPosition[0], indexRequiredToMoveLeft] != RT.Closed && !visitedCells.Any(x => x.SequenceEqual(new int[] { currentPosition[0], indexRequiredToMoveLeft }));
            
            //Move to the first valid cell that is found
            //We can check other routes later via backtracking, if needed
            if (canGoUp)
                MoveToNewCell(ref visitedCells, ref currentPath, ref currentPosition, new int[] { indexRequiredToMoveUp, currentPosition[1] }, ref distance);
            else if (canGoRight)
                MoveToNewCell(ref visitedCells, ref currentPath, ref currentPosition, new int[] { currentPosition[0], indexRequiredToMoveRight }, ref distance);
            else if (canGoDown)
                MoveToNewCell(ref visitedCells, ref currentPath, ref currentPosition, new int[] { indexRequiredToMoveDown, currentPosition[1] }, ref distance);
            else if (canGoLeft)
                MoveToNewCell(ref visitedCells, ref currentPath, ref currentPosition, new int[] { currentPosition[0], indexRequiredToMoveLeft }, ref distance);           
            //Can't go in any direction from current position due to OutOfBounds or RT.Closed, so first check if we have reach RT.End
            //Backtrack if we haven't reached RT.End
            else if (grid[currentPosition[0], currentPosition[1]] != RT.End) {
                //Mark the current cell as visited
                visitedCells.Add(currentPosition);
                //If it is not possible to backtrack [any further], exit else clause
                if (currentPath.Count <= 1) break;
                //Otherwise, if it is possible, and RT.End has still not been reached, pop cell off of currentPath to backtrack,
                currentPath.RemoveAt(currentPath.Count - 1);
                currentPosition = new int[] { currentPath[currentPath.Count - 1][0], currentPath[currentPath.Count - 1][1] };
                distance--;
            }
            if (grid[currentPosition[0], currentPosition[1]] == RT.End) routeFound = true; //Terminating condition - Route Found!!! :)
        }
        return new RouteResult { RouteFound = routeFound, Distance = distance };
    }

    public static void Main()
    {
        // Finish the 'getRoute' function to return if a grid can be navigated to get from 'Start' to 'End'
        // 'Open' can be navigated 'Closed' cannot, diagonal directions are not valid - only up/down/left/right
        // 'RouteFound' should contain if it is possible to get a complete route
        // 'Distance' should contain the total cells convered to complete a route

        GetRoute(grid1).Write();
        GetRoute(grid2).Write();
        GetRoute(grid3).Write();
    }
}
