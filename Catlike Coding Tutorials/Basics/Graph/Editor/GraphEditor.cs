using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Graph))]
public class EditorGraph : Editor{
    public override void OnInspectorGUI(){
        DrawDefaultInspector();
        var graphScript = (Graph) target;
        if (GUILayout.Button("Build Graph")) graphScript.BuildGraph();
    }
}