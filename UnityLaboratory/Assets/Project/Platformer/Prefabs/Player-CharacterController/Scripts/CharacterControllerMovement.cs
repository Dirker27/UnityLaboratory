using UnityEngine;
using System.Collections;

public class CharacterControllerMovement : MonoBehaviour
{
    private const string TAG = "[CharacterControllerMovement] ";

    public float walkSpeed = 3f;     // [m/s]
    public float runSpeed  = 6f;     // [m/s]
    public float jumpSpeed = 6f;     // [m/s]

    public float gravity = 9.8f;     // [m/s^2]
    
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

	// Use this for initialization
	void Start ()
    {
        characterController = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        PerformMovement();
    }

    /**
     * CharacterController-based movement.
     *
     * https://docs.unity3d.com/ScriptReference/CharacterController.Move.html
     */
    void PerformMovement()
    {
        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));

            // Apply "Sprint"
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection *= runSpeed;
            }
            else
            {
                moveDirection *= walkSpeed;
            }

            // Perform Jump (from ground)
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Debug.Log(TAG + "Collision detected at pos[" + hit.transform.position + "].");
    }
}
