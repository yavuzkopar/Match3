using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
public class Match3Skin : MonoBehaviour
{
    [SerializeField]
    Tile[] tilePrefabs;
    [SerializeField]
    TileSwapper tileSwapper;
    float busyDuration;

    [SerializeField]
    Match3Game game;
    Grid2D<Tile> grid;
    float2 tileOffset;
    [SerializeField, Range(0.1f, 1f)]
    float dragThreshold = 0.5f;
    public bool IsPlaying => true;
    public bool IsBusy => busyDuration>0;

    [SerializeField, Range(0.1f, 20f)]
    float dropSpeed = 8f;
    [SerializeField]
    float newDropOffset = 2f;

    [SerializeField]
    TextMeshPro totalScoreText;
    public void StartNewGame()
    {
        busyDuration = 0;
        totalScoreText.SetText("0");
        game.StartNewGame();
        Debug.Log("S4");
        tileOffset = -0.5f * (float2)(game.Size - 1);

        if (grid.Isundefined)
        {
            grid = new(game.Size);
            Debug.Log(grid.SizeX);
        }
        else
        {
            for (int y = 0; y < game.Size.y; y++)
            {
                for (int x = 0; x < game.Size.x; x++)
                {
                    grid[x, y].DeSpawn();
                    grid[x, y] = null;
                }
            }
        }

        for (int y = 0; y < grid.SizeY; y++)
        {
            for (int x = 0; x < grid.SizeX; x++)
            {
                grid[x, y] = SpawnTile(game[x, y], x, y);
                Debug.Log("S2");
            }
        }

    }
    Tile SpawnTile(TileState t, float x, float y)
    {
        Debug.Log("S1");
        return tilePrefabs[(int)t - 1].Spawn(new Vector3(tileOffset.x + x, tileOffset.y + y));
    }
    public void DoWork() 
    {
        if(busyDuration>0)
        {
            tileSwapper.Update();
            busyDuration -= Time.deltaTime;
            return;
        }
        if(game.HasMatch)
        {
            ProcessMatches();
            
        }
        else if (game.NeedsFilling)
        {
            DropTiles();
        }
    }
    void ProcessMatches()
    {
        game.ProcessMatches();
        for (int i = 0; i < game.ClearedTileCoordinates.Count; i++)
        {
            int2 c = game.ClearedTileCoordinates[i];
            busyDuration = Mathf.Max(grid[c].Disappear(), busyDuration);
            grid[c] = null;
        }
        totalScoreText.SetText("{0}",game.TotalScore);
    }
    public bool EveluateDrag(Vector3 startPos, Vector3 endPos)
    {
        float2 a = ScreenToTileSpace(startPos), b = ScreenToTileSpace(endPos);
        var move = new Move(
            (int2)floor(a), (b - a) switch
            {
                var d when d.x > dragThreshold => MoveDirection.Right,
                var d when d.x < -dragThreshold => MoveDirection.Left,
                var d when d.y > dragThreshold => MoveDirection.Up,
                var d when d.y < -dragThreshold => MoveDirection.Down,
                _ => MoveDirection.None
            }
        );
        if (
            move.IsValid &&
            grid.AreValidCoordinates(move.From) && grid.AreValidCoordinates(move.To)
            )
        {
            DoMove(move);
            return false;
        }
        return true;
    }

    float2 ScreenToTileSpace(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Vector3 p = ray.origin - ray.direction * (ray.origin.z / ray.direction.z);
        return float2(p.x - tileOffset.x + 0.5f, p.y - tileOffset.y + 0.5f);
    }
    void DoMove(Move move)
    {
        bool success = game.TryMove(move);
        Tile a = grid[move.From];
        Tile b = grid[move.To];
        busyDuration = tileSwapper.Swap(a, b, !success);
        if (success)
        {
            grid[move.From] = b;
            grid[move.To] = a;
        }
    }
    void DropTiles()
    {
        game.DropTiles();

        for (int i = 0; i < game.DroppedTiles.Count; i++)
        {
            TileDrop drop = game.DroppedTiles[i];
            Tile tile;
            if (drop.fromY < grid.SizeY)
            {
                tile = grid[drop.coordinates.x, drop.fromY];
                //tile.transform.localPosition = new Vector3(
                //    drop.coordinates.x + tileOffset.x, drop.coordinates.y + tileOffset.y
                //);
            }
            else
            {
                tile = SpawnTile(
                    game[drop.coordinates], drop.coordinates.x, drop.fromY + newDropOffset
                );
            }
            grid[drop.coordinates] = tile;
            busyDuration = Mathf.Max(tile.Fall(drop.coordinates.y+ tileOffset.y,dropSpeed),busyDuration);
        }
    }

}
