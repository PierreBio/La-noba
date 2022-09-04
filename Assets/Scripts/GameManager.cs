using System;
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

    [SerializeField]
    private int m_nbAdditionalCharactersToShow; 
    
    public int TypingCharactersToShowPerFrame
    { 
        get => m_nbAdditionalCharactersToShow;
        set
        {
            m_nbAdditionalCharactersToShow = value;
            ChangeCharactersDisplaySpeed(m_nbAdditionalCharactersToShow);
        }
    }

    protected override void Awake()
    {
        base.Awake();
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
        if (TypingCharactersToShowPerFrame == 0)
        {
            yield break;
        }

        int previousTypingCharacterSpeed = m_nbAdditionalCharactersToShow;
        TypingCharactersToShowPerFrame = 0;

        yield return new WaitForSeconds(_secondsStopped);

        TypingCharactersToShowPerFrame = previousTypingCharacterSpeed;
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
        SceneManager.LoadScene("HichameScene");
    }

    private void OnValidate()
    {
        //ChangeCharactersDisplaySpeed(TypingCharactersToShowPerFrame);
    }

}
