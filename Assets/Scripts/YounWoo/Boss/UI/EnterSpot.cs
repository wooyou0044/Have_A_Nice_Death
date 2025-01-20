using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSpot : MonoBehaviour
{
    void Start()
    {

    }


    // 플레이어가 들어오면 Canvas가 SetActive됨
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            //Debug.Log("들어옴");
        }
    }
}
