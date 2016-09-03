using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    private Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Camera.main.GetComponent<follow>().enabled = false;
    }
    void Update()
    {

        if (controller.isGrounded)
        {
            // THERE IS A BUG WITH CAMERA, need to disable then enable, 
            //you dont need to worry about it...
            Camera.main.GetComponent<follow>().enabled = true;

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
                moveDirection.y = jumpSpeed;

        }
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}
