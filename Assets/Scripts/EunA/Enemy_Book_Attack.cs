using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Enemy_Book_Attack : MonoBehaviour
{
    public GameObject PaperPlane;
    public GameObject PaperPlaneMuzzle;
    public bool isAttack = false;
    public bool isEnabled;
    float PlaneCoolTime;
    float PlaneElapsedTime;

    void Start()
    {
        PlaneCoolTime = 1.0f;
        PlaneElapsedTime = 1.0f;
        isEnabled = false;
        isAttack = true;
    }

    void Update()
    {
        PlaneElapsedTime += Time.deltaTime;
        if (isEnabled == true && isAttack == true&& PlaneElapsedTime >= PlaneCoolTime)
        {
            Instantiate(PaperPlane, PaperPlaneMuzzle.transform.position, PaperPlaneMuzzle.transform.rotation);
            PlaneElapsedTime = 0;
        }
    }

}
