
using UnityEngine;


[ExecuteInEditMode]
[AddComponentMenu("Alysha/WallType", 0)] // change the name of the script in th inspector
public class ChangeWall : MonoBehaviour
{
    
    [SerializeField]GameObject curWallType;

    [Tooltip("You can change wall type by dragging and dropping new prefab in here!")]
    [SerializeField] GameObject WallType;



    // Start is called before the first frame update
    void Start()
    {
        //get the instantiated wall and asign it to curwalltype
        curWallType = transform.GetChild(0).gameObject;

        //assign curwalltype to wall type so that player can see the wall type
        WallType = curWallType;
    }

    // Update is called once per frame
    void Update()
    {
        //if walltype isnt empty and wall type is different from cur wall type, change it
        if (WallType != null && WallType != curWallType)
        {
            ChangeWallType();
        }
    }


    private void ChangeWallType()
    {

        if (WallType != curWallType)
        {
            WallType.transform.rotation = curWallType.transform.rotation; 

            DestroyImmediate(transform.GetChild(0).gameObject);

            Vector3 offset = new Vector3(0,WallType.transform.localScale.y * 0.5f,0);

            Instantiate(WallType, transform.position + offset, WallType.transform.rotation, transform);
            curWallType = WallType;
        }
    }
}
