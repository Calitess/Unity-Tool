
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using Unity.VisualScripting;

[ExecuteInEditMode]
[AddComponentMenu("Alysha/WallType", 0)] // change the name of the script in th inspector
public class ChangeWall : MonoBehaviour
{
    
    [SerializeField] GameObject curWallType;

    [Tooltip("You can change wall type by dragging and dropping new prefab in here!")]
    [SerializeField] GameObject WallType;

    //saving all the modified rotations
    [SerializeField] private List <Quaternion> originalRot = new List<Quaternion> ();

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            //get the instantiated wall and asign it to curwalltype
            curWallType = transform.GetChild(0).gameObject;

            //assign curwalltype to wall type so that player can see the wall type
            WallType = curWallType;
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
            if (originalRot.Count <= 1)
            {
                Transform t = transform.GetChild(0); // assuming the cube is the first child
                originalRot.Add(t.localRotation);
            }

            //if walltype isnt empty and wall type is different from cur wall type, change it
            if (WallType != null && WallType != curWallType)
            {
                ChangeWallType();
                Debug.Log(" changing wall type ");
            }
        }
        catch
        {
            Debug.LogWarning("You have not put in a prefab. Click 'Delete All Walls' button to delete empty wall segments");
        }
    }


    private void ChangeWallType()
    {

        Debug.Log(" changing wall type void entered ");

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
}
