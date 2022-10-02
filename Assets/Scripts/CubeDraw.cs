using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDraw : MonoBehaviour
{

    public int _cubeWidth = 0;
    public int _cubeHeight = 0;
    public int _cubeDepth = 0;

    int CubeWidth { get => _cubeWidth; set { _cubeWidth = value; InitCube(); } }

    int CubeHeight { get => _cubeHeight; set { _cubeHeight = value; InitCube(); } }
    int CubeDepth { get => _cubeDepth; set { _cubeDepth = value; InitCube(); } }

    public float Distance = 1;

    public GameObject Voxel;

    private List<GameObject> CubeList = new List<GameObject>();
    private GameObject[,,] CubeGrid;
    public float RotationX = 2f;
    public float RotationY = 3f;
    private float _distaChange = 1;

    void Start()
    {
        InitCube();
    }

    private void InitCube()
    {
        CubeList.ForEach(k => Destroy(k));
        CubeList.Clear();
        CubeGrid = new GameObject[CubeWidth, CubeHeight, CubeDepth];

        for (int i = 0; i < CubeWidth; i++)
        {
            for (int j = 0; j < CubeHeight; j++)
            {
                for (int k = 0; k < CubeDepth; k++)
                {
                    var c = Instantiate(Voxel);
                    c.transform.parent = transform;
                    c.transform.localPosition = new Vector3(
                        (i - (int)(CubeWidth * 0.5)) * Distance,
                        (j - (int)(CubeHeight * 0.5)) * Distance,
                        (k - (int)(CubeDepth * 0.5)) * Distance);
                    CubeGrid[i, j, k] = c;
                    CubeList.Add(c);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        for (int i = 0; i < CubeWidth; i++)
        {
            for (int j = 0; j < CubeHeight; j++)
            {
                var tVal = 2 + Mathf.Sin(j + Time.realtimeSinceStartup);
                for (int k = 0; k < CubeDepth; k++)
                {
                    var c = CubeGrid[i, j, k];
                    c.transform.localPosition = new Vector3(
                      (i - (int)(CubeWidth * 0.5)) * Distance + tVal,
                      (j - (int)(CubeHeight * 0.5)) * Distance + tVal,
                      (k - (int)(CubeDepth * 0.5)) * Distance + tVal);
                    c.transform.Rotate(new Vector3(Time.deltaTime * -RotationX * 0.7f, Time.deltaTime * -RotationY * 1.2f, 0));
                    //if(i % 3 == 0 && j % 3 == 0)
                    //{
                    //    var tVal = 2 + Mathf.Sin(k+10*Time.realtimeSinceStartup);
                    //    c.transform.localScale = new Vector3(tVal, tVal, tVal);
                    //}
                    c.transform.localScale = new Vector3(tVal*0.5f, tVal * 0.5f, tVal * 0.5f);
                }
            }
        }
        this.transform.Rotate(new Vector3(Time.deltaTime * RotationX*0.1f, Time.deltaTime * RotationY * 0.1f, 0));
        
    }
}
