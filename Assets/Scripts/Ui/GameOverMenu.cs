using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverMenu : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private GameObject gameOverMenuUI;

    [SerializeField]
    private GameObject inputField;

    [SerializeField]
    private Text finalScoreText;

    [SerializeField]
    private Text thanksText;

    [SerializeField]
    private Highscores highscore;

    private void Start()
    {
        highscore.NewHighscoreUploaded += Highscore_NewHighscoreUploaded;
    }

    private void Highscore_NewHighscoreUploaded(object sender, Highscores.NewHighscoreUploadedEventArgs e)
    {
        inputField.SetActive(false);

        thanksText.text = string.Format("Thanks, {0}", e.UserName);
        thanksText.gameObject.SetActive(true);
    }

    public void OnGameOver()
    {
        finalScoreText.text = player.Score.ToString();
        gameOverMenuUI.SetActive(true);
    }

    public void AddNewHighscore(string playerName)
    {
        highscore.AddNewHighscore(HighscoresMenu.FormatUserNameInput(playerName), player.Score);
    }
}
