using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    public GameObject Player;
    public float CameraZ = -10;

    private void FixedUpdate()
    {
        MaxDistanceCamera();
        Vector3 targetPos = new Vector3(Player.transform.position.x, Player.transform.position.y+3, CameraZ);
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
    }
    void MaxDistanceCamera()
    {
        if(transform.position.x <= -44f)
        {
            transform.position = new Vector3(-44f, transform.position.y, CameraZ);
        }
        if (transform.position.y <= -1f)
        {
            transform.position = new Vector3(transform.position.x, -1f, CameraZ);
        }
        else if(transform.position.y >= 0.5f)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, CameraZ);
        }
    }
}
