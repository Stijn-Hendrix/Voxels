using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField] float speed = 20f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] float gravity = -9.81f;

    Vector3 vel;

    private void Awake() {
        characterController = GetComponent<CharacterController>();
	}

	void Update()
    {
        if (characterController.isGrounded && vel.y < 0) {
            vel.y = -2f;
		}

        float x = Input.GetAxis("Horizontal"), z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) {
            vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}

        vel.y += gravity * Time.deltaTime;

        characterController.Move(move * speed * Time.deltaTime + vel * Time.deltaTime);
    }
}
