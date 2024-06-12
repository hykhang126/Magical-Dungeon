using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform pointOfView;
    public float mouseSensitivity = 1f;
    private float verticalRotationStore;
    private Vector2 mouseInput;
    
    public bool invertLook;

    public float moveSpeed = 5f,runspeed = 8f;
    private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon;

    private Camera cam;

    public float jumpForce = 12f;
    public float gravMod = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Camera Rotation
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);

        verticalRotationStore += mouseInput.y;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, -60f, 60f);
        
        if(invertLook)
            pointOfView.rotation = Quaternion.Euler(verticalRotationStore, pointOfView.rotation.eulerAngles.y,pointOfView.rotation.eulerAngles.z);
        else
            pointOfView.rotation = Quaternion.Euler(-verticalRotationStore, pointOfView.rotation.eulerAngles.y, pointOfView.rotation.eulerAngles.z);

        //Player Movement
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runspeed;
        }
        else
        {
            activeMoveSpeed = moveSpeed;
        }
        
        float yVel = movement.y;
        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }
        //Jumping
        if (Input.GetButtonDown("Jump"))
        {
            movement.y = jumpForce;
        }
        

        //Apply Gravity
        movement.y += Physics.gravity.y * Time.deltaTime * gravMod;

        charCon.Move(movement * Time.deltaTime);
    }

    private void LateUpdate()
    {
        cam.transform.position = pointOfView.position;
        cam.transform.rotation = pointOfView.rotation;
    }
}
