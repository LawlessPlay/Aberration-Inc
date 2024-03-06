using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

    //A* Pathfinding.
    public class PathFinder
    {
        // Finds the shortest path from start to end using A* pathfinding algorithm.
        // It takes the start and end tiles, a list of searchable tiles, a boolean flag to ignore obstacles, and another to allow walking through allies as input.
        // It returns a list of tiles that represents the path from start to end.
        public List<OverlayTile> FindPath(OverlayTile start, OverlayTile end, List<OverlayTile> searchableTiles)
        {
            List<OverlayTile> openList = new List<OverlayTile>();
            List<OverlayTile> closedList = new List<OverlayTile>();

            openList.Add(start);

            while (openList.Count > 0)
            {
                OverlayTile currentOverlayTile = openList.OrderBy(x => x.F).First();

                openList.Remove(currentOverlayTile);
                closedList.Add(currentOverlayTile);

                if (currentOverlayTile.gridLocation == end.gridLocation)
                {
                    return GetFinishedList(start, end);
                }

                var neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);

                // Calculate and update the G and H cost of each neighbouring tile.
                foreach (var neighbour in neighbourTiles)
                {
                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    neighbour.G = currentOverlayTile.G + 1;
                    neighbour.H = GetManhattenDistance(end, neighbour);


                neighbour.previous = currentOverlayTile;
                if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }

            return new List<OverlayTile>();
        }

        // Returns the path from start to end by traversing the previous tile of each tile in reverse order.
        private List<OverlayTile> GetFinishedList(OverlayTile start, OverlayTile end)
        {
            List<OverlayTile> finishedList = new List<OverlayTile>();

            OverlayTile currentTile = end;

            while (currentTile != start)
            {
                finishedList.Add(currentTile);
                currentTile = currentTile.previous;
            }

            finishedList.Reverse();

            return finishedList;
        }

        // Returns the Manhattan distance between start and neighbour tile multiplied by the move cost of the neighbour tile.
        public int GetManhattenDistance(OverlayTile start, OverlayTile neighbour)
        {

            int xDistance = Mathf.Abs(start.gridLocation.x - neighbour.gridLocation.x);
            int yDistance = Mathf.Abs(start.gridLocation.y - neighbour.gridLocation.y);

            return 1 * (xDistance + yDistance);
        }
    }
