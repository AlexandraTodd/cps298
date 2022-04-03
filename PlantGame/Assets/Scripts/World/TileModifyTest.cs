using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileModifyTest : MonoBehaviour
{
    private Tilemap tilemap;
    public Tile dirt;
    public Tile grass;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector3 pos = player.transform.position;
            pos.y -= player.GetComponent<SpriteRenderer>().bounds.size.y / 4;
            Vector3Int playerPos = tilemap.WorldToCell(pos);
            if (tilemap.GetTile(playerPos) == dirt) tilemap.SetTile(playerPos, grass);
            else tilemap.SetTile(playerPos, dirt);
        }
    }
}
