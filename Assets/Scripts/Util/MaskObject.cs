using UnityEngine;

public sealed class MaskObject : MonoBehaviour
{
    private void Start()
    {
        GetComponent<SpriteRenderer>().material.renderQueue = 3002;
    }
}
