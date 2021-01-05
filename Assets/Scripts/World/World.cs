using UnityEngine;

public sealed class World : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private Transform leftWall;

    [SerializeField]
    private Transform rightWall;

    private void Awake()
    {
        HandleScreenAspect(mainCamera.aspect);
    }

    private void HandleScreenAspect(float screenAspect)
    {
        if (screenAspect > 0.55f)
        {
            leftWall.position = new Vector3(-262, leftWall.position.y, leftWall.position.z);
            rightWall.position = new Vector3(262, rightWall.position.y, rightWall.position.z);
        }
        else if (screenAspect > 0.49f)
        {
            leftWall.position = new Vector3(-238.45f, leftWall.position.y, leftWall.position.z);
            rightWall.position = new Vector3(238.45f, rightWall.position.y, rightWall.position.z);
        }
    }
}
