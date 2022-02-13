using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController characterController;

    [SerializeField] float speed = 20f;
    [Header("Jump")]
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] bool useDoubleJump;

    bool canDoubleJump;

    Vector3 vel;

    private void Awake() {
        characterController = GetComponent<CharacterController>();

        canDoubleJump = false;
    }

	void Update()
    {
        if (characterController.isGrounded && vel.y < 0) {
            vel.y = -2f;

            canDoubleJump = true;
        }

        float x = Input.GetAxis("Horizontal"), z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) {
            move /= move.magnitude;
		}

        if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) {
            Jump();
        }
        if (useDoubleJump) {
            if (Input.GetKeyDown(KeyCode.Space) && !characterController.isGrounded && canDoubleJump) {
                Jump();
                canDoubleJump = false;
            }
		}

        vel.y += gravity * Time.deltaTime;

        characterController.Move(move * speed * Time.deltaTime + vel * Time.deltaTime);
    }

    void Jump() {
        vel.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }
}
