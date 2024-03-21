using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMazeGeneration : MonoBehaviour
{
    public Transform player;
    public MazeGenerator mazeGen;
    public Maze playerMazeBlock;
    public Vector2 lastSpawnPos;

    [Header("Maze Configs")]
    [SerializeField] Vector2 startPosition;
    [SerializeField][Range(1, 200)] int mazeWidth = 10;
    [SerializeField][Range(1, 200)] int mazeHeight = 10;
    [SerializeField][Range(1, 10)] float mazeCellSize = 2f;
    [SerializeField][Range(0, 10)] int mazeCenterCarving = 0;
    [SerializeField][Range(.001f, .5f)] float mazeWallThickness = .05f;
    [SerializeField] GameObject mazeMazeCellPrefab;
    [SerializeField] GameObject mazeMazeCellWallPrefab;

    const int timerFrequency = 10;
    float timerInterval;
    float timer;

    public void Start()
    {
        timerInterval = 1.0f / timerFrequency;
        UpdateMaze(new Vector2(0, 0));
    }

    public void Update()
    {
        if(playerMazeBlock)
        {
            if(lastSpawnPos != playerMazeBlock.GetPosition())
            {
                lastSpawnPos = playerMazeBlock.GetPosition();
                GenerateMazeAroundPlayer();
            }
        }

        timer -= Time.deltaTime;
        if(timer < 0)
        {
            playerMazeBlock = mazeGen.GetMazeBlockAtPosition(player.position);

            timer += timerInterval;
        }
    }


    [ContextMenu("UpdateMazeOnPlayerPosition")]
    public void GenerateMazeAroundPlayer()
    {
        UpdateMaze(playerMazeBlock.GetPosition());
    }

    public void UpdateMaze(Vector2 target)
    {
        GenerateMazeBlocks(Get9BlocksNeededPositions(target), mazeWallThickness, mazeMazeCellPrefab, mazeMazeCellWallPrefab);
        RemoveMazeBlocks(Get9BlocksPositions(target));
        UpdateBorderWalls();
    }

    public void GenerateMazeBlocks(List<Vector2> targetPos, float wallThickness, GameObject cellPrefab, GameObject wallPrefab)
    {
        foreach(Vector2 spawnPos in targetPos)
            GenerateMazeBlock(new Vector2(spawnPos.x, spawnPos.y), mazeCenterCarving, wallThickness, cellPrefab, wallPrefab);
    }

    public Maze GenerateMazeBlock(Vector2 spawnPos, int centerCarveSpace, float wallThickness, GameObject cellPrefab, GameObject wallPrefab)
    {
        return mazeGen.CreateMaze(spawnPos, mazeWidth, mazeHeight, mazeCellSize, centerCarveSpace, wallThickness, cellPrefab, wallPrefab);
    }

    public void RemoveMazeBlocks(List<Vector2> blacklist)
    {
        foreach(Maze block in mazeGen.GetMazeBlocks())
        {
            if(!blacklist.Contains(block.GetPosition()))
                DestroyImmediate(block.gameObject);
        }
    }

    public void UpdateBorderWalls()
    {
        foreach(Maze mazeBlock in mazeGen.GetMazeBlocks())
        {
            Vector2 upNeighboorPosition = mazeBlock.GetPosition() + new Vector2(0, mazeWidth);
            if(mazeGen.GetMazeBlockAtPosition(upNeighboorPosition))
                mazeBlock.DisableBorderWalls(CellDirections.UP);
            else
                mazeBlock.EnableBorderWalls(CellDirections.UP);

            Vector2 downNeighboorPosition = mazeBlock.GetPosition() - new Vector2(0, mazeWidth);
            if(mazeGen.GetMazeBlockAtPosition(downNeighboorPosition))
                mazeBlock.DisableBorderWalls(CellDirections.DOWN);
            else
                mazeBlock.EnableBorderWalls(CellDirections.DOWN);

            Vector2 rightNeighboorPosition = mazeBlock.GetPosition() + new Vector2(mazeHeight, 0);
            if(mazeGen.GetMazeBlockAtPosition(rightNeighboorPosition))
                mazeBlock.DisableBorderWalls(CellDirections.RIGHT);
            else
                mazeBlock.EnableBorderWalls(CellDirections.RIGHT);

            Vector2 leftNeighboorPosition = mazeBlock.GetPosition() - new Vector2(mazeHeight, 0);
            if(mazeGen.GetMazeBlockAtPosition(leftNeighboorPosition))
                mazeBlock.DisableBorderWalls(CellDirections.LEFT);
            else
                mazeBlock.EnableBorderWalls(CellDirections.LEFT);
        }
    }

    public List<Vector2> Get9BlocksNeededPositions(Vector2 targetPos)
    {
        List<Vector2> neededBlocksPositions = new List<Vector2>();
        foreach(Vector2 blockPos in Get9BlocksPositions(targetPos))
        {
            if(mazeGen.GetMazeBlockAtPosition(blockPos) == null)
            {
                neededBlocksPositions.Add(blockPos);
            }
        }

        return neededBlocksPositions;
    }

    public List<Vector2> Get9BlocksPositions(Vector2 targetPos)
    {
        List<Vector2> blocksPositions = new List<Vector2>();

        blocksPositions.Add(new Vector2(targetPos.x, targetPos.y));
        blocksPositions.Add(new Vector2(targetPos.x, targetPos.y+mazeHeight));
        blocksPositions.Add(new Vector2(targetPos.x, targetPos.y-mazeHeight));

        blocksPositions.Add(new Vector2(targetPos.x-mazeWidth, targetPos.y));
        blocksPositions.Add(new Vector2(targetPos.x-mazeWidth, targetPos.y+mazeHeight));
        blocksPositions.Add(new Vector2(targetPos.x-mazeWidth, targetPos.y-mazeHeight));
        
        blocksPositions.Add(new Vector2(targetPos.x+mazeWidth, targetPos.y));
        blocksPositions.Add(new Vector2(targetPos.x+mazeWidth, targetPos.y+mazeHeight));
        blocksPositions.Add(new Vector2(targetPos.x+mazeWidth, targetPos.y-mazeHeight));

        return blocksPositions;
    }
}
