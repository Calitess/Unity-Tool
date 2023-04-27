
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class ShowMousePosition : MonoBehaviour
{


    // use mouse down event and mouse up event to know ehen the player is holding it down
    // while eit is being held down, and moving, instantiate prefab every 1 unit
    // also create another script for all the instantiated prefabs, this script holds what type of wall to display



    [Tooltip("When enabled, this will allow you to paint walls on an object. It will NOT allow you to select anything in the scene view")]
    [SerializeField] public bool creatingWall = true;

    [SerializeField] private bool useGrid = false;

    [Tooltip("Wall prefab goes here")]
    [SerializeField] private GameObject[] walls;

    [SerializeField] private GameObject currentWall,lastWall;


    public void CreateWall()
    {
        if (currentWall != null) return;

        if (useGrid)
        {
            Vector3 startPoint = SnapPosition(GetMouseWorldPosition());
            Vector3 offset = new Vector3(0, walls[0].transform.localScale.y * 0.5f, 0);
            currentWall = Instantiate(walls[0], startPoint + offset, Quaternion.identity, transform);
        }
        else if (!useGrid)
        {
            Vector3 startPoint = GetMouseWorldPosition();
            Vector3 offset = new Vector3(0, walls[0].transform.localScale.y * 0.5f, 0);
            currentWall = Instantiate(walls[0], startPoint + offset, Quaternion.identity, transform);
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
        }

    }

    public void FinishWall()
    {
        if (currentWall == null) return;

        currentWall = null;

        //Debug.Log("Wall is finished");
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
            return hit.point;
        }
        else
        {
            return Vector3.down;
        }
    }

    private void CreateWallSegment(Vector3 curPoint)
    {

        // apply the new offset value to the transform
        Vector3 offset = new Vector3(0, currentWall.transform.localScale.y * 0.5f, 0);

        float distance = Vector3.Distance(currentWall.transform.position , curPoint + offset);
        //Debug.Log(distance+"  "+ currentWall.transform.position + "  "+ curPoint);

        
        if (distance >= currentWall.transform.localScale.z-0.05f)
        {




            GameObject newWall = Instantiate(walls[0], curPoint + offset, Quaternion.identity, transform);


            lastWall.transform.LookAt(currentWall.transform);
            currentWall.transform.LookAt(newWall.transform);
            lastWall = currentWall;
            newWall.transform.LookAt(currentWall.transform);
            currentWall = newWall;



            //Debug.Log("Wall is continuing to be built");
        }
        
    }

    [MenuItem("Tool/Ground Selection  %g")] //bonus tool that allows users to snap any hierarchy selection to the ground! Yay!
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

    public void deleteAllChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    public Vector3 SnapPosition(Vector3 original)
    {
        Vector3 snapped;
        snapped.x = Mathf.Floor(original.x + 0.5f);
        snapped.y = Mathf.Floor(original.y + 0.5f);
        snapped.z = Mathf.Floor(original.z + 0.5f);
        return snapped;

    }


}
