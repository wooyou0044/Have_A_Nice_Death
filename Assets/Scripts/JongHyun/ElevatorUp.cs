using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorUp : MonoBehaviour
{
    Vector3 growUp;
    void Start()
    {
        growUp = new Vector3(0,0.000000001f);
    }


    void Update()
    {
        transform.position += growUp;
        if (growUp.y > -2.7f)
        {
            growUp.y = -2.7f;
        }
    }
}
