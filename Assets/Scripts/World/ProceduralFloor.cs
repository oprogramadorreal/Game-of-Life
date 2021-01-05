using UnityEngine;

public sealed class ProceduralFloor : MonoBehaviour
{
    [SerializeField]
    private Vector2 floorSize = new Vector2(200, 100);

    [SerializeField]
    private Vector2 floorTileSize = new Vector2(1, 1);

    [SerializeField]
    private GameObject floorTilePrefab;

    private Vector3 floorOrigin;

    private void Start()
    {
        var numberOfTiles = new Vector2Int(
            Mathf.FloorToInt(floorSize.x / floorTileSize.x),
            Mathf.FloorToInt(floorSize.y / floorTileSize.y)
        );

        var actualFloorSize = new Vector2(
            numberOfTiles.x * floorTileSize.x,
            numberOfTiles.y * floorTileSize.y
        );

        floorOrigin = new Vector3(
            -actualFloorSize.x / 2.0f,
            -actualFloorSize.y / 2.0f,
            0.0f
        );

        for (var x = 0; x < numberOfTiles.x; ++x)
        {
            for (var y = 0; y < numberOfTiles.y; ++y)
            {
                var tileObject = Instantiate(floorTilePrefab, CalculateTileCenter(x, y), Quaternion.identity, transform);
                tileObject.transform.localScale = new Vector3(floorTileSize.x, floorTileSize.y, 1.0f);
            }
        }
    }

    private Vector3 CalculateTileCenter(int x, int y)
    {
        var p = floorOrigin
            + new Vector3(x * floorTileSize.x, y * floorTileSize.y, 0)
            + new Vector3(floorTileSize.x / 2.0f, floorTileSize.y / 2.0f, 0.0f);

        return transform.TransformPoint(p);
    }
}
