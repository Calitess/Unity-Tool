
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShowMousePosition))]
public class ShowPointerEditor : Editor
{
    private SerializedProperty creatingWallProperty,gridProperty,wallsProperty, undoProperty, randomProperty, raycastProperty, useCustomDistanceProperty, customDistanceProperty;
    private ShowMousePosition targetObject;
    private bool showTips = true;



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
        raycastProperty = serializedObject.FindProperty("considerRaycast");
        useCustomDistanceProperty = serializedObject.FindProperty("useCustomDistance");
        customDistanceProperty = serializedObject.FindProperty("customDistance");


    }

    public override void OnInspectorGUI()
    {
        

        // Update the serialized object with any changes made in the inspector
        serializedObject.Update();

        EditorGUILayout.LabelField("Modular Wall Tool", EditorStyles.boldLabel) ;


        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Wall Creation", EditorStyles.boldLabel);



        // Draw the fields for all the property
        EditorGUILayout.PropertyField(creatingWallProperty);

        EditorGUILayout.PropertyField(gridProperty);

        EditorGUILayout.PropertyField(randomProperty);

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Custom Properties", EditorStyles.boldLabel);


        EditorGUILayout.PropertyField(undoProperty);

        EditorGUILayout.PropertyField(raycastProperty);

        EditorGUILayout.PropertyField(useCustomDistanceProperty);

        if (targetObject.useCustomDistance)
        {

            EditorGUILayout.PropertyField(customDistanceProperty);
        }



        EditorGUILayout.Space(20);

        if (GUILayout.Button("Remove All Prefabs"))
        {
            targetObject.walls = null;
        }

        EditorGUILayout.Space();

        //Make a delete button that deletes all the children
        if (GUILayout.Button("Delete All Walls"))
        {
            targetObject.deleteAllChildren();
        }
        EditorGUILayout.Space();

        if (GUILayout.Button("Make Wall Segment to Default Layer"))
        {
            targetObject.DefaultLayer();
           
        }
        EditorGUILayout.Space();

        showTips = EditorGUILayout.BeginToggleGroup("Tips", showTips);
        if (showTips) // only show the tips if the toggle is set to true
        {
            GUIStyle italic = new GUIStyle(GUI.skin.label);
            italic.fontStyle = FontStyle.Italic;

            GUILayout.Label("Hover over elements to get more info", italic);

            EditorGUILayout.Space();

            GUILayout.Label("Alt-C to select parents", italic); 
            GUILayout.Label("Alt-V to select child", italic);
            GUILayout.Label("Alt-B to ground transform for walls created", italic);
            GUILayout.Label("Ctrl-G to ground selection", italic);
            
        }
        EditorGUILayout.EndToggleGroup();

        EditorGUILayout.Space(20);



        EditorGUILayout.PropertyField(wallsProperty);

        try
        {
            if (targetObject.walls != null && targetObject.walls.Length > 0)
            {
                if (targetObject.considerRaycast == false)
                {
                    for (int i = 0; i < targetObject.walls.Length; i++)
                    {
                        List<Transform> children = new List<Transform>();
                        Stack<Transform> tempList = new Stack<Transform>();

                        tempList.Push(targetObject.walls[i].transform);

                        do
                        {
                            Transform t = tempList.Pop();
                            children.Add(t);
                            for(int n = 0; n < t.childCount; n++)
                            {
                                tempList.Push(t.GetChild(n));
                            }

                        } while (tempList.Count > 0);

                        foreach(Transform child in children)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                        }

                        //targetObject.walls[i].layer = LayerMask.NameToLayer("Ignore Raycast");
                    }
                }
                else if (targetObject.considerRaycast == true)
                {
                    for (int i = 0; i < targetObject.walls.Length; i++)
                    {
                        List<Transform> children = new List<Transform>();
                        Stack<Transform> tempList = new Stack<Transform>();

                        tempList.Push(targetObject.walls[i].transform);

                        do
                        {
                            Transform t = tempList.Pop();
                            children.Add(t);
                            for (int n = 0; n < t.childCount; n++)
                            {
                                tempList.Push(t.GetChild(n));
                            }

                        } while (tempList.Count > 0);

                        foreach (Transform child in children)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Default");
                        }
                        //targetObject.walls[i].layer = LayerMask.NameToLayer("Default");
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("Please put in a prefab in the walls array!");
        }




        // Apply any changes made to the serialized object
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        serializedObject.Update();

        if (creatingWallProperty.boolValue == true)
        {

            //Make everything else in scene not selectable
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            GetMouseWorldPosition();

            // Repaint the scene view
            SceneView.RepaintAll();
        }
        else { }




        serializedObject.ApplyModifiedProperties();

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

