using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] 
    private Texture2D cursorTexture;
    [SerializeField]
    private Vector2 hotSpot = Vector2.zero;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    private void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

}
