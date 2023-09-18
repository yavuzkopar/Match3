using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct Move
{
    public MoveDirection Direction { get; private set; }
    public int2 From { get; private set; }
    public int2 To { get; private set; }

    public bool IsValid => Direction != MoveDirection.None;

    public Move(int2 coordinates, MoveDirection direction)
    {

        Direction = direction;
        From = coordinates;
        To = coordinates + direction switch
        {
            MoveDirection.Up => new int2(0, 1),
            MoveDirection.Right => new int2(1, 0),
            MoveDirection.Down => new int2(0, -1),
            _ => new int2(-1, 0)
        };
    }
}
public enum MoveDirection
{
    None,
    Up,
    Right,
    Down,
    Left,
}
