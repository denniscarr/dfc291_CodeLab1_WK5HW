using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    private static GameManager instance;

    int currentLevel = 0;

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        currentLevel = GetComponent<LevelSetup>().levelNumber;
    }

    void Update()
    {
        // Restart level.
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        // On skelly nope scenes, click to advance level.
        if (SceneManager.GetActiveScene().name.Contains("Skelly Nope") && Input.GetMouseButtonDown(0))
        {
            AdvanceScene();
        }
    }

    void AdvanceScene()
    {
        string nextScene = "";

        // If the current scene is a 'between level scene'
        if (SceneManager.GetActiveScene().name.Contains("Skelly Nope"))
        {
            if (currentLevel == 9) Application.Quit();

            currentLevel += 1;
            nextScene = "Level" + currentLevel;
        }

        // If it's a gameplay scene:
        else
        {
            nextScene = "Level" + currentLevel + " Skelly Nope";
        }

        SceneManager.LoadScene(nextScene);
    }
}
