using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    void Start()
    {
        text.text = "You survived " + PlayerPrefs.GetInt("WavesCompleted", 0) + " wave\n" +
                    "and killed " + PlayerPrefs.GetInt("ZombiesKilled", 0) + " zombies";
    }

    public void Retry()
    {
        SceneManager.LoadScene("MainScene");
    }
}
