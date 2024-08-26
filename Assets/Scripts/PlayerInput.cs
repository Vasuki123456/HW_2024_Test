using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Vector2 inputDirection = Vector2.zero;
    private Rigidbody rb;
    private float speed;

    private void Start()
    {
        var config = ConfigurationManager.GetConfig();
        if (config != null)
        {
            speed = config.player_data.speed;
        }
        else
        {
            Debug.LogWarning("Configuration data could not be loaded.");
            speed = 5f;
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody component is missing on the player object.");
        }

        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        //keyboard
        inputDirection.x = Input.GetAxis("Horizontal");
        inputDirection.y = Input.GetAxis("Vertical");

        //controller
        if (Gamepad.current != null)
        {
            Vector2 move = Gamepad.current.leftStick.ReadValue();
            inputDirection = move.normalized;
        }

        //mobile
        if (IsMobile())
        {
            HandleMobileInput();
        }

        //diagonal normalization
        if (inputDirection.magnitude > 1f)
        {
            inputDirection.Normalize();
        }
    }

    private void MovePlayer()
    {
        Vector3 move = new Vector3(inputDirection.x, 0, inputDirection.y) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Camera.main.nearClipPlane));
            Vector3 direction = (touchPosition - transform.position).normalized;

            inputDirection = new Vector2(direction.x, direction.z);
        }
    }

    private bool IsMobile()
    {
        return Application.isMobilePlatform && Input.touchCount > 0;
    }
}
