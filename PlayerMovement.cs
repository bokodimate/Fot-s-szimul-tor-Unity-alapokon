using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;
    public float speed = 12f;
    public float gravity = -20f;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [HideInInspector]
    public bool isSwimming;
    public bool isAtLadder;

    Vector3 velocity;
    bool isGrounded;
    public bool isOnBlind = false;

    
    // Update is called once per frame
    void Update() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z; 

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButton("Jump") && isAtLadder) {
            velocity.y = 2;
            isGrounded = true;
        } else if (Input.GetButtonDown("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        } else if (Input.GetButton("Jump") && isSwimming) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (!Input.GetButton("Jump") && isAtLadder) {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (Input.GetKey(KeyCode.W)) {
            transform.Find("HumanCharacter").GetComponent<Animator>().Play("walk");

            if (Input.GetKey(KeyCode.Space)) {
                //transform.Find("HumanMale_Character").GetComponent<Animator>().Play("JumpWhileRunning");
            }
        } else if (Input.GetKey(KeyCode.S)) {
            

            if (Input.GetKey(KeyCode.A)) {
                //transform.Find("HumanMale_Character").GetComponent<Animator>().Play("RunBackwardLeft");
            } else if (Input.GetKey(KeyCode.D)) {
                //transform.Find("HumanMale_Character").GetComponent<Animator>().Play("RunBackwardRight");
            } else {
                //transform.Find("HumanMale_Character").GetComponent<Animator>().Play("RunBackwards");
            }

        } else if (Input.GetKey(KeyCode.A)) {
           // transform.Find("HumanMale_Character").GetComponent<Animator>().Play("RunLeft");
        } else if (Input.GetKey(KeyCode.D)) {
          //  transform.Find("HumanMale_Character").GetComponent<Animator>().Play("RunRight");
        } else if (Input.GetKey(KeyCode.Space) && isGrounded) {
           // transform.Find("HumanMale_Character").GetComponent<Animator>().Play("Jump");
        } else {
            transform.Find("HumanCharacter").GetComponent<Animator>().Play("idle");
        }
    }
}
