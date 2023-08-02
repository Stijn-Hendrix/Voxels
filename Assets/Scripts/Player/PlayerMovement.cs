using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed = 20f;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	void Update()
    {
        float x = Input.GetAxis("Horizontal"), z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1) {
            move /= move.magnitude;
		}

        move *= speed;

        if (Input.GetKey(KeyCode.Space)) {
            move.y = speed;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            move.y = -speed;
        }

        transform.position += move * Time.deltaTime;
    }
}
