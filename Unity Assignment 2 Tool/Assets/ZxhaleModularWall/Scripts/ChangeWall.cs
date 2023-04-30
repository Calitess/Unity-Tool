
using UnityEngine;
using System.Collections.Generic;

using System.Collections;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[AddComponentMenu("Alysha/WallType", 0)] // change the name of the script in th inspector
public class ChangeWall : MonoBehaviour
{
    
    [SerializeField] GameObject curWallType;

    [Tooltip("You can change wall type by dragging and dropping new prefab in here!")]
    [SerializeField] public GameObject WallType;

    //saving all the modified rotations
    [SerializeField] public List <Quaternion> originalRot = new List<Quaternion> ();

    public bool refreshRotation;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            
            //get the instantiated wall and asign it to curwalltype
            curWallType = transform.GetChild(0).gameObject;

            //assign curwalltype to wall type so that player can see the wall type
            WallType = curWallType;

            StartCoroutine(RefreshRot());
           

        }
        catch 
        {
            Debug.LogWarning("You have not put in a prefab. Click 'Delete All Walls' button to delete empty wall segments");
        }

        

        
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (refreshRotation)
            {

                if (originalRot.Count <= 2)
                {
                    Transform t = transform.GetChild(0); // assuming the cube is the first child
                    originalRot.Add(t.localRotation);
                    //Debug.Log(t.localRotation);
                }
                else
                {
                    originalRot.RemoveAt(0);
                }

            }

                //if walltype isnt empty and wall type is different from cur wall type, change it
                if (WallType != null && WallType != curWallType)
                {
                    ChangeWallType();
                    //Debug.Log(" changing wall type ");
                }
            
        }
        catch
        {
            Debug.LogWarning("You have not put in a prefab. Click 'Delete All Walls' button to delete empty wall segments");
        }
    }


    private void ChangeWallType()
    {

        //Debug.Log(" changing wall type void entered ");

        if (WallType != curWallType)
        {
                WallType.transform.rotation = curWallType.transform.rotation;

                //Debug.Log(WallType.transform.rotation + " = " + curWallType.transform.rotation);

                DestroyImmediate(transform.GetChild(0).gameObject);
                
                Vector3 offset = new Vector3(0, WallType.transform.localScale.y * 0.5f, 0);

                Quaternion oriRot = originalRot[originalRot.Count - 1];


                Instantiate(WallType, transform.position + offset, oriRot, transform);

                curWallType = WallType;


        }

    }

    public IEnumerator RefreshRot()
    {
        yield return new WaitForSecondsRealtime(0f);
        refreshRotation = true;
        yield return new WaitForSecondsRealtime(1f);
        refreshRotation = false;


    }
}
