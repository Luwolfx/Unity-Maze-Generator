/*
* ======================================================================
*
?   LUWOLF'S MAZE GENERATOR
*
*   Made by: Luwolf
*   Link: https://luwolf.top
*
*   SUPPORT ME ON KO-FI!
*   https://ko-fi.com/luwolf
*
* ======================================================================
*/

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Maze : MonoBehaviour
{
    [Header("Maze Settings")]
    [SerializeField] Vector2 position;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] float cellSize;
    [SerializeField] GameObject mazeCellPrefab;
    [SerializeField] GameObject wallPrefab;

    [Header("Maze Info")]
    public List<MazePosition> mazePositions;

#region MAZE_INFO

    public void Config(Vector2 position, int width, int height, float cellSize, GameObject mazeCellPrefab, GameObject wallPrefab)
    {
        this.position = position;
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.mazeCellPrefab = mazeCellPrefab;
        this.wallPrefab = wallPrefab;
    }

    public Vector2 GetPosition() => position;
    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellSize() => cellSize;
    public GameObject GetCellPrefab() => mazeCellPrefab;
    public GameObject GetCellWallPrefab() => wallPrefab;

#endregion

#region CELL_INFO

    public MazePosition GetMazeCellPosition(int width, int height)
    {
        return mazePositions.Find(x => x.x == width && x.y == height);
    }

    public MazePosition GetMazeCellPosition(Vector3 position)
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
        
        MazePosition upNeighbor = GetCellPositionUpNeighbor(position);
        if(upNeighbor != null && !upNeighbor.HasBeenVisited()) result.Add(upNeighbor);

        MazePosition downNeighbor = GetCellPositionDownNeighbor(position);
        if(downNeighbor != null && !downNeighbor.HasBeenVisited()) result.Add(downNeighbor);

        MazePosition leftNeighbor = GetCellPositionLeftNeighbor(position);
        if(leftNeighbor != null && !leftNeighbor.HasBeenVisited()) result.Add(leftNeighbor);

        MazePosition rightNeighbor = GetCellPositionRightNeighbor(position);
        if(rightNeighbor != null && !rightNeighbor.HasBeenVisited()) result.Add(rightNeighbor);

        return result;
    }

    public List<MazePosition> GetCellNeighbors(MazePosition position)
    {
        List<MazePosition> result = new List<MazePosition>();
        
        MazePosition upNeighbor = GetCellPositionUpNeighbor(position);
        if(upNeighbor != null) result.Add(upNeighbor);

        MazePosition downNeighbor = GetCellPositionDownNeighbor(position);
        if(downNeighbor != null) result.Add(downNeighbor);

        MazePosition leftNeighbor = GetCellPositionLeftNeighbor(position);
        if(leftNeighbor != null) result.Add(leftNeighbor);

        MazePosition rightNeighbor = GetCellPositionRightNeighbor(position);
        if(rightNeighbor != null) result.Add(rightNeighbor);

        return result;
    }

    public MazePosition GetCellPositionUpNeighbor(MazePosition position)
    {
        return GetMazeCellPosition(position.x, position.y+1);
    }

    public MazePosition GetCellPositionDownNeighbor(MazePosition position)
    {
        return GetMazeCellPosition(position.x, position.y-1);
    }

    public MazePosition GetCellPositionRightNeighbor(MazePosition position)
    {
        return GetMazeCellPosition(position.x+1, position.y);
    }

    public MazePosition GetCellPositionLeftNeighbor(MazePosition position)
    {
        return GetMazeCellPosition(position.x-1, position.y);
    }

    public bool IsBorderCell(MazeCell cell)
    {
        if(GetCellPositionUpNeighbor(cell.GetMazePosition()) == null)
            return true;
        else if(GetCellPositionDownNeighbor(cell.GetMazePosition()) == null)
            return true;
        else if(GetCellPositionLeftNeighbor(cell.GetMazePosition()) == null)
            return true;
        else if(GetCellPositionRightNeighbor(cell.GetMazePosition()) == null)
            return true;
        
        return false;
    }

#endregion    

    [ContextMenu("DisableUpBorderWalls")]
    public void DisableUpBorderWalls()
    {
        DisableBorderWalls(CellDirections.UP);
    }

    [ContextMenu("DisableDownBorderWalls")]
    public void DisableDownBorderWalls()
    {
        DisableBorderWalls(CellDirections.DOWN);
    }

    [ContextMenu("DisableLeftBorderWalls")]
    public void DisableLeftBorderWalls()
    {
        DisableBorderWalls(CellDirections.LEFT);
    }

    [ContextMenu("DisableRightBorderWalls")]
    public void DisableRightBorderWalls()
    {
        DisableBorderWalls(CellDirections.RIGHT);
    }

    public void DisableBorderWalls(CellDirections directions)
    {
        List<MazePosition> borderPositions = GetBorderPositions();
        foreach(MazePosition mazePosition in borderPositions)
        {
            if(mazePosition.x == 1 && directions.HasFlag(CellDirections.LEFT))
            {
                mazePosition.cell.DisableWall(CellDirections.LEFT);
            }

            if(mazePosition.x == width && directions.HasFlag(CellDirections.RIGHT))
            {
                mazePosition.cell.DisableWall(CellDirections.RIGHT);
            }

            if(mazePosition.y == 1 && directions.HasFlag(CellDirections.DOWN))
            {
                mazePosition.cell.DisableWall(CellDirections.DOWN);
            }

            if(mazePosition.y == height && directions.HasFlag(CellDirections.UP))
            {
                mazePosition.cell.DisableWall(CellDirections.UP);
            }
        }
    }

    public void EnableBorderWalls(CellDirections directions)
    {
        List<MazePosition> borderPositions = GetBorderPositions();
        foreach(MazePosition mazePosition in borderPositions)
        {
            if(mazePosition.x == 1 && directions.HasFlag(CellDirections.LEFT))
                mazePosition.cell.EnableWall(CellDirections.LEFT);

            if(mazePosition.x == width && directions.HasFlag(CellDirections.RIGHT))
                mazePosition.cell.EnableWall(CellDirections.RIGHT);

            if(mazePosition.y == 1 && directions.HasFlag(CellDirections.DOWN))
                mazePosition.cell.EnableWall(CellDirections.DOWN);

            if(mazePosition.y == height && directions.HasFlag(CellDirections.UP))
                mazePosition.cell.EnableWall(CellDirections.UP);
        }
    }

    public List<MazePosition> GetBorderPositions()
    {
        return new List<MazePosition>(mazePositions.FindAll(x => x.x == 1 || x.x == width || x.y == 1 || x.y == height));
    }

    public bool IsBorderWall(MazePosition cellPosition, CellDirections wallDirection)
    {
        switch(wallDirection)
        {
            case CellDirections.UP:

                if(GetCellPositionUpNeighbor(cellPosition) == null)
                {
                    return true;
                }
                else
                    return false;

            case CellDirections.DOWN:

                if(GetCellPositionDownNeighbor(cellPosition) == null)
                    return true;
                else
                    return false;

            case CellDirections.LEFT:

                if(GetCellPositionLeftNeighbor(cellPosition) == null)
                    return true;
                else
                    return false;

            case CellDirections.RIGHT:

                if(GetCellPositionRightNeighbor(cellPosition) == null)
                    return true;
                else
                    return false;
            
            default: return false;
        }
    }

    public string ToString()
    {
        return $"MazeBlock: ({gameObject.name}, position:{position.x},{position.y}, width:{width}, height:{height}, cellsize:{cellSize}, mazeCells:{mazePositions.Count})";
    }
}
