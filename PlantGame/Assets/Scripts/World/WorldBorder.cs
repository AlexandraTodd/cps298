using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldBorder : MonoBehaviour
{
    public Tile empty;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap collisionMap = GetComponent<Tilemap>();
        float minY = -Camera.main.orthographicSize, maxY = Camera.main.orthographicSize;
        float minX = minY * Camera.main.aspect, maxX = maxY * Camera.main.aspect;
        Vector3Int min = collisionMap.WorldToCell(new Vector3(minX, minY, 0)), max = collisionMap.WorldToCell(new Vector3(maxX, maxY, 0));
        Debug.Log(min);
        Debug.Log(max);
        for (int x = min.x; x <= max.x; x++) {
            collisionMap.SetTile(new Vector3Int(x, min.y, 0), empty);
            collisionMap.SetTile(new Vector3Int(x, max.y, 0), empty);
        }
        for (int y = min.y + 1; y < max.y; y++) {
            collisionMap.SetTile(new Vector3Int(min.x, y, 0), empty);
            collisionMap.SetTile(new Vector3Int(max.x, y, 0), empty);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
