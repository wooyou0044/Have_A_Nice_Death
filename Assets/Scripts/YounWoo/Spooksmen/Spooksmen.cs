using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spooksmen : MonoBehaviour
{
    int attackNum;
    float hp;

    void Awake()
    {
        hp = 200.0f;
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

    // �÷��̾�� ���� ������
    void GetDamage(float damage)
    {
        hp -= damage;
    }
}
