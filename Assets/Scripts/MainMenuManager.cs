using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private Shaker cameraShaker;

    [SerializeField]
    private AudioManager audioManager;

    private void Start()
    {
        audioManager.CreateAudioSource("MenuMusic");
    }

    public void Play()
    {
        ShakeCamera();
        ButtonPressedSound();
        Invoke("LoadMainScene", 0.3f);
    }

    private void LoadMainScene()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ShakeCamera()
    {
        cameraShaker.Shake();
    }

    public void ButtonPressedSound()
    {
        audioManager.CreateTemporaryAudioSource("MenuButton");
    }

    public void QuitGame()
    {
        ShakeCamera();
        ButtonPressedSound();
        Invoke("QuitApplication", 0.3f);
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}
