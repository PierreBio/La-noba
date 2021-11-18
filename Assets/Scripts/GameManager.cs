using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private GameObject titleScreen;

    [SerializeField]
    private GameObject gameScreen;

    protected override void Awake()
    {
        base.Awake();
    }

    public void BackToMenuGame()
    {
        SceneManager.LoadScene("FirstScene");
    }

    public void GoToEndMenuGame()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
