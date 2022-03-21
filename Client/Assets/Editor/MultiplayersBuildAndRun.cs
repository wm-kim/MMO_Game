using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun 
{
    [MenuItem("Tools/Run Mutiplayer/2 Players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }
    [MenuItem("Tools/Run Mutiplayer/3 Players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);

    }
    [MenuItem("Tools/Run Mutiplayer/4 Players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);

    }


    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone ,BuildTarget.StandaloneWindows);

        for (int i = 0; i < playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(), "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe"
                , BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer); // 만들자 마자 자동실행을 하라는 옵션
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length]; 
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
