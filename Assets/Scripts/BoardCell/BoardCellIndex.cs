using System;
using UnityEngine;

[Serializable]
public struct BoardCellIndex : IEquatable<BoardCellIndex>
{
    [SerializeField]
    private int x;

    [SerializeField]
    private int y;

    public int X { get => x; }

    public int Y { get => y; }

    public BoardCellIndex(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(BoardCellIndex other)
    {
        return X == other.X
            && Y == other.Y;
    }
}
