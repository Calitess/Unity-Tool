using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ShowMousePosition))]
public class ShowPointerEditor : Editor
{
    private SerializedProperty creatingWallProperty,wallsProperty;
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
        wallsProperty = serializedObject.FindProperty("walls");

    }

    public override void OnInspectorGUI()
    {
        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();

        // Draw the bool field for creatingWall
        EditorGUILayout.PropertyField(creatingWallProperty);

        EditorGUILayout.PropertyField(wallsProperty, true);
      

        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        if (creatingWallProperty.boolValue == true)
        {
            // Draw the raycast pointer
            DrawRaycastPointer(GetMouseWorldPosition(), Vector3.down);
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

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

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return Vector3.zero;
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
            Event.current.Use();
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            targetObject.FinishWall();
            Event.current.Use();
        }
    }
}

