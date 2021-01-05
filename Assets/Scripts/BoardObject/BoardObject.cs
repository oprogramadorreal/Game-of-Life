using System.Collections.Generic;
using UnityEngine;

public class BoardObject : MonoBehaviour, IBoardObject
{
    public virtual IEnumerable<MetaCell> GetCellsOn(IBoard board)
    {
        throw new System.NotImplementedException();
    }
}

public interface IBoardObject
{
    IEnumerable<MetaCell> GetCellsOn(IBoard board);
}