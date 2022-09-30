using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HoverTitleMenu : MonoBehaviour
{

    public void SetCursorArrow()
    {
        GameManager.GetInstance().GetComponent<CursorScript>().setCursorArrow();
    }

    public void SetCursorHover()
    {
        GameManager.GetInstance().GetComponent<CursorScript>().setCursorHover();
    }

}