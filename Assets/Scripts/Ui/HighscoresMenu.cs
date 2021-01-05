using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class HighscoresMenu : MonoBehaviour
{
    [SerializeField]
    private Highscores highscores;

    [SerializeField]
    private List<Entry> entries;

    private void Start()
    {
        highscores.HighscoresDownloaded += Highscores_HighscoresDownloaded;
    }

    private void OnEnable()
    {
        foreach (var e in entries)
        {
            e.name.text = "...";
            e.score.text = "...";
        }

        highscores.DownloadHighscores();
    }

    private void Highscores_HighscoresDownloaded(object sender, Highscores.HighscoresDownloadedEventArgs e)
    {
        for (var i = 0; i < Mathf.Min(entries.Count, e.Highscores.Count); ++i)
        {
            entries[i].name.text = FormatUserNameInput(e.Highscores[i].UserName);
            entries[i].score.text = FormatTextInput(e.Highscores[i].Score.ToString(), 20);
        }
    }

    public static string FormatUserNameInput(string text)
    {
        return FormatTextInput(text, 10);
    }

    private static string FormatTextInput(string text, int maxSize)
    {
        return text.Substring(0, Mathf.Min(maxSize, text.Length));
    }

    [System.Serializable]
    public sealed class Entry
    {
        public Text name;
        public Text score;
    }
}
