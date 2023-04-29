
using UnityEngine;


[ExecuteInEditMode]
public class ChangeWall : MonoBehaviour
{
    [SerializeField] GameObject curWallType;
    [SerializeField] GameObject newWallType;
    // Start is called before the first frame update
    void Start()
    {
        curWallType = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (newWallType != null)
        {
            ChangeWallType();
        }
    }


    private void ChangeWallType()
    {
        if (newWallType != curWallType)
        {
            newWallType.transform.rotation = curWallType.transform.rotation; 

            DestroyImmediate(transform.GetChild(0).gameObject);

            Vector3 offset = new Vector3(0,newWallType.transform.localScale.y * 0.5f,0);

            Instantiate(newWallType, transform.position + offset, Quaternion.identity ,transform);
            curWallType = newWallType;
        }
    }
}
