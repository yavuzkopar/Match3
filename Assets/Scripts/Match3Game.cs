using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = UnityEngine.Random;
public class Match3Game : MonoBehaviour
{
    [SerializeField]
    int2 size;
    List<Match> matches;
    public List<TileDrop> DroppedTiles
    { get; private set; }
    Grid2D<TileState> grid;
    public int TotalScore { get; private set; }
    public TileState this[int x, int y] { get { return grid[x, y]; } }
    public TileState this[int2 c] { get { return grid[c]; } }
    public int2 Size => size;
    public List<int2> ClearedTileCoordinates
    { get; private set; }

    public bool NeedsFilling
    { get; private set; }
    public bool HasMatch => matches.Count > 0;
    public void StartNewGame()
    {
        TotalScore = 0;
        if (grid.Isundefined)
        {
            grid = new(size);
            matches = new();
            ClearedTileCoordinates = new();
            DroppedTiles = new();
        }
        FillGrid();
        
    }
    public void ProcessMatches()
    {
        ClearedTileCoordinates.Clear();

        for (int m = 0; m < matches.Count; m++)
        {
            Match match = matches[m];
            int2 step = match.isHorizontal ? int2(1, 0) : int2(0, 1);
            int2 c = match.coordinates;
            for (int i = 0; i < match.length; c += step, i++)
            {
                if (grid[c] != TileState.None)
                {
                    grid[c] = TileState.None;
                    ClearedTileCoordinates.Add(c);
                }
                
            }
            TotalScore += match.length;
        }

        matches.Clear();
        NeedsFilling = true;
    }
    private void FillGrid()
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                TileState a = TileState.None;
                TileState b = TileState.None;
                int potantialMatchCount = 0;
                if (x > 1)
                {
                    a = grid[x - 1, y];
                    if (a == grid[x - 2, y])
                    {
                        potantialMatchCount = 1;
                    }

                }
                if (y > 1)
                {
                    b = grid[x, y - 1];
                    if (b == grid[x, y - 2])
                    {
                        potantialMatchCount++;
                        if (potantialMatchCount == 1)
                        {
                            a = b;
                        }
                        else if (b < a)
                        {
                            (a, b) = (b, a);
                        }
                    }
                }
                TileState t = (TileState)Random.Range(1, 8 - potantialMatchCount);

                if (potantialMatchCount > 0 && t >= a)
                {
                    t += 1;
                }
                if (potantialMatchCount == 2 && t >= b)
                {
                    t += 1;
                }
                grid[x, y] = t;
            }
        }

    }
    bool FindMatches()
    {
        for (int y = 0; y < size.y; y++)
        {
            TileState start = grid[0, y];
            int length = 1;
            for (int x = 1; x < size.x; x++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x - length, y, length, true));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(size.x - length, y, length, true));
            }
        }
        for (int x = 0; x < size.x; x++)
        {
            TileState start = grid[x, 0];
            int length = 1;
            for (int y = 1; y < size.y; y++)
            {
                TileState t = grid[x, y];
                if (t == start)
                {
                    length += 1;
                }
                else
                {
                    if (length >= 3)
                    {
                        matches.Add(new Match(x, y - length, length, false));
                    }
                    start = t;
                    length = 1;
                }
            }
            if (length >= 3)
            {
                matches.Add(new Match(x, size.y - length, length, false));
            }
        }
        return HasMatch;
    }
    public bool TryMove(Move move)
    {
        grid.Swap(move.From, move.To);
        if(FindMatches())
        {
            return true;
        }
        grid.Swap(move.From, move.To);
        return false;
    }
    public void DropTiles()
    {
        DroppedTiles.Clear();

        for (int x = 0; x < size.x; x++)
        {
            int holeCount = 0;
            for (int y = 0; y < size.y; y++)
            {
                if (grid[x, y] == TileState.None)
                {
                    holeCount += 1;
                }
                else if (holeCount > 0)
                {
                    grid[x, y - holeCount] = grid[x, y];
                    DroppedTiles.Add(new TileDrop(x, y - holeCount, holeCount));
                }
            }
            for (int h = 1; h <= holeCount; h++)
            {
                grid[x, size.y - h] = (TileState)Random.Range(1, 8);
                DroppedTiles.Add(new TileDrop(x, size.y - h, holeCount));
            }
        }

        NeedsFilling = false;
        FindMatches();
    }

}

public enum TileState
{
    None,
    A,
    B,
    C,
    D,
    E,
    F,
    G
}
