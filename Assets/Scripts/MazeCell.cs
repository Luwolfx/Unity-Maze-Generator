using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum CellWalls{
    UP = 1,
    DOWN = 2,
    LEFT = 4,
    RIGHT = 8,
    VISITED = 128
} // 1000 1111

public class MazeCell : MonoBehaviour
{
    public Maze maze => GetComponentInParent<Maze>();
    public CellWalls walls;

    public void Config(float size, MazePosition position)
    {
        transform.localScale *= size;
        gameObject.name = "Cell_"+position.x+"x"+position.y;
    }

    public MazePosition GetMazePosition()
    {
        return maze.GetMazePosition(transform.position);
    }

    public void DisableWall(CellWalls wallPos)
    {
        GameObject wall = transform.Find("Wall_"+wallPos.ToString()).gameObject;
        wall.SetActive(false);
    }
}
