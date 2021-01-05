using UnityEngine;

public sealed class BoardSwitch : MonoBehaviour
{
    [SerializeField]
    private Pulsator pulsator;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Shaker cameraShaker;

    [SerializeField]
    private AudioManager audioManager;

    [SerializeField]
    private Board board;

    private AudioSource backgroundMusic;

    private bool isOn = true;
    private bool gameOver = false;

    private float timeAcc = 0.0f;
    private float timeOff = 0.0f;

    private void Start()
    {
        backgroundMusic = audioManager.CreateAudioSource("BackgroundMusic");
    }

    public void OnGameOver()
    {
        pulsator.enabled = false;
        backgroundMusic.pitch = 0.5f;
        isOn = false;
        gameOver = true;
    }

    public void SetOn(bool on)
    {
        if (!gameOver && isOn != on)
        {
            isOn = on;

            animator.SetBool("isOn", isOn);
            pulsator.enabled = isOn;
            cameraShaker.Shake();

            if (!isOn)
            {
                timeOff = Random.Range(2.0f, 8.0f);
                timeAcc = 0.0f;
                audioManager.CreateTemporaryAudioSource("BoardSwitchOff");
                backgroundMusic.pitch = 0.5f;
            }
            else
            {
                audioManager.CreateTemporaryAudioSource("BoardSwitchOn");
                backgroundMusic.pitch = 1.0f;
                board.SpeedUp();
            }
        }
    }

    public bool IsOn()
    {
        return isOn;
    }

    private void Update()
    {
        if (!isOn && !gameOver)
        {
            timeAcc += Time.deltaTime;

            if (timeAcc >= timeOff)
            {
                SetOn(true);
            }
        }
    }
}
