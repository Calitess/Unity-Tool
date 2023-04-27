

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShowMousePosition))]
public class ShowPointerEditor : Editor
{
    private SerializedProperty creatingWallProperty,gridProperty,wallsProperty;
    private ShowMousePosition targetObject;



    private void OnEnable()
    {
        targetObject = target as ShowMousePosition;

        if (targetObject == null)
        {
            return;
        }

        //if (target == null)
        //{
        //    return;
        //}

        // Get the serialized bool field from the target object
        creatingWallProperty = serializedObject.FindProperty("creatingWall");
        gridProperty = serializedObject.FindProperty("useGrid");
        wallsProperty = serializedObject.FindProperty("walls");
        

    }

    public override void OnInspectorGUI()
    {
        

        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();

        // Draw the bool field for creatingWall
        EditorGUILayout.PropertyField(creatingWallProperty);

        EditorGUILayout.PropertyField(gridProperty);

        EditorGUILayout.PropertyField(wallsProperty, true);

        if (GUILayout.Button("Delete All Walls"))
        {
            targetObject.deleteAllChildren();
        }


        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (creatingWallProperty.boolValue == true)
        {

            //Make everything else in scene not selectable
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            if (gridProperty.boolValue == true)
            {
                Vector3 snappingPos = targetObject.SnapPosition(targetObject.GetMouseWorldPosition());

                //Debug.Log(snappingPos);

                // Draw the raycast pointer
                DrawRaycastPointer(snappingPos, Vector3.down);
            }
            else if (gridProperty.boolValue == false)
            {
                Vector3 snappingPos = targetObject.GetMouseWorldPosition();

                //Debug.Log(snappingPos);

                // Draw the raycast pointer
                DrawRaycastPointer(snappingPos, Vector3.down);
            }

            // Handle mouse events
            HandleMouseEvents();

            // Repaint the scene view
            SceneView.RepaintAll();
        }
        else { }

    }

    private void DrawRaycastPointer(Vector3 position, Vector3 direction)
    {
        // Perform a raycast in the scene
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // Draw a line to represent the raycast
            Handles.color = Color.red;

            //Draw dotted line going up 
            Handles.DrawDottedLine(position + new Vector3(0, 10, 0), hit.point, 10);

            // Draw a sphere to represent the end of the raycast pointer
            Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);
        }
    }


    private void HandleMouseEvents()
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            targetObject.CreateWall();
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
        {
            targetObject.ContinueWall();
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            targetObject.FinishWall();
            Event.current.Use();
        }
    }

}

