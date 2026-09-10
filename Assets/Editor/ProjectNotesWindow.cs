#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ProjectNotesWindow : EditorWindow
{
    private string notes = "";

    [MenuItem("Window/General/Project Notes")]
    public static void ShowWindow()
    {
        GetWindow<ProjectNotesWindow>("Project Notes");
    }

    private void OnEnable()
    {
        notes = EditorPrefs.GetString("Unity_Project_Notes", "Type your notes here...");
    }

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        notes = EditorGUILayout.TextArea(notes, GUILayout.ExpandHeight(true));
        
        if (EditorGUI.EndChangeCheck())
        {
            EditorPrefs.SetString("Unity_Project_Notes", notes);
        }
    }
}
#endif