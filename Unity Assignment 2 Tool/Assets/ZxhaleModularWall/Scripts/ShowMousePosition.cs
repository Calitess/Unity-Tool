
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Alysha/Modular Wall",0)] // change the name of the script in th inspector
public class ShowMousePosition : MonoBehaviour
{


    // use mouse down event and mouse up event to know ehen the player is holding it down
    // while eit is being held down, and moving, instantiate prefab every 1 unit
    // also create another script for all the instantiated prefabs, this script holds what type of wall to display



    [Tooltip("When enabled, this will allow you to paint walls on an object. It will NOT allow you to select anything in the scene view")]
    [SerializeField] public bool creatingWall = true;

    [Tooltip("When enabled, this will allow you to paint walls on a grid. Best for straight walls.")]
    [SerializeField] private bool useGrid = false;

    [Tooltip("When enabled, this will allow you to undo (ctrl+z) the creation of individual walls. If disabled, it will undo creation of a wall group")]
    [SerializeField] private bool undoIndividualWalls = false;

    [Tooltip("When enabled, this will randomize what prefab to instantiate for the walls that are drawn.")]
    [SerializeField] private bool randomWall = false;

    [Tooltip("When enabled, this will let you draw prefabs on top of prefabs, best to create demolished walls/debris.")]
    [SerializeField] public bool considerRaycast = false;

    [Tooltip("Wall prefab goes here")]
    [SerializeField] public GameObject[] walls;

    [SerializeField] private GameObject currentWall,lastWall, secondLastWall,newWall, WallSegment;

    private Mesh mesh;
    public Vector3 direction;

    [Tooltip("When enabled, it used custom distance float")]
    [SerializeField] public bool useCustomDistance = false;

    [Tooltip("Custom distance between instantiated walls")]
    [SerializeField] private float customDistance, prefabDistance;




    public void CreateWall()
    {
        if (currentWall != null) return;

        if (useGrid)
        {
            // Create new undo group
            Undo.IncrementCurrentGroup();

            Vector3 startPoint = SnapPosition(GetMouseWorldPosition());

            WallSegment = new GameObject("WallSegment");
            WallSegment.transform.SetParent(transform);
            WallSegment.transform.position = startPoint;

            WallSegment.AddComponent<ChangeWall>();


            Undo.RegisterCreatedObjectUndo(WallSegment, "Create WallSegment");

            try
            {

                if (!randomWall)
                {
                    Vector3 offset = new Vector3(0, walls[0].transform.localScale.y * 0.5f, 0);
                    currentWall = Instantiate(walls[0], startPoint + offset, Quaternion.identity, WallSegment.transform);
                }
                else if (randomWall)
                {
                    int randomNum = Random.Range(0, walls.Length);
                    Vector3 offset = new Vector3(0, walls[randomNum].transform.localScale.y * 0.5f, 0);
                    currentWall = Instantiate(walls[randomNum], startPoint + offset, Quaternion.identity, WallSegment.transform);
                }

                Undo.RegisterCreatedObjectUndo(currentWall, "Create currentWall");
                Undo.SetTransformParent(currentWall.transform, WallSegment.transform, "Modify parent");

                // Name undo group
                Undo.SetCurrentGroupName("Create and Reposition GameObject with Child");
            }
            catch
            {
                Debug.LogWarning("Please put in a prefab in the walls array! Click 'Delete All Walls' to remove the empty wall segments");
            }

        }
        else if (!useGrid)
        {
            
            Undo.IncrementCurrentGroup();

            Vector3 startPoint = GetMouseWorldPosition();

            WallSegment = new GameObject("WallSegment");
            WallSegment.transform.SetParent(transform);
            WallSegment.transform.position = startPoint;

            WallSegment.AddComponent<ChangeWall>();


            Undo.RegisterCreatedObjectUndo(WallSegment, "Create WallSegment");


            try
            {
                if (!randomWall)
                {
                    Vector3 offset = new Vector3(0, walls[0].transform.localScale.y * 0.5f, 0);

                    currentWall = Instantiate(walls[0], startPoint + offset, Quaternion.identity, WallSegment.transform);
                }

                else if (randomWall)
                {
                    int randomNum = Random.Range(0, walls.Length);
                    Vector3 offset = new Vector3(0, walls[randomNum].transform.localScale.y * 0.5f, 0);
                    currentWall = Instantiate(walls[randomNum], startPoint + offset, Quaternion.identity, WallSegment.transform);
                }

                Undo.RegisterCreatedObjectUndo(currentWall, "Create currentWall");
                Undo.SetTransformParent(currentWall.transform, WallSegment.transform, "Modify parent");

                // Name undo group
                Undo.SetCurrentGroupName("Create and Reposition GameObject with Child");
            }
            catch
            {
                Debug.LogWarning("Please put in a prefab in the walls array! Click 'Delete All Walls' to remove the empty wall segments");
            }
            

        }


        lastWall = currentWall;
        //Debug.Log("Wall is starting");
    }

    public void ContinueWall()
    {
        if (currentWall == null) return;

        if (useGrid)
        {
           Vector3 curPoint = SnapPosition(GetMouseWorldPosition());
            
            if (!curPoint.Equals(currentWall.transform.position))
            {
                CreateWallSegment(curPoint);

            }
            
        }
        else if (!useGrid)
        {
            Vector3 curPoint = GetMouseWorldPosition();
            
                if (!curPoint.Equals(currentWall.transform.position))
                {
                    CreateWallSegment(curPoint);

                }
            
            //Debug.Log("Wall is finished");
        }

    }

    public void FinishWall()
    {
        if (currentWall == null) return;


        currentWall = null;
        lastWall = null;
        secondLastWall = null;
        newWall = null;

    }

   

    private void CreateWallSegment(Vector3 curPoint)
    {
        

        // apply the new offset value to the transform
        Vector3 offset = new Vector3(0, currentWall.transform.localScale.y * 0.5f, 0);

        float distance = Vector3.Distance(currentWall.transform.position , curPoint + offset);

        //Debug.Log(distance+"  "+ currentWall.transform.position + "  "+ curPoint);

        if (useCustomDistance)
        {
            prefabDistance = customDistance;
        }
        else
        {
            try
            {
                mesh = walls[0].GetComponent<MeshFilter>().sharedMesh;
                prefabDistance = mesh.bounds.size.z;
            }
            catch
            {
                Debug.LogWarning("Prefab is an empty gameObject with no mesh filter as it's parent, using custom distance.");

                useCustomDistance = true;
                customDistance = 1;
                prefabDistance = customDistance;


            }
        }
        

        if (distance >= prefabDistance)
        {
            if (undoIndividualWalls)
            {
                Undo.IncrementCurrentGroup();
            }


            WallSegment = new GameObject("WallSegment");
            WallSegment.transform.SetParent(transform);
            WallSegment.transform.position = curPoint;
            WallSegment.AddComponent<ChangeWall>();


            Undo.RegisterCreatedObjectUndo(WallSegment, "Create WallSegment");

            if (!randomWall)
            {
                

                newWall = Instantiate(walls[0], curPoint + offset, WallSegment.transform.rotation, WallSegment.transform);
              

                Undo.RegisterCreatedObjectUndo(newWall, "Create currentWall");
                Undo.SetTransformParent(newWall.transform, WallSegment.transform, "Modify parent");

                // Name undo group
                Undo.SetCurrentGroupName("Create and Reposition GameObject with Child");

                if (secondLastWall != null)
                {
                    lastWall = currentWall;
                    direction = newWall.transform.position - secondLastWall.transform.position;

                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    lastWall.transform.rotation = rotation;

                    //lastWall.transform.LookAt(currentWall.transform);
                    secondLastWall = lastWall;
                }
                else if (secondLastWall == null)
                {
                    lastWall.transform.LookAt(newWall.transform);
                    secondLastWall = lastWall;
                }

                newWall.transform.LookAt(currentWall.transform);
                currentWall = newWall;

                newWall.transform.forward = -newWall.transform.forward;
            }
            else if(randomWall)
            {
                int randomNum = Random.Range(0, walls.Length);

                 newWall = Instantiate(walls[randomNum], curPoint + offset, WallSegment.transform.rotation, WallSegment.transform);

                Undo.RegisterCreatedObjectUndo(newWall, "Create currentWall");
                Undo.SetTransformParent(newWall.transform, WallSegment.transform, "Modify parent");

                // Name undo group
                Undo.SetCurrentGroupName("Create and Reposition GameObject with Child");


                if (secondLastWall != null)
                {
                    lastWall = currentWall;
                    direction = newWall.transform.position - secondLastWall.transform.position;

                    Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);

                    lastWall.transform.rotation = rotation;

                    //lastWall.transform.LookAt(currentWall.transform);
                    secondLastWall = lastWall;
                }
                else if (secondLastWall == null)
                {
                    lastWall.transform.LookAt(newWall.transform);
                    secondLastWall = lastWall;
                }

                newWall.transform.LookAt(currentWall.transform);
                currentWall = newWall;

                newWall.transform.forward = -newWall.transform.forward;
            }







            //Debug.Log("Wall is continuing to be built");
        }
        
    }


    public Vector3 GetMouseWorldPosition()
    {
        //if (Event.current == null)
        //{
        //    return Vector3.down;
        //}

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            return hit.point;
        }
        else
        {
            return Vector3.down;
        }
    }


    public Vector3 SnapPosition(Vector3 original)
    {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x+0.5f);
        snapped.y = original.y;
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;

    }


    public void deleteAllChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }



    [MenuItem("Tool/Snap To Ground %g")]
    public static void Ground()
    {



        foreach (var transform in Selection.transforms)
        {


            var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 10f);

            foreach (var hit in hits)
            {


                if (hit.collider.gameObject == transform.gameObject)
                    continue;



                Renderer renderer = transform.gameObject.GetComponent<Renderer>();
                Vector3 offset = Vector3.zero;
                if (renderer != null)
                {
                    offset.y = renderer.bounds.size.y / 2;
                }

                transform.position = hit.point + offset;
                break;
            }
        }
    }

    [MenuItem("Tool/Select Parent &c")]
    static void SelectParentOfObjects()
    {
        List<GameObject> newSelection = new List<GameObject>();
        foreach (var s in Selection.objects)
        {
            newSelection.Add((s as GameObject).transform.parent?.gameObject);
        }
        Selection.objects = newSelection.ToArray();
    }

    [MenuItem("Tool/Select Child &v")]
    static void SelectChild()
    {
        List<GameObject> newSelection = new List<GameObject>();
        foreach (var s in Selection.gameObjects)
        {
            Transform[] children = s.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child.gameObject != s) // Exclude the parent object
                {
                    newSelection.Add(child.gameObject);
                }
            }
        }
        Selection.objects = newSelection.ToArray();
    }

    [MenuItem("Tool/Ground Transform &b")]
    static void SameParentOfObjects()
    {
        List<GameObject> newSelection = new List<GameObject>();
        foreach (var s in Selection.objects)
        {
            GameObject parentObject = (s as GameObject).transform.parent?.gameObject;
            if (parentObject != null)
            {
                newSelection.Add(parentObject);
                Transform childTransform = parentObject.transform.GetChild(0);
                childTransform.position = new Vector3(childTransform.position.x, parentObject.transform.position.y, childTransform.position.z); // Set child's y position to parent's y position
            }
        }
        Selection.objects = newSelection.ToArray();
    }




}
