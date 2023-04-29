

using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ShowMousePosition))]
public class ShowPointerEditor : Editor
{
    private SerializedProperty creatingWallProperty,gridProperty,wallsProperty, undoProperty, randomProperty;
    private ShowMousePosition targetObject;



    private void OnEnable()
    {
        targetObject = target as ShowMousePosition;

        if (targetObject == null)
        {
            return;
        }

        // Get the serialized bool field from the target object
        creatingWallProperty = serializedObject.FindProperty("creatingWall");
        undoProperty = serializedObject.FindProperty("undoIndividualWalls");
        randomProperty = serializedObject.FindProperty("randomWall");
        gridProperty = serializedObject.FindProperty("useGrid");
        wallsProperty = serializedObject.FindProperty("walls");
        

    }

    public override void OnInspectorGUI()
    {
        

        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();


        // Draw the fields for all the property
        EditorGUILayout.PropertyField(creatingWallProperty);

        EditorGUILayout.PropertyField(undoProperty);

        EditorGUILayout.PropertyField(gridProperty);

        EditorGUILayout.PropertyField(randomProperty);

        EditorGUILayout.PropertyField(wallsProperty);

        //Make a delete button that deletes all the children
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

            GetMouseWorldPosition();

            // Repaint the scene view
            SceneView.RepaintAll();
        }
        else { }

    }

    public Vector3 GetMouseWorldPosition()
    {
        if (Event.current == null)
        {
            return Vector3.down;
        }

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {


            // Draw a line to represent the raycast
            Handles.color = Color.red;

            //Draw dotted line going up 
            Handles.DrawDottedLine(hit.point + new Vector3(0, 10, 0), hit.point, 10);

            // Draw a sphere to represent the end of the raycast pointer
            Handles.DrawWireDisc(hit.point, hit.normal, 0.5f);

            
                // Handle mouse events
                HandleMouseEvents();
            



            return hit.point;
        }
        else
        {
            return Vector3.down;
        }
    }

    private void HandleMouseEvents()
    {
        if (targetObject.walls != null && targetObject.walls.Length > 0)
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
        else
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {

                Debug.LogWarning("Please put in a prefab in the walls array!");

            }
        }
        
    }



}

