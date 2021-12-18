using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameLevel))]
public class GameLevelInspector : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var gameLEvel = (GameLevel)target;
        if (gameLEvel.HasMissingLevelObjects)
        {
            EditorGUILayout.HelpBox("Missing level objects", MessageType.Error);
            if (GUILayout.Button("Remove Missing Elements"))
            {
                Undo.RecordObject(gameLEvel, "Remove Missing Level Objects.");
                gameLEvel.RemoveMissingLevelObjects();
            }
        }
    }
}
