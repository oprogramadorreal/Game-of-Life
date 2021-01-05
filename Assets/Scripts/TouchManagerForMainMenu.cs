using UnityEngine;

public sealed class TouchManagerForMainMenu : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Board board;

    private void Update()
    {
        for (var i = 0; i < Input.touchCount; ++i)
        {
            var touch = Input.GetTouch(i);
            TryToDrawOnTheBoard(touch);
        }
    }

    private void TryToDrawOnTheBoard(Touch touch)
    {
        var cellIndex = PickCell(touch.position);

        if (cellIndex.HasValue && !board.IsBorder(cellIndex.Value))
        {
            board.Add(new MetaCell
            {
                Index = cellIndex.Value,
                Type = BoardCellType.Alive
            });
        }
    }

    private BoardCellIndex? PickCell(Vector3 screenPosition)
    {
        var cameraRay = mainCamera.ScreenPointToRay(screenPosition);
        var cellIndex = board.PickCell(cameraRay);
        //Debug.Log(cellIndex.Value.X + " " + cellIndex.Value.Y);
        return cellIndex;
    }
}
