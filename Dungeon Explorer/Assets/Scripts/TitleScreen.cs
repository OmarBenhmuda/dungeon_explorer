using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private GameObject _leaderboard;

    public void Continue()
    {
        string scene = PlayerPrefs.GetString("lastPlayed");
        SceneManager.LoadScene(scene);
    }

    public void Level1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void OpenLeaderboard()
    {
        _leaderboard.SetActive(true);
        _menu.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void GoBack()
    {
        _leaderboard.SetActive(false);
        _menu.SetActive(true);
    }
}
