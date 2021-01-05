using UnityEngine;

public sealed class TouchManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Board board;

    [SerializeField]
    private Player player;

    private int boardSwitchLayer;

    private bool drawingOnTheBoard = false;

    private void Start()
    {
        boardSwitchLayer = 1 << LayerMask.NameToLayer("BoardSwitch");
    }

    private void Update()
    {
        var playerHorizontalMove = 0.0f;

        for (var i = 0; i < Input.touchCount; ++i)
        {
            var touch = Input.GetTouch(i);

            if (!TryToToggleBoardSwitch(touch))
            {
                TryToDrawOnTheBoard(touch);
                TryToJump(touch);
            }
        }

        if (!drawingOnTheBoard)
        {
            playerHorizontalMove = TryToMovePlayer();
        }

        player.SetHorizontalMove(playerHorizontalMove);
    }

    private float TryToMovePlayer()
    {
        var playerHorizontalMove = 0.0f;

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(Input.touchCount - 1);
            playerHorizontalMove = mainCamera.ScreenToWorldPoint(touch.position).x;
        }

        return playerHorizontalMove;
    }

    private bool TryToToggleBoardSwitch(Touch touch)
    {
        var cameraRay = mainCamera.ScreenPointToRay(touch.position);
        var hit = Physics2D.Raycast(cameraRay.origin, cameraRay.direction, float.MaxValue, boardSwitchLayer);

        if (hit.collider == null)
        {
            return false;
        }
        
        if (touch.phase == TouchPhase.Began)
        {
            var boardSwitch = hit.collider.gameObject.GetComponent<BoardSwitch>();
            boardSwitch.SetOn(!boardSwitch.IsOn());
        }
       
        return true;
    }

    private void TryToDrawOnTheBoard(Touch touch)
    {
        var cellIndex = PickCell(touch.position);

        if (cellIndex.HasValue && !board.IsBorder(cellIndex.Value))
        {
            if (touch.phase == TouchPhase.Began)
            {
                drawingOnTheBoard = true;
            }

            if (drawingOnTheBoard)
            {
                board.Add(new MetaCell
                {
                    Index = cellIndex.Value,
                    Type = BoardCellType.Alive
                });
            }
        }
        else
        {
            drawingOnTheBoard = false;
        }
    }

    private void TryToJump(Touch touch)
    {
        if (!drawingOnTheBoard && touch.phase == TouchPhase.Moved)
        {
            if (touch.deltaPosition.y > 10.0f)
            {
                player.Jump();
            }
        }
    }

    private BoardCellIndex? PickCell(Vector3 screenPosition)
    {
        var cameraRay = mainCamera.ScreenPointToRay(screenPosition);
        var cellIndex = board.PickCell(cameraRay);
        //Debug.Log(cellIndex.Value.X + " " + cellIndex.Value.Y);
        return cellIndex;
    }

    public void OnDoubleTap()
    {
        player.Jump();
    }
}
