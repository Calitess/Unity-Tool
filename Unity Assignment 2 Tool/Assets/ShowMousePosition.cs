
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

    [Tooltip("Wall prefab goes here")]
    [SerializeField] public GameObject[] walls;

    private GameObject currentWall;

    private void Update()
    {
        if (creatingWall && currentWall != null)
        {
            Vector3 endPoint = GetMouseWorldPosition();
            float distance = Vector3.Distance(currentWall.transform.position, endPoint);
            currentWall.transform.localScale = new Vector3(1, 1, distance);
            currentWall.transform.LookAt(endPoint);
        }
    }

    public void CreateWall()
    {
        if (currentWall != null) return;

        Vector3 startPoint = GetMouseWorldPosition();
        currentWall = Instantiate(walls[0], startPoint, Quaternion.identity, transform);
    }

    public void FinishWall()
    {
        if (currentWall == null) return;

        currentWall = null;
    }

    private Vector3 GetMouseWorldPosition()
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


}
