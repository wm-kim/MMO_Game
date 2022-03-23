using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// 개발 단계에서는 사용할 수 있지만, 그렇지 않으면 생성이 안됨
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor 
{
#if UNITY_EDITOR
    
    // 단축키
    // % (Ctrl) # (Shift) & (Alt)
    [MenuItem("Tools/GenerateMap %g")]
    private static void GenerateMap()
    {
        GenerateByPath("Assets/Resources/Map");
        GenerateByPath("../Common/MapData");
    }

    private static void GenerateByPath(string pathPrefix)
    {
        // ok button을 누르면 true로 설정
        //if(EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Canel"))
        //{
        //    new GameObject("Hello World");
        //}

        GameObject[] gameobjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameobjects)
        {
            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);

            // active한 object만 찾을 수 있음
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

            // 파일을 만들기 서버와 공유, 바이너리, text 형식(이걸 선택)
            using (var writer = File.CreateText($"{pathPrefix}/{go.name}.txt"))
            {
                writer.WriteLine(tmBase.cellBounds.xMin);
                writer.WriteLine(tmBase.cellBounds.xMax);
                writer.WriteLine(tmBase.cellBounds.yMin);
                writer.WriteLine(tmBase.cellBounds.yMax);

                for (int y = tmBase.cellBounds.yMax; y >= tmBase.cellBounds.yMin; y--)
                {
                    for (int x = tmBase.cellBounds.xMin; x <= tmBase.cellBounds.xMax; x++)
                    {
                        // 범위를 벗어난다고 해도 0으로 됨
                        TileBase tile = tm.GetTile(new Vector3Int(x, y, 0));
                        if (tile != null) writer.Write("1");
                        else writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }
    }

#endif
}
