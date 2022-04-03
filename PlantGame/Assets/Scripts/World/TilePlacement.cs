using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.Tilemaps;

public class TilePlacement : MonoBehaviour
{
    public Grid sprites;

    // Start is called before the first frame update
    void Start()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        TileBase[] spritemap = sprites.GetComponentInChildren<Tilemap>().GetTilesBlock(sprites.GetComponentInChildren<Tilemap>().cellBounds);
        Tile empty = null;
        foreach (Tile t in spritemap) if (t != null && t.name == "empty") {empty = t; break;}
        FileStream stream = new FileStream(Application.persistentDataPath + "/field.dat", FileMode.Open);
        if (!stream.CanRead) return;
        using (BinaryReader reader = new BinaryReader(stream)) {
            int width = reader.ReadUInt16(), height = reader.ReadUInt16();
            int minX = reader.ReadInt16(), minY = reader.ReadInt16();
            byte palsize = reader.ReadByte();
            Tile[] palette = new Tile[palsize];
            for (int i = 0; i < palsize; i++) {
                string name = reader.ReadString();
                bool found = false;
                foreach (Tile t in spritemap) if (t != null && t.name == name) {found = true; palette[i] = t; break;}
                if (!found) throw new InvalidDataException("Tile save file references tiles that do not exist");
            }
            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    byte b = reader.ReadByte();
                    if (b != 0xFF) tilemap.SetTile(new Vector3Int(minX + x, minY + y, 0), palette[b]);
                    else tilemap.SetTile(new Vector3Int(minX + x, minY + y, 0), empty);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy() {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        Vector3Int size = tilemap.cellBounds.size;
        Vector3Int min = tilemap.cellBounds.min;
        byte[,] map = new byte[size.y, size.x];
        string[] palette = new string[256];
        int palsize = 0;
        for (int y = 0; y < size.y; y++) {
            for (int x = 0; x < size.x; x++) {
                Sprite sprite = tilemap.GetSprite(new Vector3Int(min.x + x, min.y + y, 0));
                if (sprite == null) {
                    map[y, x] = 0xFF;
                    continue;
                }
                int idx = -1;
                for (int i = 0; i < palsize; i++) if (palette[i] == sprite.name) {idx = i; break;}
                if (idx == -1) {
                    if (palsize >= 255) throw new InternalBufferOverflowException("Too many tiles in the map!");
                    idx = palsize++;
                    palette[idx] = sprite.name;
                    Debug.Log(sprite.name);
                }
                map[y, x] = (byte)idx;
            }
        }

        FileStream stream = new FileStream(Application.persistentDataPath + "/field.dat", FileMode.OpenOrCreate);
        if (!stream.CanWrite) return;
        using (BinaryWriter writer = new BinaryWriter(stream)) {
            writer.Write((ushort)size.x);
            writer.Write((ushort)size.y);
            writer.Write((short)min.x);
            writer.Write((short)min.y);
            writer.Write((byte)palsize);
            for (int i = 0; i < palsize; i++) writer.Write(palette[i]);
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    writer.Write(map[y, x]);
                }
            }
        }
    }
}
