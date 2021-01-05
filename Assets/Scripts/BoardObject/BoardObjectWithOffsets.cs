using System.Collections.Generic;
using UnityEngine;

public sealed class BoardObjectWithOffsets : BoardObject
{
    [SerializeField]
    private List<BoardCellIndex> offsets;

    [SerializeField]
    private BoardCellType cellType;

    public override IEnumerable<MetaCell> GetCellsOn(IBoard board)
    {
        var cellsOrigin = board.PickCell(transform.position);

        var xSignal = transform.localScale.x < 0.0f ? -1 : 1;
        var ySignal = transform.localScale.y < 0.0f ? -1 : 1;

        foreach (var o in offsets)
        {
            var cellIndex = new BoardCellIndex(cellsOrigin.X + o.X * xSignal, cellsOrigin.Y + o.Y * ySignal);

            yield return new MetaCell()
            {
                Index = cellIndex,
                Type = cellType
            };
        }
    }
}
