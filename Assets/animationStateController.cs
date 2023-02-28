using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    ThirdPersonController controller;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isWalking", controller.isWalking);
        animator.SetBool("isGrounded", controller.isGrounded);
    }
}
