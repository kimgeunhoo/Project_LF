using BSPDungeonGenrator.Config;
using BSPDungeonGenrator.Core;
using BSPDungeonGenrator.Generation;
using UnityEngine;
using UnityEngine.Tilemaps;


public class DoorController : MonoBehaviour
{
    [SerializeField] 
    private Collider2D doorCollider;
    [SerializeField]
    private SpriteRenderer doorSpriteRenderer;
    [SerializeField]
    private Sprite closedSprtie;
    [SerializeField]
    private Sprite openSprite;

    public Vector2Int GridPos
    { get; private set; }

    public int RoomId
    { get; private set; }

    public bool IsOpen
    { get; private set; }

    private void Awake()
    {
        if (doorCollider == null)
        {
            doorCollider = GetComponent<Collider2D>();
        }
        if (doorSpriteRenderer == null)
        {
            doorSpriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Init(Vector2Int gridPos, int roomId, bool startOpen = false)
    {
        GridPos = gridPos;
        RoomId = roomId;
        SetOpen(startOpen);
    }

    public void SetOpen(bool open)
    {
        IsOpen = open;

        if (doorCollider != null)
        {
            doorCollider.enabled = !open;
        }

        if (doorSpriteRenderer != null)
        {
            doorSpriteRenderer.sprite = open ? openSprite : closedSprtie;
        }
    }

}
