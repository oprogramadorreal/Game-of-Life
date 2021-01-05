using UnityEngine;

public sealed class StampsAdder : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Board board;

    [SerializeField]
    private GameObject stampToAddPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddStamp(Input.mousePosition);
        }
    }

    private void AddStamp(Vector3 screenPosition)
    {
        var cellIndex = PickCell(screenPosition);

        if (cellIndex.HasValue && !board.IsBorder(cellIndex.Value))
        {
            var newObj = Instantiate(stampToAddPrefab, board.CalculateGlobalCellCenter(cellIndex.Value), Quaternion.identity);
            board.AddStampObject(newObj.GetComponent<BoardObject>());
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
