using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

using TMPro.Examples;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TitleScreenAnimationManager _titleScreenAnimationMgr;

    private TransitionAnimationManager _transitionAnimationMgr;

    [SerializeField]
    private int m_nbAdditionalCharactersToShow;

    private int m_currentNbAdditionalCharactersToShow;

    private int m_nbAllCharactersDisplayed = 10000;

    [SerializeField] float m_stopClickChoiceCooldown; // in seconds

    public bool m_changeNodeEnable;

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
        m_changeNodeEnable = true;
        CurrentTypingCharactersToShowPerFrame = m_nbAdditionalCharactersToShow;
#if UNITY_EDITOR
        EditorSceneManager.activeSceneChangedInEditMode += SetCurrentTitleScreenAnimationManager; // FOR EDITOR DEBUGGING
#else
        SceneManager.activeSceneChanged += SetCurrentTitleScreenAnimationManager;
#endif
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
            TextConsoleSimulator txtConsoleSim = FindObjectOfType<TextConsoleSimulator>();

            bool allCharactersAreDisplayed = txtConsoleSim != null && txtConsoleSim.AllCharactersAreDisplayed() == false;
            bool canCancelChangeChoice = CurrentTypingCharactersToShowPerFrame != m_nbAllCharactersDisplayed && allCharactersAreDisplayed;

            if (canCancelChangeChoice)
            {
                SwitchChangeNodeEnable();
                Invoke("SwitchChangeNodeEnable", m_stopClickChoiceCooldown);
                Debug.Log(m_changeNodeEnable);
            }

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

        if (next.name == TITLE_SCENE && _titleScreenAnimationMgr != null)
        {
            _titleScreenAnimationMgr.ChangeAnimationState("titlescreen_fadein");
        }

        if (next.name == GAME_SCENE)
        {
            _transitionAnimationMgr = FindObjectOfType<TransitionAnimationManager>();
        }
    }

    public void TriggerBackToTitleMenu()
    {
        _transitionAnimationMgr.ChangeAnimationState("game_fadeout");
        float animationTime = _transitionAnimationMgr.GetCurrentAnimationDuration();

        Invoke("BackToMenuGame", animationTime);
    }

    private void SwitchChangeNodeEnable()
    {
        m_changeNodeEnable = !m_changeNodeEnable;
    }
}
