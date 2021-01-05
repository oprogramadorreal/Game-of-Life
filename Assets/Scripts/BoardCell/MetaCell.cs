/// <summary>
/// Not an actual board cell. Only describes a cell.
/// </summary>
public struct MetaCell
{
    public BoardCellIndex Index { get; set; }

    public BoardCellType Type { get; set; }
}
