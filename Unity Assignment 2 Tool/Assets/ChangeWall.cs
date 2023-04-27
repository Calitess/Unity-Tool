
using UnityEngine;


[ExecuteInEditMode]
public class ChangeWall : MonoBehaviour
{
    GameObject curWallType;
    [SerializeField] GameObject newWallType;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (newWallType != curWallType)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
            Instantiate(newWallType, transform);
            curWallType = newWallType;
        }
    }
}
