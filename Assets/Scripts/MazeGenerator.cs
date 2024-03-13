using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Config")]
    [SerializeField] bool openDeadEnds;
    [SerializeField] GameObject mazeCellPrefab;
    [SerializeField] GameObject wallPrefab;

    [Header("Test Configs")]
    [SerializeField] Vector2 startPosition;
    [SerializeField][Range(1, 200)] int width = 10;
    [SerializeField][Range(1, 200)] int height = 10;
    [SerializeField][Range(1, 10)] float cellSize = 2;
    [SerializeField][Range(1, 25)] int centerCarving = 2;

    [ContextMenu("CreateMazeWithTestConfig")]
    void CreateMazeWithTestConfig()
    {
        CreateMaze(startPosition, width, height, cellSize, centerCarving);
    }

    public void CreateMaze(Vector2 spawnPos, int width, int height, float cellSize, int centerCarveSpace)
    {
        GenerateMaze( out Maze maze );
        GenerateMazeGrid(spawnPos, maze, width, height, cellSize);
        GenerateWalls(maze, centerCarveSpace);
        GeneratePath(maze);
    }

    void GenerateMaze(out Maze maze)
    {
        GameObject mazeObject = new GameObject();
        mazeObject.name = "Maze";
        mazeObject.transform.parent = gameObject.transform;
        maze = mazeObject.AddComponent<Maze>();
    }

    void GenerateMazeGrid(Vector2 spawnPos, Maze maze, int width, int height, float cellSize)
    {
        maze.width = width;
        maze.height = height;
        maze.cellSize = cellSize;
        maze.mazePositions = new List<MazePosition>();
        for (int actualWidth = 1; actualWidth <= width; actualWidth++)
        {
            for(int actualHeight = 1; actualHeight <= height; actualHeight++)
            {
                MazePosition mazePosition = new MazePosition();
                InstantiateCell(mazeCellPrefab, maze, new Vector3(((spawnPos.x-width/2)+actualWidth)*cellSize, 0, ((spawnPos.y-height/2)+actualHeight)*cellSize), out MazeCell mazeCell);
                
                mazePosition.Config(actualWidth, actualHeight, mazeCell);
                maze.mazePositions.Add(mazePosition);
                mazeCell.Config(cellSize, mazePosition);
            }
        }
    }

    void GenerateWalls(Maze maze, int centerCarveSpace)
    {
        foreach(MazePosition cellPosition in maze.mazePositions)
        {
            Vector2 mazeCenter = new Vector2(maze.width/2, maze.height/2);
            MazeCell cell = cellPosition.cell;
            if(Vector2.Distance(cellPosition.ToVector2(), mazeCenter) > centerCarveSpace)
            {
                if(cell.walls.HasFlag(CellDirections.UP))
                {
                    Transform wallTransform = Instantiate(wallPrefab, cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.UP.ToString();
                    wallTransform.localPosition = new Vector3(0f, .5f, .5f);
                    wallTransform.localScale = new Vector3(1f, 1f, .05f);
                }

                if(cell.walls.HasFlag(CellDirections.LEFT))
                {
                    Transform wallTransform = Instantiate(wallPrefab, cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.LEFT.ToString();
                    wallTransform.localPosition = new Vector3(-.5f, .5f, 0f);
                    wallTransform.localScale = new Vector3(.05f, 1f, 1f);
                }

                if(cell.walls.HasFlag(CellDirections.RIGHT) && maze.GetPositionRightNeighbor(cellPosition) == null)
                {
                    Transform wallTransform = Instantiate(wallPrefab, cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.RIGHT.ToString();
                    wallTransform.localPosition = new Vector3(.5f, .5f, 0f);
                    wallTransform.localScale = new Vector3(.05f, 1f, 1f);
                }

                if(cell.walls.HasFlag(CellDirections.DOWN) && maze.GetPositionDownNeighbor(cellPosition) == null)
                {
                    Transform wallTransform = Instantiate(wallPrefab, cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.DOWN.ToString();
                    wallTransform.localPosition = new Vector3(0f, .5f, -.5f);
                    wallTransform.localScale = new Vector3(1f, 1f, .05f);
                }
            }
        }
    }

    void GeneratePath(Maze maze)
    {
        bool deadEnd = false;
        List<MazePosition> path = new List<MazePosition>();
        bool deadEndOpened = false;

        maze.mazePositions[0].MarkAsVisited();
        path.Add(maze.mazePositions[0]);

        while(!deadEnd)
        {

            List<MazePosition> pathPossibilities = maze.GetUnvisitedNeighbors(path[path.Count-1]);

            if(pathPossibilities.Count > 0)
            {
                int randomPos = Random.Range(0, pathPossibilities.Count);

                maze.DisableNeighborsWall(path[path.Count-1], pathPossibilities[randomPos]);
                pathPossibilities[randomPos].MarkAsVisited();
                path.Add(pathPossibilities[randomPos]);
                deadEndOpened = false;
            }
            else if(openDeadEnds && !deadEndOpened)
            {
                pathPossibilities = maze.GetNeighbors(path[path.Count-1]);
                if(pathPossibilities.Contains(path[path.Count-2]))
                    pathPossibilities.Remove(path[path.Count-2]);
                deadEndOpened = true;

                if(pathPossibilities.Count > 0)
                {
                    int randomPos = Random.Range(0, pathPossibilities.Count);

                    maze.DisableNeighborsWall(path[path.Count-1], pathPossibilities[randomPos]);
                    pathPossibilities[randomPos].MarkAsVisited();
                    path.Add(pathPossibilities[randomPos]);
                }
            }
            else if(path.Count > 0)
            {
                path.RemoveAt(path.Count-1);
                if(path.Count <= 0)
                    deadEnd = true;
            }
            else
            {
                deadEnd = true;
            }
        }
    }

    void InstantiateCell(GameObject cellPrefab, Maze maze, Vector3 position, out MazeCell returnCell)
    {
        GameObject instantiatedCell = Instantiate(cellPrefab, maze.transform);
        instantiatedCell.transform.position = position;
        returnCell = instantiatedCell.GetComponent<MazeCell>();
    }

}
