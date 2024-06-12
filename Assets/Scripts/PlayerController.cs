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

    public float moveSpeed = 5f;
    private Vector3 moveDir, movement;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {
        //Mouse Rotation
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"),Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y + mouseInput.x,transform.rotation.eulerAngles.z);

        verticalRotationStore += mouseInput.y;
        verticalRotationStore = Mathf.Clamp(verticalRotationStore, -60f, 60f);
        
        if(invertLook)
            pointOfView.rotation = Quaternion.Euler(verticalRotationStore, pointOfView.rotation.eulerAngles.y,pointOfView.rotation.eulerAngles.z);
        else
            pointOfView.rotation = Quaternion.Euler(-verticalRotationStore, pointOfView.rotation.eulerAngles.y, pointOfView.rotation.eulerAngles.z);

        //Player Movement
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),0,Input.GetAxisRaw("Vertical"));

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized;

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
