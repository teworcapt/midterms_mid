using System;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject _smallWall;
    [SerializeField] GameObject _mediumWall;
    [SerializeField] GameObject _largeWall;
    [SerializeField] Transform _startline;
    [SerializeField] Transform _endline;

    private int _smallwallnumber = 10;
    private int _mediumwallnumber = 6;
    private int _largewallnumber = 3;
    private int planeWidth = 10;
    private int planeHeight = 10;
    private GameObject _mazePlane;

    RaycastHit genWall;
    float wallrotation;

    bool isFree;

    private void Start()
    {


        for (int x = 0; x < planeWidth; x++)
        {
            for (int z = 0; z < planeHeight; z++)
            {
                Vector3 vector3 = new Vector3(x * planeWidth, z * planeHeight);
                Vector3 pos = vector3;

                    SpawnWall();
            }
        }
    }


    void SpawnWall()
    {

        switch (4)
        {
            case 1:
                {
                    GameObject _smallWall = Instantiate(_smallWall, _startline.position, Quaternion.Euler((float)0.0, (float)0.0, Random.Range((float)0.0, (float)360.0)));
                }
            case 2:
                {
                    GameObject _mediumwallnumber = Instantiate(_mediumwallnumber, _startline.position, Quaternion.Euler((float)0.0, (float)0.0, Random.Range((float)0.0, (float)360.0)));
                }
            case 3:
                {
                    GameObject _largeWall = Instantiate(_largeWall, _startline.position, Quaternion.Euler((float)0.0, (float)0.0, Random.Range((float)0.0, (float)360.0)));
                }

            default:
                break;
        }
    }

}