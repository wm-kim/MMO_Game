using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���� �ܰ迡���� ����� �� ������, �׷��� ������ ������ �ȵ�
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor 
{
#if UNITY_EDITOR

    // ����Ű
    // % (Ctrl) # (Shift) & (Alt)
    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        // ok button�� ������ true�� ����
        if(EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Canel"))
        {
            new GameObject("Hello World");
        }
    }



#endif
}
