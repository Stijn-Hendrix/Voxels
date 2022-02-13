using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 100f;

    [SerializeField] Transform playerTransform;
    float xRot = 0f;

    float x, y;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Cursor.lockState == CursorLockMode.Locked) {
            x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            y = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
            xRot -= y;
            xRot = Mathf.Clamp(xRot, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRot, 0, 0);
            playerTransform.Rotate(Vector3.up * x);
        }
    }
}
