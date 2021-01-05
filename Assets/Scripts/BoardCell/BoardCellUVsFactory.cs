using System.Collections.Generic;
using UnityEngine;

public static class BoardCellUVsFactory
{
    private static readonly Dictionary<BoardCellType, Vector2[]> tileTypeToUVs = new Dictionary<BoardCellType, Vector2[]>()
    {
        { BoardCellType.Dead, GetTileUVCoords(0, 0) },
        { BoardCellType.Alive, GetTileUVCoords(1, 1) },
        { BoardCellType.Player, GetTileUVCoords(0, 1) },
    };

    private static Vector2[] GetTileUVCoords(int uOffset, int vOffset)
    {
        const float textureSize = 96;
        const float tileSize = 32;
        const float offsetFix = 0.5f * (tileSize / textureSize); // based on https://answers.unity.com/questions/1009926/texture-bleeding-on-mesh.html

        var u = uOffset * tileSize;
        var v = vOffset * tileSize;

        return new Vector2[]
        {
            new Vector2(u / textureSize + offsetFix, v / textureSize + offsetFix),
            new Vector2(u / textureSize + offsetFix, (v + tileSize) / textureSize - offsetFix),
            new Vector2((u + tileSize) / textureSize - offsetFix, (v + tileSize) / textureSize - offsetFix),
            new Vector2((u + tileSize) / textureSize - offsetFix, v / textureSize + offsetFix),
        };
    }

    public static Vector2[] GetUVsFor(BoardCellType type)
    {
        return tileTypeToUVs[type];
    }

    public static Vector2[] GetTransparentUVs()
    {
        return GetTileUVCoords(2, 0);
    }
}
