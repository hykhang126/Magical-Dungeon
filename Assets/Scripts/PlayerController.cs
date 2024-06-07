using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerMotor))]

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private float activeSpeed;
    [SerializeField]
    private float walkSpeed = 4.0f;
    [SerializeField]
    private float runSpeed = 8.0f;
    [SerializeField]
    private float jumpForce = 8.0f;
    [SerializeField]
    private float mouseSensitivity = 4.0f;
    [SerializeField]
    private bool invertLook;

    private Vector3 velocity;
    private float verticalRotStore,gravityMod = 2.5f;


    private PlayerMotor motor;
    private CharacterController charCon;
    private Camera cam;

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;
    //private float timeBetweenShots = 0.1f;
    [SerializeField]
    private float shotCounter;

    private float maxHeat = 20.0f, /* heatPerShot = 0.8f, */ coolRate = 1.7f, overheatCoolRate = 5.0f;
    private bool overHeated;
    [SerializeField]
    private float heatCounter;

    public Gun[] allGuns;
    private int selectedGun;

    public float muzzleDisplayTime;
    private float muzzleCounter;





    private void Start() 
    {
        motor = GetComponent<PlayerMotor>();
        charCon = GetComponent<CharacterController>();  

        cam = Camera.main;    

        Cursor.lockState = CursorLockMode.Locked;

        UIController.instance.weaponTempSlider.maxValue = maxHeat;

        // Transform newTrans = SpawnManager.instance.GetSpawnPoint();
        // transform.position = newTrans.position;
        // transform.rotation = newTrans.rotation;
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            //Free the mouse
            FreeMouse();

            //Movement function
            Movement();

            //Rotation function
            Rotation();

            //Shooting function
            ShootingHandler();
        }

    }




    private void FreeMouse()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }




    private void Movement()
    {
        //Calculating movement velocity as 3D vector
        float _xMov = Input.GetAxisRaw("Horizontal");    
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        //Check Movement mode
        if (Input.GetKey(KeyCode.LeftShift))
            activeSpeed = runSpeed;
        else
            activeSpeed = walkSpeed;

        float verticalGravStore = velocity.y;

        //Apply movement
        velocity = (_movHorizontal + _movVertical).normalized * activeSpeed;

        //Check Gravity
        velocity.y = verticalGravStore;

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.25f, groundLayers);

        if (isGrounded) 
        {
            velocity.y = 0.0f;
            //Apply Jump
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
            }
        }
        else
        {
            //Apply Gravity
            velocity.y += Physics.gravity.y * Time.deltaTime * gravityMod;
        }   

        //Perform Movement
        motor.Move(velocity);
    }




    private void Rotation()
    {
        //Calculating and Applying Rotation
        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;

        //Horizontal
        float _rotation = mouseInput.x;
        motor.Rotate(_rotation);

        if (invertLook) 
            verticalRotStore += mouseInput.y;
        else 
            verticalRotStore -= mouseInput.y;

        //Vertical
        verticalRotStore = Mathf.Clamp(verticalRotStore, -75.0f, 75.0f);
        motor.RotateCamera(verticalRotStore);
    }




    private void ShootingHandler()
    {
        shotCounter -= Time.deltaTime;
        //turn off muzzle flash
        if ( allGuns[selectedGun].muzzleFlash.activeInHierarchy)
        {
            muzzleCounter -= Time.deltaTime;

            if (muzzleCounter <= 0)
            {
                allGuns[selectedGun].muzzleFlash.SetActive(false);
            }
        }

        if (!overHeated)
        {
            if (Input.GetMouseButton(0) && allGuns[selectedGun].isAutomatic)
            {
                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }
            else if (Input.GetMouseButtonDown(0) && !allGuns[selectedGun].isAutomatic)
            {
                if (shotCounter <= 0)
                {
                    Shoot();
                }
            }

            heatCounter -= coolRate * Time.deltaTime;
        }
        else
        {
            heatCounter -= overheatCoolRate * Time.deltaTime;
        }

        //Over-heated check
        if (heatCounter <= 0)
        {
            heatCounter = 0.0f;
            overHeated = false;

            UIController.instance.overHeatedMessage.gameObject.SetActive(false);
        }

        //Over-heat Slider
        UIController.instance.weaponTempSlider.value = heatCounter;

        //Switching weapons
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            selectedGun++;

            if (selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }

            SwitchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            selectedGun--;

            if (selectedGun <= -1)
            {
                selectedGun = allGuns.Length - 1;
            }

            SwitchGun();
        }

        for (int i = 0; i < allGuns.Length; i++)
        {
            if ( Input.GetKeyDown((i + 1).ToString()) )
            {
                selectedGun = i;
                SwitchGun();
            }
        }



    }

    private void Shoot()
    {
        //Hit scan
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("we hit: " + hit.collider.gameObject.name);

            GameObject bulletImpactInstance = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            
            Destroy(bulletImpactInstance, 10.0f);
        }

        //Fire rate
        shotCounter = allGuns[selectedGun].timeBetweenShots;

        //Over-heat system
        heatCounter += allGuns[selectedGun].heatPerShot;
        if (heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;
            overHeated = true;

            UIController.instance.overHeatedMessage.gameObject.SetActive(true);
        }

        //Show muzzle flash
        allGuns[selectedGun].muzzleFlash.SetActive(true);
        muzzleCounter = muzzleDisplayTime;


    }




    private void SwitchGun()
    {
        foreach (Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }

        allGuns[selectedGun].gameObject.SetActive(true);

        allGuns[selectedGun].muzzleFlash.SetActive(false);
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