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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Maze Config")]
    [SerializeField] bool openDeadEnds;

    [Header("Test Configs")]
    [SerializeField] Vector2 startPosition;
    [SerializeField][Range(1, 200)] int testWidth = 10;
    [SerializeField][Range(1, 200)] int testHeight = 10;
    [SerializeField][Range(1, 10)] float testCellSize = 2f;
    [SerializeField][Range(0, 10)] int testCenterCarving = 2;
    [SerializeField][Range(.001f, .5f)] float testWallThickness = .05f;
    [SerializeField] GameObject testMazeCellPrefab;
    [SerializeField] GameObject testMazeCellWallPrefab;

    [ContextMenu("CreateMazeWithTestConfig")]
    void CreateMazeWithTestConfig()
    {
        CreateMaze(startPosition, testWidth, testHeight, testCellSize, testCenterCarving, testWallThickness, testMazeCellPrefab, testMazeCellWallPrefab);
    }

    public Maze CreateMaze(Vector2 spawnPos, int width, int height, float cellSize, int centerCarveSpace, float wallThickness, GameObject cellPrefab, GameObject wallPrefab)
    {
        GenerateMaze( out Maze maze, spawnPos, width, height, cellSize, cellPrefab, wallPrefab );
        GenerateMazeGrid(maze);
        GenerateWalls(maze, centerCarveSpace, wallThickness);
        GeneratePath(maze);

        return maze;
    }

    void GenerateMaze(out Maze maze, Vector2 position, int width, int height, float cellSize, GameObject cellPrefab, GameObject wallPrefab)
    {
        GameObject mazeObject = new GameObject();
        mazeObject.name = $"MazeBlock({position.x},{position.y})";
        mazeObject.transform.position = new Vector3(position.x*cellSize, 0, position.y*cellSize);
        mazeObject.transform.parent = gameObject.transform;
        maze = mazeObject.AddComponent<Maze>();
        maze.Config(position, width, height, cellSize, cellPrefab, wallPrefab);
    }

    void GenerateMazeGrid(Maze maze)
    {
        maze.mazePositions = new List<MazePosition>();
        for (int actualWidth = 1; actualWidth <= maze.GetWidth(); actualWidth++)
        {
            for(int actualHeight = 1; actualHeight <= maze.GetHeight(); actualHeight++)
            {
                MazePosition mazePosition = new MazePosition();
                InstantiateCell( maze.GetCellPrefab(), maze, new Vector3( ( ( maze.GetPosition().x - maze.GetWidth() / 2 ) + actualWidth ) * maze.GetCellSize(), 0, ( ( maze.GetPosition().y-maze.GetHeight() / 2 ) + actualHeight ) * maze.GetCellSize() ), out MazeCell mazeCell );
                
                mazePosition.Config(actualWidth, actualHeight, mazeCell);
                maze.mazePositions.Add(mazePosition);
                mazeCell.Config(maze.GetCellSize(), mazePosition);
            }
        }
    }

    void GenerateWalls(Maze maze, int centerCarveSpace, float wallThickness)
    {
        foreach(MazePosition cellPosition in maze.mazePositions)
        {
            Vector2 mazeCenter = new Vector2(maze.GetWidth()/2, maze.GetHeight()/2);
            MazeCell cell = cellPosition.cell;
            if(Vector2.Distance(cellPosition.ToVector2(), mazeCenter) > centerCarveSpace || maze.IsBorderCell(cell))
            {
                if(cell.walls.HasFlag(CellDirections.UP))
                {
                    Transform wallTransform = Instantiate(maze.GetCellWallPrefab(), cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.UP.ToString();
                    wallTransform.localPosition = new Vector3(0f, .5f, .5f);
                    wallTransform.localScale = new Vector3(1f, 1f, wallThickness);
                }

                if(cell.walls.HasFlag(CellDirections.LEFT))
                {
                    Transform wallTransform = Instantiate(maze.GetCellWallPrefab(), cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.LEFT.ToString();
                    wallTransform.localPosition = new Vector3(-.5f, .5f, 0f);
                    wallTransform.localScale = new Vector3(wallThickness, 1f, 1f);
                }

                if(cell.walls.HasFlag(CellDirections.RIGHT) && maze.GetCellPositionRightNeighbor(cellPosition) == null)
                {
                    Transform wallTransform = Instantiate(maze.GetCellWallPrefab(), cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.RIGHT.ToString();
                    wallTransform.localPosition = new Vector3(.5f, .5f, 0f);
                    wallTransform.localScale = new Vector3(wallThickness, 1f, 1f);
                }

                if(cell.walls.HasFlag(CellDirections.DOWN) && maze.GetCellPositionDownNeighbor(cellPosition) == null)
                {
                    Transform wallTransform = Instantiate(maze.GetCellWallPrefab(), cell.transform).transform;
                    wallTransform.name = "Wall_"+CellDirections.DOWN.ToString();
                    wallTransform.localPosition = new Vector3(0f, .5f, -.5f);
                    wallTransform.localScale = new Vector3(1f, 1f, wallThickness);
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
                pathPossibilities = maze.GetCellNeighbors(path[path.Count-1]);
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

    public Maze GetMazeBlockAtPosition(Vector3 targetPos)
    {
        Maze nearestMaze = null;
        float nearestDistance = 9999;

        foreach(Transform mazeBlockTransform in transform)
        {
            Maze mazeBlock = mazeBlockTransform.GetComponent<Maze>();
            if(Vector3.Distance(targetPos, mazeBlock.transform.position) < nearestDistance)
            {
                nearestMaze = mazeBlock;
                nearestDistance = Vector3.Distance(targetPos, mazeBlock.transform.position);
            }
        }
        
        return nearestMaze;
    }

    public Maze GetMazeBlockAtPosition(Vector2 targetPos)
    {
        return GetMazeBlocks().Find(x => x.GetPosition() == targetPos);
    }

    public List<Maze> GetMazeBlocks()
    {
        List<Maze> MazeBlocks = new List<Maze>();
        foreach(Transform mazeBlockTransform in transform)
        {
            MazeBlocks.Add(mazeBlockTransform.GetComponent<Maze>());
            
        }
        return MazeBlocks;
    }
}
