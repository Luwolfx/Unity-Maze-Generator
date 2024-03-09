using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Maze : MonoBehaviour
{
    public int width;
    public int height;
    public float cellSize;
    public List<MazePosition> mazePositions;

    public MazePosition GetMazePosition(int width, int height)
    {
        return mazePositions.Find(x => x.x == width && x.y == height);
    }

    public MazePosition GetMazePosition(Vector3 position)
    {
        return mazePositions.Find(x => x.cell.transform.position == position);
    }

    public void DisableNeighborsWall(MazePosition neighbor1, MazePosition neighbor2)
    {
        if(neighbor1.x < neighbor2.x)
        {
            neighbor2.cell.DisableWall(CellWalls.LEFT);
        }
        else if(neighbor1.x > neighbor2.x)
        {
            neighbor1.cell.DisableWall(CellWalls.LEFT);
        }
        else if(neighbor1.y < neighbor2.y)
        {
            neighbor1.cell.DisableWall(CellWalls.UP);
        }
        else if(neighbor1.y > neighbor2.y)
        {
            neighbor2.cell.DisableWall(CellWalls.UP);
        }
    }

    public List<MazePosition> GetUnvisitedNeighbors(MazePosition position)
    {
        List<MazePosition> result = new List<MazePosition>();
        
        MazePosition upNeighbor = GetPositionUpNeighbor(position);
        if(upNeighbor != null && !upNeighbor.HasBeenVisited()) result.Add(upNeighbor);

        MazePosition downNeighbor = GetPositionDownNeighbor(position);
        if(downNeighbor != null && !downNeighbor.HasBeenVisited()) result.Add(downNeighbor);

        MazePosition leftNeighbor = GetPositionLeftNeighbor(position);
        if(leftNeighbor != null && !leftNeighbor.HasBeenVisited()) result.Add(leftNeighbor);

        MazePosition rightNeighbor = GetPositionRightNeighbor(position);
        if(rightNeighbor != null && !rightNeighbor.HasBeenVisited()) result.Add(rightNeighbor);

        return result;
    }

    public MazePosition GetPositionUpNeighbor(MazePosition position)
    {
        return GetMazePosition(position.x, position.y+1);
    }

    public MazePosition GetPositionDownNeighbor(MazePosition position)
    {
        return GetMazePosition(position.x, position.y-1);
    }

    public MazePosition GetPositionRightNeighbor(MazePosition position)
    {
        return GetMazePosition(position.x+1, position.y);
    }

    public MazePosition GetPositionLeftNeighbor(MazePosition position)
    {
        return GetMazePosition(position.x-1, position.y);
    }
    
}
