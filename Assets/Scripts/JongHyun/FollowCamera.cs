using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform cameraTransform;
    [SerializeField]
    float backgroundSpeed;
    private Vector3 offset; // ���� ī�޶� ������ �ʱ� �Ÿ�

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // ���� ī�޶� �⺻���� ����
        }

        offset = transform.position - cameraTransform.position;
    }

    void Update()
    {
        transform.position = cameraTransform.position * backgroundSpeed + offset;
    }
}
