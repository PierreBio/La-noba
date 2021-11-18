using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public void BackToMenu()
    {
        GameManager.GetInstance().BackToMenuGame();
    }

    public void Play()
    {
        GameManager.GetInstance().PlayGame();
    }
}
