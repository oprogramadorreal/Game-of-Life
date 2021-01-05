using UnityEngine;

public sealed class FloorTile : MonoBehaviour
{
    private int lifes = 2;

    private MeshRenderer meshRenderer;

    [SerializeField]
    private Material floorDamaged;

    [SerializeField]
    private GameObject particlesPrefab;

    private AudioManager audioManager;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = MeshFactory.CreateSquare2D();

        audioManager = FindObjectOfType<AudioManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Coin"))
        {
            if (--lifes <= 0)
            {
                var particles = Instantiate(particlesPrefab, gameObject.transform.position, Quaternion.identity);
                Destroy(particles, 0.5f);

                Destroy(gameObject);
                collision.collider.gameObject.SetActive(false);

                audioManager.CreateTemporaryAudioSource("TileBreak");
            }
            else
            {
                meshRenderer.material = floorDamaged;
                audioManager.CreateTemporaryAudioSource("TileHit");
            }
        }
    }
}
