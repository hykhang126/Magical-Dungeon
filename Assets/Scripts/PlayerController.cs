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

    public Transform groundCheckPoint;
    private bool isGrounded;
    public LayerMask groundLayers;

    public GameObject bulletImpact;

    public float timeBetweenShots = .1f;
    private float shotCounter;

    public float maxHeat = 10f, heatPerShot = 1f, coolRate = 4f, overheatCoolRate = 5f;
    private float heatCounter;
    private bool overheated;

    public Gun[] allGuns;
    private int selectedGun = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cam = Camera.main;

        UIController.instance.weaponTempSlider.maxValue = maxHeat;

        SwitchGun();
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

        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, .25f, groundLayers);
        //Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }
        

        //Apply Gravity
        movement.y += Physics.gravity.y * Time.deltaTime * gravMod;

        charCon.Move(movement * Time.deltaTime);

        //Unlocking Camera
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if(Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        //Gun Switching
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;

            if(selectedGun >= allGuns.Length)
            {
                selectedGun = 0;
            }
            SwitchGun();
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if(selectedGun < 0)
            {
                selectedGun = allGuns.Length-1;
            }
            SwitchGun();
        }

        if (!overheated)
        {
            //Shooting
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }

            //Automatic Firing
            if (Input.GetButton("Fire1") && allGuns[selectedGun].isAuto)
            {
                shotCounter -= Time.deltaTime;

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
            if(heatCounter <= 0)
            {
                overheated = false;
                UIController.instance.overheatedMessage.gameObject.SetActive(false);
            }
        }
        if(heatCounter < 0)
        {
            heatCounter = 0;
        }

        UIController.instance.weaponTempSlider.value = heatCounter;
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {

            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f),Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(bulletImpactObject, 2f);
        }

        shotCounter = timeBetweenShots;

        heatCounter += heatPerShot;
        if(heatCounter >= maxHeat)
        {
            heatCounter = maxHeat;

            overheated = true;

            UIController.instance.overheatedMessage.gameObject.SetActive(true);
        }

    }

    private void LateUpdate()
    {
        cam.transform.position = pointOfView.position;
        cam.transform.rotation = pointOfView.rotation;
    }

    void SwitchGun()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);
        heatPerShot = allGuns[selectedGun].heatPerShot;
        timeBetweenShots = allGuns[selectedGun].timeBetweenShots;

    }
}
