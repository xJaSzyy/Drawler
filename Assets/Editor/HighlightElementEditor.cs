using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ButtonAnimation))]
public class HighlightElementEditor : Editor
{
    SerializedProperty highlightElementProp;
    SerializedProperty imageProp;
    SerializedProperty textProp;
    SerializedProperty colorProp;

    private void OnEnable()
    {
        highlightElementProp = serializedObject.FindProperty("highlightElement");
        imageProp = serializedObject.FindProperty("image");
        textProp = serializedObject.FindProperty("text");
        colorProp = serializedObject.FindProperty("color");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(highlightElementProp);

        if ((HighlightElement)highlightElementProp.enumValueIndex == HighlightElement.Image)
        {
            EditorGUILayout.PropertyField(imageProp);
            EditorGUILayout.PropertyField(colorProp);
        }
        else if ((HighlightElement)highlightElementProp.enumValueIndex == HighlightElement.Text)
        {
            EditorGUILayout.PropertyField(textProp);
            EditorGUILayout.PropertyField(colorProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
