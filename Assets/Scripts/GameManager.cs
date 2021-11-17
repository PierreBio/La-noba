using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void PlayGame()
    {
        titleScreen.SetActive(false);
        gameScreen.SetActive(true);
    }
}
