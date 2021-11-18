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
    [SerializeField] private GameObject camera;

    [SerializeField] private float elevationAngle = 0.0f;
    [SerializeField] private float azimuthAngle = 0.0f;
    [SerializeField] private float aimSpeedHorizontal = 30.0f;
    [SerializeField] private float aimSpeedVertical = 20.0f;

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
        // rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
        elevationAngle = camera.transform.localEulerAngles.x;
        azimuthAngle = transform.localEulerAngles.y;
        Cursor.visible = false;
    }

  
    // Update is called once per frame
    void Update()
    {
        MouseAim();
        Movement();
    }

    void Movement()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        Vector3 movement = transform.TransformVector(new Vector3(inputX, 0, inputY) * moveSpeed);
        velocity = new Vector3(movement.x, velocity.y, movement.z);
        Vector2 horizontalMove = new Vector2(velocity.x, velocity.z);
        Vector2.ClampMagnitude(horizontalMove, maxMoveSpeedHorizontal);
        velocity = new Vector3(horizontalMove.x, velocity.y, horizontalMove.y);

        if (!isGrounded)
        {
            Ray groundCheck = new Ray(transform.position + Vector3.up * groundTolerance * 0.5f, Vector3.down);

            if (Physics.Raycast(groundCheck, groundTolerance))
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
        bool mouseAim = Input.GetMouseButton(1); ;
        cursorSprite.sprite = cursorSpriteNormal;

        //Aim
        float aimX = 0;
        float aimY = 0;

        cursorSprite.transform.position = Input.mousePosition;

        if (mouseAim)
        {
            cursorSprite.sprite = cursorSpritePan;
            //Cursor.lockState = CursorLockMode.Locked;

            aimX = Input.GetAxis("Mouse X");
            aimY = Input.GetAxis("Mouse Y");
            azimuthAngle += aimX * aimSpeedHorizontal * Time.deltaTime;
            elevationAngle = Mathf.Clamp(elevationAngle - aimY * aimSpeedVertical * Time.deltaTime, -89.999f, 89.999f);

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, azimuthAngle, transform.localEulerAngles.z);
            camera.transform.localEulerAngles = new Vector3(elevationAngle, camera.transform.localEulerAngles.y, camera.transform.localEulerAngles.z);
        }
        //else
        //{
        //    Cursor.lockState = CursorLockMode.None;
        //}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    }
}
