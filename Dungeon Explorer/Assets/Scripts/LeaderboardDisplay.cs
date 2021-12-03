using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{
    public Text leaderboard;


    // Start is called before the first frame update
    void Start()
    {
        int size;
        if (!PlayerPrefs.HasKey("size"))
        {
            PlayerPrefs.SetInt("size", 0);
        }

        size = PlayerPrefs.GetInt("size");

        for (int i = size - 1; i >= 0; i--)
        {
            leaderboard.text += PlayerPrefs.GetString(i.ToString()) + "\n";
        }



    }

}