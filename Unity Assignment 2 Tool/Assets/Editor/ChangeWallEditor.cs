
using UnityEngine;
using UnityEditor;

//editor to only show walltype
[CustomEditor(typeof(ChangeWall))]
[CanEditMultipleObjects]
public class ChangeWallEditor : Editor
{

    private SerializedProperty WallTypeProperty;



    private void OnEnable()
    {
        // Get the serialized field from the target object
        WallTypeProperty = serializedObject.FindProperty("WallType");

    }

    public override void OnInspectorGUI()
    {


        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();


        // Draw the fields for all the property
        EditorGUILayout.PropertyField(WallTypeProperty);


        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

}
