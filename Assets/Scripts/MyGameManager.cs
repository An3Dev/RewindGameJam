using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour
{
    public static Transform player;

    public static int currentLevel = 0;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Start()
    {
        currentLevel = SceneManager.GetActiveScene().buildIndex + 1;
        Debug.Log(currentLevel);
    }

    public static void GoToNextLevel()
    {
        //SceneManager.LoadScene("Level_" + (currentLevel + 1));

        if (SceneManager.sceneCountInBuildSettings >= (currentLevel + 1))
        {
            SceneManager.LoadScene("Level_" + (currentLevel + 1));
        }
        else
        {
            // load menu because player beat the games
            SceneManager.LoadScene(0);

        }

    }
}
