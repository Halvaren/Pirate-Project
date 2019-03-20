using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 5f;
    public float gravity = 20.0f;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    //Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    CharacterController controller;
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    public Transform cameraTransform;
    private ThirdPersonCamera camera;

    private Vector3 targetDirection;

    private bool aiming;

    void Awake ()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask ("Floor");
        // Set up references.
        anim = GetComponent <Animator> ();
       // playerRigidbody = GetComponent <Rigidbody> ();
        controller = GetComponent <CharacterController> ();

        camera = cameraTransform.GetComponent<ThirdPersonCamera>();
    }


    void FixedUpdate ()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw ("Horizontal");
        float v = Input.GetAxisRaw ("Vertical");

        aiming = Input.GetButton("Aiming");
        camera.SetAiming(aiming);

        UpdateTargetDirection(h, v);

        // Move the player around the scene.
        Move (h, v);

        // Turn the player to face the mouse cursor.
        if(!aiming) Turning ();

        // Animate the player.
        Animating (h, v);
    }

    void Move (float h, float v)
    {
        // Set the movement vector based on the axis input.
        //movement.Set (h, 0f, v);
         if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes

            movement = targetDirection;
            //movement = transform.TransformDirection(movement);
            movement = movement * movementSpeed;
        }

        // Apply gravity
        movement.y = movement.y - (gravity * Time.deltaTime);

        // Move the controller
        controller.Move(movement * Time.deltaTime);
    }

    void UpdateTargetDirection(float h, float v)
    {
        targetDirection = h * cameraTransform.right + v * cameraTransform.forward;
    }

    void Turning ()
    {
        Vector3 lookDirection = targetDirection.normalized;
        if(lookDirection != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(lookDirection, transform.up);
            float differenceRotation = newRotation.eulerAngles.y - transform.eulerAngles.y;
            float eulerY = newRotation.eulerAngles.y;
            
            Vector3 euler = new Vector3(transform.eulerAngles.x, eulerY, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime);
        }    

        /* // Create a ray from the mouse cursor on screen in the direction of the camera.
        Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

        // Create a RaycastHit variable to store information about what was hit by the ray.
        RaycastHit floorHit;

        // Perform the raycast and if it hits something on the floor layer...
        if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit.
            Vector3 playerToMouse = floorHit.point - transform.position;

            // Ensure the vector is entirely along the floor plane.
            playerToMouse.y = 0f;

            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation (playerToMouse);

            // Set the player's rotation to this new rotation.
            transform.rotation = newRotation;
        }*/
    }

    void Animating (float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool walking = v != 0f || (h != 0f && !aiming);
        bool rotating = false;//Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f;
        bool sideRunning = aiming && h != 0f;
        bool running = Input.GetButton("Fire3");

        anim.SetBool ("IsWalking", (walking || rotating) && !sideRunning);
        anim.SetBool ("IsSideRunning", sideRunning);
        anim.SetBool("IsRunning",  walking && running);

        if(running) movementSpeed = 8f;
        else movementSpeed = 5f;
    }
}