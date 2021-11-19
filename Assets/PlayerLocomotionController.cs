using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLocomotionController : MonoBehaviour
{

    //[SerializeField] private int leftClicks = 0;
    //[SerializeField] private int rightClicks = 0;
    [SerializeField] private float distanceTravelled = 0;

    [Header("Gameplay")]
    [SerializeField] private float selectionRange = 10.0f;
    [SerializeField] private LayerMask pickupsMask;
    [SerializeField] private Image cursorSprite;
    [SerializeField] private Sprite cursorSpriteNormal;
    [SerializeField] private Sprite cursorSpritePan;
    [SerializeField] private Sprite cursorSpriteCanPickup;
    [SerializeField] private Sprite cursorSpriteClick;

    [Header("Aim")] 
    [SerializeField] private bool shooterStyleMouseAim = true;
    [SerializeField] private GameObject cameraOrigin;
    [SerializeField] private float cameraElevationAngle = 0.0f;
    [SerializeField] private float bodyAzimuthAngle = 0.0f;
    [SerializeField] private float aimSpeedHorizontal = 30.0f;
    [SerializeField] private float aimSpeedVertical = 20.0f;

    [Header("Headlook")]
    [SerializeField] private GameObject headTrackedCamera;
    [SerializeField] private AnimationCurve horizontalHeadLookCurve;
    [SerializeField] private float horizontalHeadLookMaxAngle = 40.0f;
    [SerializeField] private float headLookAzimuthSpeed = 10.0f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float maxMoveSpeedHorizontal = 2.0f;
    [SerializeField] private Vector3 gravity = new Vector3(0.0f, -10.0f, 0.0f);
    [SerializeField] private float jumpPowerBurst = 5.0f;
    [SerializeField] private float airControlMultiplier = 1.0f;
    [SerializeField] private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float coyoteTimeDuration = 0.5f;
    [SerializeField] private float coyoteTimeRemaining = 1.0f;
    [SerializeField] private float groundTolerance = 0.05f;

    [Header("Audio")]
    [SerializeField] private AudioSource footstepSoutAudioSource;
    [SerializeField] private float stepStrideLength = 0.2f;

    //  private Rigidbody rb;
    private CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        cameraOrigin.transform.rotation = Quaternion.identity;
         
        // rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        //cameraElevationAngle = cameraOrigin.transform.localEulerAngles.x;
        bodyAzimuthAngle = transform.localEulerAngles.y;
    }

  
    // Update is called once per frame
    void Update()
    {
        HeadLook();
        MouseAim();
        Movement();

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, bodyAzimuthAngle, transform.localEulerAngles.z);
        cameraOrigin.transform.localEulerAngles = new Vector3(cameraElevationAngle, 0, 0);
    }

    void HeadLook()
    {
        float trackedCameraAzimuth = Mathf.Repeat(headTrackedCamera.transform.localEulerAngles.y + 720.0f, 360.0f);

        if (trackedCameraAzimuth > 180.0f)
        {
            trackedCameraAzimuth -= 360.0f;
        }

        float normalizedAzimuth = Mathf.Abs(trackedCameraAzimuth) / horizontalHeadLookMaxAngle;
        float rotationInput = horizontalHeadLookCurve.Evaluate(normalizedAzimuth);
        if (rotationInput > 0.0f)
        {
            bodyAzimuthAngle = bodyAzimuthAngle + rotationInput * headLookAzimuthSpeed * Time.deltaTime * Mathf.Sign(trackedCameraAzimuth);
        }
    }

    void Movement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");

        Quaternion cameraRotation = headTrackedCamera.transform.rotation;
        cameraRotation.eulerAngles = Vector3.Scale(cameraRotation.eulerAngles, new Vector3(0.0f, 1.0f, 0.0f));
        Vector3 movement = cameraRotation * new Vector3(inputX, 0, inputY) * moveSpeed;

            //headTrackedCamera.transform.TransformVector();
        velocity = new Vector3(movement.x, velocity.y, movement.z);
        Vector2 horizontalMove = new Vector2(velocity.x, velocity.z);
        Vector2.ClampMagnitude(horizontalMove, maxMoveSpeedHorizontal);
        velocity = new Vector3(horizontalMove.x, velocity.y, horizontalMove.y);

        if (!isGrounded)
        {
            Ray groundCheck = new Ray(transform.position + controller.center, Vector3.down);

            if (Physics.Raycast(groundCheck, groundTolerance + controller.height*0.5f))
            {
                coyoteTimeRemaining = coyoteTimeDuration;
            }

        }

        coyoteTimeRemaining -= Time.deltaTime;
        isGrounded = coyoteTimeRemaining > 0.0f;
        
        if (isGrounded)
        {
            velocity = new Vector3(velocity.x, 0, velocity.z);

            if (Input.GetButton("Jump"))
            {
                velocity = new Vector3(velocity.x, jumpPowerBurst, velocity.z);
                transform.position += new Vector3(0, groundTolerance * 2, 0);
                coyoteTimeRemaining = 0.0f;
                isGrounded = false;
            }

            distanceTravelled += horizontalMove.magnitude * Time.deltaTime;
            if (distanceTravelled > stepStrideLength)
            {
                footstepSoutAudioSource.pitch = Random.Range(0.9f, 1.1f);
                footstepSoutAudioSource.Play(0);
                distanceTravelled -= stepStrideLength;
            }
        }
        else
        {
            velocity += (gravity) * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void MouseAim()
    {
        cursorSprite.sprite = cursorSpriteNormal;
        Cursor.visible = false;

        //Aim
        float aimX = Input.GetAxis("Mouse X");
        float aimY = Input.GetAxis("Mouse Y");

        if (shooterStyleMouseAim)
        {
            Cursor.lockState = CursorLockMode.Locked;
            cursorSprite.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
            bodyAzimuthAngle += aimX * aimSpeedHorizontal * Time.deltaTime;
            cameraElevationAngle += -aimY * aimSpeedVertical * Time.deltaTime;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            cursorSprite.transform.position = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            aimX = Input.GetAxis("Mouse X");
            bodyAzimuthAngle += aimX * aimSpeedHorizontal * Time.deltaTime;
        }

        if (Input.GetMouseButton(1))
        {
            cursorSprite.sprite = cursorSpritePan;
            bodyAzimuthAngle += aimX * aimSpeedHorizontal * Time.deltaTime;
            cameraElevationAngle = cameraElevationAngle - aimY * aimSpeedVertical * Time.deltaTime;
        }

        Ray ray = Camera.main.ScreenPointToRay(cursorSprite.transform.position);
        RaycastHit hit;
        bool canPickup = Physics.Raycast(ray, out hit, selectionRange, pickupsMask);
        if (canPickup)
        {
            Debug.DrawLine(ray.origin, hit.point);
            cursorSprite.sprite = cursorSpriteCanPickup;
        }


        if (Input.GetMouseButton(0))
        {
            cursorSprite.sprite = cursorSpriteClick;
            if (canPickup)
            {
                PickupItem pickup = hit.collider.gameObject.GetComponent<PickupItem>();
                if (pickup != null)
                {
                    pickup.PickUp();
                }
            }
        }

        cameraElevationAngle = Mathf.Clamp(cameraElevationAngle, -89.99f, 89.99f);
    }
}
