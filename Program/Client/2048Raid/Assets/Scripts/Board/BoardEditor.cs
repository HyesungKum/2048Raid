#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    private Board board = null;

    private void OnEnable()
    {
        board = target as Board;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //create button
        if (GUILayout.Button("Create Board"))
        {
            board.CreateNodes();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif