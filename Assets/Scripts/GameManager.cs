using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TitleScreenAnimationManager _titleScreenAnimationMgr;

    [SerializeField]
    private int m_nbAdditionalCharactersToShow;

    private int m_currentNbAdditionalCharactersToShow;

    private int m_nbAllCharactersDisplayed = 10000;

    public bool DisplayAllCharactersInstant
    {
        get => m_currentNbAdditionalCharactersToShow == m_nbAllCharactersDisplayed;
    }

    const string TITLE_SCENE = "FirstScene";
    const string GAME_SCENE = "HichameScene";


    public int CurrentTypingCharactersToShowPerFrame
    { 
        get => m_currentNbAdditionalCharactersToShow;
        set
        {
            m_currentNbAdditionalCharactersToShow = value;
            ChangeCharactersDisplaySpeed(m_currentNbAdditionalCharactersToShow);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        _titleScreenAnimationMgr = FindObjectOfType<TitleScreenAnimationManager>();
    }

    private void Start()
    {
        CurrentTypingCharactersToShowPerFrame = m_nbAdditionalCharactersToShow;
        SceneManager.activeSceneChanged += SetCurrentTitleScreenAnimationManager;
        EditorSceneManager.activeSceneChangedInEditMode += SetCurrentTitleScreenAnimationManager; // FOR EDITOR DEBUGGING
    }

    public event Action<int> onChangeCharactersDisplaySpeed;
    public void ChangeCharactersDisplaySpeed(int value)
    {
        if (onChangeCharactersDisplaySpeed != null)
        {
            onChangeCharactersDisplaySpeed(value);
        }
    }
    public IEnumerator StopTextDisplayForSeconds(float _secondsStopped)
    {
        if (CurrentTypingCharactersToShowPerFrame == 0)
        {
            yield break;
        }

        CurrentTypingCharactersToShowPerFrame = 0;

        yield return new WaitForSeconds(_secondsStopped);

        CurrentTypingCharactersToShowPerFrame = m_nbAdditionalCharactersToShow;
    }

    public void BackToMenuGame()
    {
        SceneManager.LoadScene(TITLE_SCENE);
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
        SceneManager.LoadScene(GAME_SCENE);
    }

    private void OnValidate()
    {
        //ChangeCharactersDisplaySpeed(TypingCharactersToShowPerFrame);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
        {
            CurrentTypingCharactersToShowPerFrame = m_nbAllCharactersDisplayed;
        }
    }

    public void SetBackTextDisplaySpeed()
    {
        CurrentTypingCharactersToShowPerFrame = m_nbAdditionalCharactersToShow;
        StopCoroutine("StopTextDisplayForSeconds");
    }

    public void SetCurrentTitleScreenAnimationManager(Scene current, Scene next)
    {
        _titleScreenAnimationMgr = FindObjectOfType<TitleScreenAnimationManager>();
    }

}
