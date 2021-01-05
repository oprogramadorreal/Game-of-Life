public sealed class BoardCell
{
    public BoardCellIndex Index { get; private set; }

    public BoardCellType Type { get; set; } = BoardCellType.Dead;

    /// <summary>
    /// Supposed to be managed by the GameBoard only.
    /// </summary>
    public BoardCellType NextType { get; set; } = BoardCellType.Dead;

    public BoardCell(BoardCellIndex index)
    {
        Index = index;
    }
}
