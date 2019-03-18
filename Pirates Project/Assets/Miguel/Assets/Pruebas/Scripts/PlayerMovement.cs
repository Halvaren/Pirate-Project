using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float gravity = 20.0f;

    Vector3 movement;                   // The vector to store the direction of the player's movement.
    Animator anim;                      // Reference to the animator component.
    //Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
    CharacterController controller;
    int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
    float camRayLength = 100f;          // The length of the ray from the camera into the scene.

    void Awake ()
    {
        // Create a layer mask for the floor layer.
        floorMask = LayerMask.GetMask ("Floor");
        // Set up references.
        anim = GetComponent <Animator> ();
       // playerRigidbody = GetComponent <Rigidbody> ();
        controller = GetComponent <CharacterController> ();
    }


    void FixedUpdate ()
    {
        // Store the input axes.
        float h = Input.GetAxisRaw ("Horizontal");
        float v = Input.GetAxisRaw ("Vertical");

        // Move the player around the scene.
        Move (h, v);

        // Turn the player to face the mouse cursor.
        Turning ();

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

            movement = new Vector3(h, 0.0f, v);
            movement = transform.TransformDirection(movement);
            movement = movement * speed;
        }

        // Apply gravity
        movement.y = movement.y - (gravity * Time.deltaTime);

        // Move the controller
        controller.Move(movement * Time.deltaTime);
    }

    void Turning ()
    {
        // Create a ray from the mouse cursor on screen in the direction of the camera.
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
        }
    }

    void Animating (float h, float v)
    {
        // Create a boolean that is true if either of the input axes is non-zero.
        bool walking = v != 0f;
        bool rotating = Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f;
        bool sideRunning = h != 0f;
        bool running = Input.GetButton("Fire3");

        anim.SetBool ("IsWalking", (walking || rotating) && !sideRunning);
        anim.SetBool ("IsSideRunning", sideRunning);
        anim.SetBool("IsRunning",  walking && running);

        if(running) speed = 8f;
        else speed = 5f;
    }
}