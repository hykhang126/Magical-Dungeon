using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]

public class PlayerMotor : MonoBehaviourPunCallbacks
{
    private CharacterController charCon;
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private float rotation;
    private float cameraRotation;

    public Transform viewPoint;
    


    // Start is called before the first frame update
    void Start()
    { 
        charCon = GetComponent<CharacterController>();    

        cam = Camera.main;   
    }



    //Set a movement vector
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    //Set a rotation vector
    public void Rotate(float _rotation)
    {
        rotation = _rotation;
    }

    //Set a rotation vector for camera
    public void RotateCamera(float _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }



    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            PerformMovement();
            PerformRotation();
        }

    }

    //Performs movement based on velocity var.
    void PerformMovement()
    {
       charCon.Move(velocity * Time.fixedDeltaTime);
    }

    void PerformRotation()
    {
        transform.Rotate(Vector3.up * rotation);

        if (cam != null)
        {
            viewPoint.rotation = Quaternion.Euler(cameraRotation, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z);
        }
    }



    private void LateUpdate() 
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;    
        }
   
    }




}
