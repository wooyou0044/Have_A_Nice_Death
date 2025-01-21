using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ScreenShaking : MonoBehaviour
{
    public Transform ShakeCameraTransform;
    private Vector3 originalPosition;
    float elapsed;

    void Start()
    {
        if (ShakeCameraTransform == null)
        {
            ShakeCameraTransform = Camera.main.transform;
        }
        originalPosition = ShakeCameraTransform.position; 
    }
    public void Shake()
    {
        elapsed = 0f;
        while (elapsed < 0.3f)
        {
            elapsed += Time.deltaTime;
            Vector3 randomCamera = new Vector3(Random.Range(-1f, 1f) * 0.3f,Random.Range(-1f, 1f) * 0.3f,0);

            ShakeCameraTransform.position = Vector3.Lerp(ShakeCameraTransform.position, originalPosition + randomCamera, 0.5f);            
        }
        ShakeCameraTransform.position = originalPosition;
    }
}
