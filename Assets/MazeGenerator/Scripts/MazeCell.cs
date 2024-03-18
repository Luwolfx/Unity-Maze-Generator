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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum CellDirections{
    UP = 1,
    DOWN = 2,
    LEFT = 4,
    RIGHT = 8,
    VISITED = 128
} // 1000 1111

public class MazeCell : MonoBehaviour
{
    public Maze maze => GetComponentInParent<Maze>();
    public CellDirections walls;

    public void Config(float size, MazePosition position)
    {
        transform.localScale *= size;
        gameObject.name = "Cell_"+position.x+"x"+position.y;
    }

    public MazePosition GetMazePosition()
    {
        return maze.GetMazeCellPosition(transform.position);
    }

    public void DisableWall(CellDirections wallPos)
    {
        if(transform.Find("Wall_"+wallPos.ToString()))
        {
            GameObject wall = transform.Find("Wall_"+wallPos.ToString()).gameObject;
            wall.SetActive(false);
        }
    }

    public void EnableWall(CellDirections wallPos)
    {
        if(transform.Find("Wall_"+wallPos.ToString()))
        {
            GameObject wall = transform.Find("Wall_"+wallPos.ToString()).gameObject;
            wall.SetActive(true);
        }
        else
        {
            //TODO: GENERATE WALL
        }
    }
}
