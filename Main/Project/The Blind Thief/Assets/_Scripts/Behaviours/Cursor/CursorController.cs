using UnityEngine;
using System.Collections;

public class CursorController : Singleton<CursorController>
{
    [SerializeField] private Texture2D cursorTexture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot; //The offset from the top left of the texture to use as the target point (must be within the bounds of the cursor).

    void Start()
    {
        hotSpot = new Vector2(cursorTexture.width/2,cursorTexture.height/2);
        SetToDefaultCursor();
    }

    void SetToDefaultCursor()
    {
        Cursor.SetCursor(cursorTexture,hotSpot,cursorMode);
    }
}
