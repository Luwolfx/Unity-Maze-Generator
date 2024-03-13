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

using UnityEngine;

[System.Serializable]
public class MazePosition
{
    [Header("Position")]
    public int x;
    public int y;

    [Header("Others")]
    public MazeCell cell;

    public void Config(int width, int height, MazeCell cell)
    {
        x = width;
        y = height;
        this.cell = cell;
    }

    public void MarkAsVisited()
    {
        cell.walls |= CellDirections.VISITED;
    }

    public bool HasBeenVisited()
    {
        return cell.walls.HasFlag(CellDirections.VISITED);
    }

    public Vector2 ToVector2()
    {
        return new Vector2(x, y);
    }

    public MazeCell ToCell()
    {
        return cell;
    }

    public string ToString()
    {
        return "(CELL_X"+x+"_Y"+y+")";
    }
}
