using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5.0f;
    [SerializeField]
    private float mouseSensitivity = 5.0f;
    [SerializeField]
    private bool invertLook;

    private float verticalRotStore;

    private PlayerMotor motor;




    private void Start() 
    {
        motor = GetComponent<PlayerMotor>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //Calculating movement velocity as 3D vector
        float _xMov = Input.GetAxisRaw("Horizontal");    
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Final movement vector
        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

        //Apply movement
        motor.Move(_velocity);



        //Calculating and Applying Rotation
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        float _rotation = mouseInput.x;
        motor.Rotate(_rotation);

        if (invertLook) 
            verticalRotStore += mouseInput.y;
        else 
            verticalRotStore -= mouseInput.y;

        verticalRotStore = Mathf.Clamp(verticalRotStore, -75.0f, 75.0f);

        motor.RotateCamera(verticalRotStore);







    }

}




        // //Calculate rotation as 3D vector for Horizontal turn.
        // float _yRot = Input.GetAxisRaw("Mouse X");

        // Vector3 _rotation = new Vector3(0,_yRot,0) * lookSensitivity;

        // //Apply rotation
        // motor.Rotate(_rotation);

        // //Calculate Camera rotation as 3D vector for Vertical turn.
        // float _xRot = Input.GetAxisRaw("Mouse Y");

        // Vector3 _cameraRotation = new Vector3(verticalRotStore,0,0) * lookSensitivity;

        // //Apply rotation
        // motor.RotateCamera(_cameraRotation);