using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField]
    private Shaker cameraShaker;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Player player;

    [SerializeField]
    private UnityEvent gameOverEvent;

    private bool gameOver = false;

    public void GoToMenu()
    {
        cameraShaker.Shake();
        audioManager.CreateTemporaryAudioSource("MenuButton");
        Invoke(nameof(LoadMenuScene), 0.3f);
    }

    private void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void Update()
    {
        if (!gameOver && player.transform.position.y  < -400)
        {
            gameOverEvent.Invoke();
            gameOver = true;
        }
    }
}
