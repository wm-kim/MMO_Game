using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 개발 단계에서는 사용할 수 있지만, 그렇지 않으면 생성이 안됨
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor 
{
#if UNITY_EDITOR

    // 단축키
    // % (Ctrl) # (Shift) & (Alt)
    [MenuItem("Tools/GenerateMap %#g")]
    private static void GenerateMap()
    {
        // ok button을 누르면 true로 설정
        if(EditorUtility.DisplayDialog("Hello World", "Create?", "Create", "Canel"))
        {
            new GameObject("Hello World");
        }
    }



#endif
}
