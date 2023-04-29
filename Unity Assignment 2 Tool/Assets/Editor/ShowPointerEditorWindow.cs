using UnityEditor;
using UnityEngine;

public class ShowPointerEditorWindow : EditorWindow
{
    //giving player the ability to instatiate a container with creating wall straight away
    public bool creatingWallBool = false;

    //So tht player can choose n open window from menu bar
    [MenuItem("Tool/Modular Wall")]

    //opens the window(what happens when player click on it in the menu bar)
    public static void Open()
    {
        ShowPointerEditorWindow window = GetWindow<ShowPointerEditorWindow>();
        window.titleContent = new GUIContent("Modular Wall");
        window.Show();
    }

    private void OnGUI()
    {
        //Shows tooltip in the editor window
        GUIContent content = new GUIContent("Start creating wall", "When enabled, this will allow you to paint walls straight away after clicking 'Create Wall Container' button");

        //Make a toggle bool
        creatingWallBool = EditorGUILayout.Toggle(content, creatingWallBool);

        //Make a button that creates the container
        if (GUILayout.Button("Create Wall Container"))
        {
            GameObject go = new GameObject("Wall Container");

            //attaching a script on the wall container
            go.AddComponent<ShowMousePosition>();

            //getting the bool from the window 
            ShowMousePosition myScript = go.GetComponent<ShowMousePosition>();

            //assigning the bool to the script
            myScript.creatingWall = creatingWallBool;

            //automatically select wallcontainer in the hierarchy
            Selection.activeGameObject = go;
        }


        // Save the state of the toggle in EditorPrefs
        EditorPrefs.SetBool("CreatingWall", creatingWallBool);
    }

    private void OnEnable()
    {
        // Load the state of the toggle from EditorPrefs
        creatingWallBool = EditorPrefs.GetBool("CreatingWall", false);
    }
}
