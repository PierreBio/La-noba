using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TitleScreenAnimationManager _titleScreenAnimationMgr;

    protected override void Awake()
    {
        base.Awake();
    }

    public void BackToMenuGame()
    {
        SceneManager.LoadScene("FirstScene");
        SoundManager.GetInstance().Play("main_theme", SoundManager.GetInstance().gameObject);
    }

    public void GoToEndMenuGame()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void PlayGame()
    {
        _titleScreenAnimationMgr.ChangeAnimationState("titlescreen_fadeout");
        float startGameDelay = _titleScreenAnimationMgr.GetCurrentAnimationDuration();

        Invoke("LoadGameScene", startGameDelay);
    }

    void LoadGameScene()
    {
        SceneManager.LoadScene("HichameScene");
    }
}
