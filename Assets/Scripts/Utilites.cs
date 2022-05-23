using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  static class Utilites 
{

    public static void GetUnvisitedCellsAround( int x, int y, out List<LabyrinthGeneratorCell> unvisitedCells, LabyrinthGeneratorCell[,] cells, int labyrinthSize)
    {
        unvisitedCells = new List<LabyrinthGeneratorCell>();

        if (x > 0 && !cells[x - 1, y].IsVisited) unvisitedCells.Add(cells[x - 1, y]);
        if (y > 0 && !cells[x, y - 1].IsVisited) unvisitedCells.Add(cells[x, y - 1]);
        if (x < labyrinthSize - 1 && !cells[x + 1, y].IsVisited) unvisitedCells.Add(cells[x + 1, y]);
        if (y < labyrinthSize - 1 && !cells[x, y + 1].IsVisited) unvisitedCells.Add(cells[x, y + 1]);
    }
}
