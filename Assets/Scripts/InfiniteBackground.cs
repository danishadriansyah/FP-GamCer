using UnityEngine;
using UnityEngine.Tilemaps;

public class InfiniteBackground : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Tilemap Dimensions (in world units)")]
    public float tilemapWidth = 24f;   // total width of one tilemap
    public float tilemapHeight = 14f;  // total height of one tilemap

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference not assigned to InfiniteTilemap!");
        }

        // Optional: auto-detect width/height from TilemapRenderer bounds
        TilemapRenderer tr = GetComponent<TilemapRenderer>();
        if (tr != null)
        {
            Bounds bounds = tr.bounds;
            tilemapWidth = bounds.size.x;
            tilemapHeight = bounds.size.y;
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 pos = transform.position;
        Vector3 playerPos = player.position;

        // Move horizontally (X-axis)
        if (playerPos.x - pos.x > tilemapWidth * 1.5f)
            pos.x += tilemapWidth * 3f;
        else if (pos.x - playerPos.x > tilemapWidth * 1.5f)
            pos.x -= tilemapWidth * 3f;

        // Move vertically (Y-axis)
        if (playerPos.y - pos.y > tilemapHeight * 1.5f)
            pos.y += tilemapHeight * 3f;
        else if (pos.y - playerPos.y > tilemapHeight * 1.5f)
            pos.y -= tilemapHeight * 3f;

        transform.position = pos;
    }
}
