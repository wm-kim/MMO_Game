using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

// ���� �ܰ迡���� ����� �� ������, �׷��� ������ ������ �ȵ�
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor 
{
#if UNITY_EDITOR
    
    // ����Ű
    // % (Ctrl) # (Shift) & (Alt)
    [MenuItem("Tools/GenerateMap %g")]
    private static void GenerateMap()
    {
        GenerateByPath("Assets/Resources/Map");
        GenerateByPath("../Common/MapData");
    }

    private static void GenerateByPath(string pathPrefix)
    {
        // ok button�� ������ true�� ����
        //if(EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Canel"))
        //{
        //    new GameObject("Hello World");
        //}

        GameObject[] gameobjects = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (GameObject go in gameobjects)
        {
            Tilemap tmBase = Util.FindChild<Tilemap>(go, "Tilemap_Base", true);

            // active�� object�� ã�� �� ����
            Tilemap tm = Util.FindChild<Tilemap>(go, "Tilemap_Collision", true);

            // ������ ����� ������ ����, ���̳ʸ�, text ����(�̰� ����)
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
                        // ������ ����ٰ� �ص� 0���� ��
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
