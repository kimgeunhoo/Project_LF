using UnityEngine;

public class PlayerSeePoint : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private Transform visualRoot;

    private void Awake()
    {
        playerCamera = Camera.main;
        visualRoot = transform;
    }

    private void Update()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = playerCamera.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 myPos = transform.position;
        float dirX = mouseWorldPos.x - myPos.x;

        if (dirX > 0.01f)
        {
            visualRoot.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (dirX < -0.01f)
        {
            visualRoot.localScale = new Vector3(1f, 1f, 1f);

        }
    }

}
