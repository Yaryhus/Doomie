using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public GameObject[] itemsToHide;
    public GameObject[] itemsToShow;

    #region Singleton
    public static LevelManager instance;
    bool playerDead;

    void Awake()
    {
        instance = this;
    }
    #endregion
    // Start is called before the first frame update

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayerDied()
    {
        foreach(GameObject i in itemsToHide)
        {
            i.SetActive(false);
        }

        foreach (GameObject i in itemsToShow)
        {
            i.SetActive(true);
        }

        playerDead = true;
        //Debug.Log("You died. Press R to Restart");
    }

    private void Update()
    {
        if (playerDead)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReloadScene();
            }
        }
    }
}
