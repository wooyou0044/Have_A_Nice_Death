using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCamera : MonoBehaviour
{
    public GameObject Player;
    public float CameraZ = -10;
    public GameObject BossRoomWall;
    Vector3 bossRoom;

    private void Start()
    {
        bossRoom = new Vector3(5.4f, 1.6f, CameraZ);
    }
    private void FixedUpdate()
    {        
        if (transform.position.x >= -8f)
        {
            transform.position = Vector3.Lerp(bossRoom, transform.position, 0.9f);
            Camera.main.orthographicSize = 9;
            BossRoomWall.SetActive(true);
        }
        else
        {
            Vector3 targetPos = new Vector3(Player.transform.position.x, Player.transform.position.y + 3, CameraZ);
            MaxDistanceCamera();            
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
        }       
        
    }
    void MaxDistanceCamera()
    {
        if(transform.position.x <= -44f)
        {
            transform.position = new Vector3(-44f, transform.position.y, CameraZ);
        }
        if (transform.position.y <= 0f)
        {
            transform.position = new Vector3(transform.position.x, 0f, CameraZ);
        }
        else if(transform.position.y >= 0.5f)
        {
            transform.position = new Vector3(transform.position.x, 0.5f, CameraZ);
        }        
    }
}
