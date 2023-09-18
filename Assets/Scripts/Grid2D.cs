using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public struct Grid2D<T> 
{
    T[] cells;
    int2 size;
    public Grid2D(int2 size)
    {
        this.size = size;
        cells = new T[size.x *size.y];
    }
    public int2 Size => size;
    public int SizeX => size.x;
    public int SizeY => size.y;

    public bool Isundefined => cells == null || cells.Length == 0;

    public T this[int x, int y]
    {
        get => cells[y * size.x + x];
        set => cells[y * size.x + x] = value;
    }
    public T this[int2 c]
    {
        get => cells[c.y * size.x + c.x];
        set => cells[c.y * size.x +c.x] = value;
    }
    public bool AreValidCoordinates(int2 c)
    {
        return 0 <= c.x && c.x < size.x && 0 <= c.y && c.y < size.y;
    }
    public void Swap(int2 a, int2 b) => (this[a], this[b]) = (this[b], this[a]);

}
