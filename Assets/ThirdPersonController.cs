using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float runSpeed = 15f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public float gravity;
    public float jumpStrenght;

    public bool isWalking;
    public bool isGrounded;

    Vector3 moveVelocity;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = new Vector3();
        if (direction.magnitude > 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime) ;
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            isWalking = true;
        }
        else
        {
            isWalking = false;
        }

        float moveSpeed = speed;
        if (Input.GetButton("Run"))
        {
            moveSpeed = runSpeed;
        }
         
        moveVelocity = moveDir.normalized * moveSpeed + new Vector3(0, moveVelocity.y, 0);

        if (controller.isGrounded)
        {
            isGrounded = true;
            if (Input.GetButtonDown("Jump"))
            {
                moveVelocity.y = jumpStrenght;
            }
        }
        else
        {
            moveVelocity.y += gravity * Time.deltaTime;
            isGrounded = false;
        }
        
        

        controller.Move(moveVelocity * Time.deltaTime);
    }
}
