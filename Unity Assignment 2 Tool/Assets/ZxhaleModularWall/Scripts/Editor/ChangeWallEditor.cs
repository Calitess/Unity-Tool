
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;

//editor to only show walltype
[CustomEditor(typeof(ChangeWall))]
[CanEditMultipleObjects]
public class ChangeWallEditor : Editor
{

    private SerializedProperty WallTypeProperty;
    private ChangeWall targetObject;


    private void OnEnable()
    {
        targetObject = target as ChangeWall;

        if (targetObject == null)
        {
            return;
        }

        // Get the serialized field from the target object
        WallTypeProperty = serializedObject.FindProperty("WallType");

    }

    public override void OnInspectorGUI()
    {


        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();


        // Draw the fields for all the property
        EditorGUILayout.PropertyField(WallTypeProperty);

        if (GUILayout.Button("Rotate 90Deg"))
        {
            foreach (var obj in Selection.gameObjects)
            {
                ChangeWall targetObject = obj.GetComponent<ChangeWall>();

                if (targetObject != null)
                {
                    for (int i = 0; i < targetObject.transform.childCount; i++)
                    {
                        Transform child = targetObject.transform.GetChild(i);
                        child.rotation *= Quaternion.Euler(0f, 90f, 0f);
                        targetObject.originalRot.Add(child.localRotation);
                    }

                    EditorCoroutineUtility.StartCoroutine(targetObject.RefreshRot(), targetObject);
                }
            }
        }


        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

}


