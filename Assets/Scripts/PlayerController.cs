using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // ===== Inspector / Camera & View =====
    [SerializeField] private Transform viewPoint;
    [SerializeField] private float mouseSensitivity = 1f;
    public bool invertLook ;
    
    // ===== Inspector / Movement =====
    [Header("Movement data")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float jumpForce = 5f;

    // ===== Inspector / Ground Check =====
    public Transform groundCheckPoint;
    public bool isGrounded;
    public LayerMask groundLayer;

    // ===== Runtime / Cached =====
    public CharacterController controller;
    private Camera playerCamera;

    // ===== Runtime / State =====
    [SerializeField] private bool running = true;
    private float gravityMod = 2.5f;
    private float verticalRotStore = 0f;
    private Vector2 mouseInput;
    private Vector3 moveDir, movement;
    
    public GameObject bulletImpact;
    public float timeBetweenShots = 0.1f;
    private float shotCounter;
    
    public float maxHeat = 10f;
    // public float heatPerShot = 1f;
    // public float coolRate = 4f;
    // public float overheatCoolRate = 5f;
    private float heatCounter ;
    private bool overheated;
    
    public int maxBullets = 30;
    public int currentBullets;
    private float reloadTime = 0f;
    private const float reloadTimer = 5f; 
    private float timeRate = 4f;
    private bool reloading;

    // ===== Properties =====
    private float ActualSpeed
    {
        get
        {
            if (running) return runSpeed;
            return walkSpeed;
        }
    }

    // ===== Unity Callbacks =====
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerCamera = Camera.main;
       // UIController.instance.weaponTempSlider.maxValue = maxHeat;
        currentBullets = maxBullets;
        UIController.instance.maxBullets.text = maxBullets.ToString();
    }

    private void Update()
    {
        HandleCursorLock();
        HandleMouseInput();
       // HandleShootingAndHeat();
        ReloadWeapon();
        HandleShootingBullets();
        //UIController.instance.weaponTempSlider.value = heatCounter;
        UIController.instance.reloadMessage.gameObject.SetActive(reloading);
        isGrounded = Physics.Raycast(groundCheckPoint.position, Vector3.down, 0.2f, groundLayer);
        Movement();
    }

    private void LateUpdate()
    {
        playerCamera.transform.position = viewPoint.position;
        playerCamera.transform.rotation = viewPoint.rotation;
    }

    // ===== Private Methods =====
    private void ProcessLookInput()
    {
        transform.Rotate(Vector3.up * mouseInput.x);

        verticalRotStore -= mouseInput.y;
        if (invertLook)
        {
            verticalRotStore += mouseInput.y * 2;
        }
        verticalRotStore = Mathf.Clamp(verticalRotStore, -80f, 60f);

        viewPoint.rotation = Quaternion.Euler(
            verticalRotStore,
            viewPoint.rotation.eulerAngles.y,
            viewPoint.rotation.eulerAngles.z
        );
    }

    private void Movement()
    {
        float ix = Input.GetAxisRaw("Horizontal");
        float iz = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(ix, 0f, iz);

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            running = false;
        }
        else
        {
            running = true;
        }

        float yVel = movement.y;
        movement = (transform.forward * moveDir.z + transform.right * moveDir.x).normalized * ActualSpeed;

        if (controller.isGrounded)
        {
            movement.y = 0f;
        }
        else
        {
            movement.y = yVel;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        ApplyGravity();
        controller.Move(movement * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;
    }

    private void Shoot()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        ray.origin = playerCamera.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject butlletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(butlletImpactObject, 5f);
        }

        shotCounter = timeBetweenShots;
        currentBullets--;
        UIController.instance.countBulletsText.text = currentBullets.ToString();
        if (currentBullets <= 0)
        {
            currentBullets = 0;
            reloading = true;
        }
        // heatCounter += heatPerShot;
        // if (heatCounter >= maxHeat)
        // {
        //     heatCounter = maxHeat;
        //     overheated = true;
        //     UIController.instance.overheatedMessage.gameObject.SetActive(true);
        // }
    }

    private void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void HandleMouseInput()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;
        ProcessLookInput();
    }

    private void HandleShootingBullets()
    {
        if (!reloading && currentBullets > 0)
        {
            if (Input.GetMouseButton(0))
            {
                shotCounter -= Time.deltaTime;
                if (shotCounter <= 0)
                {
                    Shoot();
                }
                
            }
        }
        else
        {
            reloadTime += timeRate*Time.deltaTime;
            if (reloadTime >= reloadTimer)
            {
                currentBullets = maxBullets;
                UIController.instance.countBulletsText.text = currentBullets.ToString();
                reloadTime = 0f;
                reloading = false;
            }
        }
    }

    private void ReloadWeapon()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            reloading = true;
        }
    }
    // private void HandleShootingAndHeat()
    // {
    //     if (!overheated)
    //     {
    //         if (Cursor.lockState == CursorLockMode.Locked)
    //         {
    //             if (Input.GetMouseButtonDown(0))
    //             {
    //                 Shoot();
    //             }
    //         }
    //
    //         if (Input.GetMouseButton(0))
    //         {
    //             shotCounter -= Time.deltaTime;
    //             if (shotCounter <= 0)
    //             {
    //                 Shoot();
    //             }
    //         }
    //
    //         heatCounter -= coolRate * Time.deltaTime;
    //     }
    //     else
    //     {
    //         heatCounter -= overheatCoolRate * Time.deltaTime;
    //         if (heatCounter <= 0)
    //         {
    //             heatCounter = 0;
    //             overheated = false;
    //             UIController.instance.overheatedMessage.gameObject.SetActive(false);
    //         }
    //     }
    //
    //     if (heatCounter < 0)
    //     {
    //         heatCounter = 0f;
    //     }
    // }
}
