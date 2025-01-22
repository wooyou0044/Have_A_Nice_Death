using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSpot : MonoBehaviour
{
    bool isPlayerEnter;

    public bool IsEnter
    {
        get
        {
            return isPlayerEnter;
        }
        set
        {
            isPlayerEnter = value;
        }
    }

    void Start()
    {
        isPlayerEnter = false;
    }


    // �÷��̾ ������ Canvas�� SetActive��
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            isPlayerEnter = true;
        }
    }
}
