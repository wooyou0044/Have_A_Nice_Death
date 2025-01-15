using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector3 offsetCamera = new Vector3(0, 2, -10);
    [SerializeField] 
    float followSpeed = 5f; // 따라가는 속도 (높을수록 빠르게 따라감)

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }
    
    void FixedUpdate()
    {
        Vector3 targetPosition = transform.position + offsetCamera;

        float dx = targetPosition.x - cameraTransform.position.x;
        float dy = targetPosition.y - cameraTransform.position.y-1;
        float dz = targetPosition.z - cameraTransform.position.z;

        float stepX = dx * followSpeed * Time.deltaTime;
        float stepY = dy * followSpeed * Time.deltaTime;
        float stepZ = dz * followSpeed * Time.deltaTime;

        // 카메라 위치 갱신
        cameraTransform.position = new Vector3(
            cameraTransform.position.x + stepX,
            cameraTransform.position.y + stepY,
            cameraTransform.position.z + stepZ );

        MaximumDistanceCamera();

    }
    void MaximumDistanceCamera()
    {
        if(cameraTransform.position.x < -5.0f)
        {
            cameraTransform.position = new Vector3(-5.0f,cameraTransform.position.y, cameraTransform.position.z);
        }
        //반대쪽 x대입
        //else if(cameraTransform.position.x > 30f)
        //{
        //    cameraTransform.position = new Vector3(30f, cameraTransform.position.y, cameraTransform.position.z);
        //}
    }
}
