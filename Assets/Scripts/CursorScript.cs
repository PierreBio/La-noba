using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    public enum CursorType
    {
        ARROW,
        HOVER
    }

    public Texture2D cursorArrow;
    public Texture2D cursorHover;

    // Start is called before the first frame update
    void Start()
    {
        //set the cursor origin to its centre. (default is upper left corner)
        Vector2 cursorOffset = Vector2.zero;

        //Sets the cursor to the Crosshair sprite with given offset 
        //and automatic switching to hardware default if necessary
        Cursor.SetCursor(cursorArrow, cursorOffset, CursorMode.ForceSoftware);
    }

    public void setCursor(CursorType _type)
    {
        switch (_type)
        {
            case CursorType.ARROW:
                Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.ForceSoftware);
                break;
            case CursorType.HOVER:
                Cursor.SetCursor(cursorHover, Vector2.zero, CursorMode.ForceSoftware);
                break;
            default:
                break;
        }
    }
}
