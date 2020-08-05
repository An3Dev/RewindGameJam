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
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public static void GoToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetSceneByBuildIndex(currentLevel+1).name);
    }
}
