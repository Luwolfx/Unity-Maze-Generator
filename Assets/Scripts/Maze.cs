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
            neighbor2.cell.DisableWall(CellDirections.LEFT);
        }
        else if(neighbor1.x > neighbor2.x)
        {
            neighbor1.cell.DisableWall(CellDirections.LEFT);
        }
        else if(neighbor1.y < neighbor2.y)
        {
            neighbor1.cell.DisableWall(CellDirections.UP);
        }
        else if(neighbor1.y > neighbor2.y)
        {
            neighbor2.cell.DisableWall(CellDirections.UP);
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

    public List<MazePosition> GetNeighbors(MazePosition position)
    {
        List<MazePosition> result = new List<MazePosition>();
        
        MazePosition upNeighbor = GetPositionUpNeighbor(position);
        if(upNeighbor != null) result.Add(upNeighbor);

        MazePosition downNeighbor = GetPositionDownNeighbor(position);
        if(downNeighbor != null) result.Add(downNeighbor);

        MazePosition leftNeighbor = GetPositionLeftNeighbor(position);
        if(leftNeighbor != null) result.Add(leftNeighbor);

        MazePosition rightNeighbor = GetPositionRightNeighbor(position);
        if(rightNeighbor != null) result.Add(rightNeighbor);

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
    
    public void DisableBorderWalls(CellDirections directions)
    {
        List<MazePosition> borderPositions = GetBorderPositions();
        foreach(MazePosition mazePosition in borderPositions)
        {
            Debug.Log("Border maze cell: "+mazePosition.ToString());
            if(mazePosition.x == 1 && directions.HasFlag(CellDirections.LEFT))
            {
                Debug.Log("Disabling UP wall from: "+mazePosition.ToString());
                mazePosition.cell.DisableWall(CellDirections.LEFT);
            }

            if(mazePosition.x == width && directions.HasFlag(CellDirections.RIGHT))
            {
                Debug.Log("Disabling DOWN wall from: "+mazePosition.ToString());
                mazePosition.cell.DisableWall(CellDirections.RIGHT);
            }

            if(mazePosition.y == 1 && directions.HasFlag(CellDirections.DOWN))
            {
                Debug.Log("Disabling LEFT wall from: "+mazePosition.ToString());
                mazePosition.cell.DisableWall(CellDirections.DOWN);
            }

            if(mazePosition.y == height && directions.HasFlag(CellDirections.UP))
            {
                Debug.Log("Disabling RIGHT wall from: "+mazePosition.ToString());
                mazePosition.cell.DisableWall(CellDirections.UP);
            }
        }
    }

    public List<MazePosition> GetBorderPositions()
    {
        return new List<MazePosition>(mazePositions.FindAll(x => x.x == 1 || x.x == width || x.y == 1 || x.y == height));
    }
}
