using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;

    private void Awake()
    {
        instance = this;
    }

    public void changeScene(int num)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(num);
    }
}
