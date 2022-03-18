using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// map load 삭제 역할, load가 되었으면 collision file 추출
public class MapManager
{
    public Grid CurrentGrid { get; private set; }

    public int MinX { get; set; }
    public int MaxX { get; set; }
    public int MinY { get; set; }
    public int MaxY { get; set; }

    bool[,] _collision;

    // destpos가 넘어옴
    public bool CanGo(Vector3Int cellPos)
    {
        if (cellPos.x < MinX || cellPos.x >= MaxX)
            return false;
        if (cellPos.y < MinY || cellPos.y >= MaxY)
            return false;

        int x = cellPos.x - MinX;
        int y = MaxY - cellPos.y; // 위쪽으로 갈수록 y가 높아짐
        return !_collision[y, x];
    }

    public void LoadMap(int mapId)
    {
        DestroyMap(); // 기존에 있었던것 삭제

        string mapName = "Map_" + mapId.ToString("000"); // formatting, 1일경우에 001이됨
        GameObject go = Managers.Resource.Instantiate($"Map/{mapName}");
        go.name = "Map";

        // 꺼져 있는 object를 찾는건 까다롭기때문에 Tilemap_Collision은 active true인 상태에서 시작한다.
        GameObject collision = Util.FindChild(go, "Tilemap_Collision", true);
        if (collision != null)
        {
            collision.SetActive(false);
        }

        CurrentGrid = go.GetComponent<Grid>();

        // Collision 관련 파일 .txt하면 안됨
        TextAsset txt = Managers.Resource.Load<TextAsset>($"Map/{mapName}");
        // parsing 문장단위로 읽을 때 편리
        StringReader reader = new StringReader(txt.text);

        MinX = int.Parse(reader.ReadLine());
        MaxX = int.Parse(reader.ReadLine());
        MinY = int.Parse(reader.ReadLine());
        MaxY = int.Parse(reader.ReadLine());
        
        int xCount = MaxX - MinX + 1;
        int yCount = MaxY - MinY + 1;
        _collision = new bool[yCount, xCount];

        for(int y = 0; y < yCount; y++)
        {
            string line = reader.ReadLine();
            for(int x = 0; x < xCount; x++)
            {
                _collision[y, x] = (line[x] == '1' ? true : false);
            }
        }
    }

    public void DestroyMap()
    {
        GameObject map = GameObject.Find("Map");
        if(map != null)
        {
            GameObject.Destroy(map);
            CurrentGrid = null;
        }
    }
}
