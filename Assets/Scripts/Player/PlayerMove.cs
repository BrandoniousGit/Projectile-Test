using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed, groundAccel, airSpeed, airAccel, airDecel, jumpForce, dashStrength, groundCheckDistance, sprintMoveSpeed, crouchMoveSpeed, slideMinSpeed, maxSlopeAngle, slideGracePeriod, dashCooldown;
    [SerializeField] private bool grounded, crouching, sprinting, dashUsed, doubleJumpUsed;
    [SerializeField] private Vector3 playerGravity;

    private bool isFrozen = false;

    private float slowdownMulti, speed, groundedLerp, temporarySpeed, tempSpeedDecay, slideTimer, groundedTimer, dashTimer;
    private RaycastHit slopeHit;

    //public TextMeshProUGUI spedometer;

    public LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        var camList = Camera.allCameras;

        foreach (var camera in camList)
        {
            if (camera != cam)
            {
                camera.gameObject.SetActive(false);
            }
        }
    }

    void UserInput()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(xMove, 0, zMove).normalized;
        Vector3 horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // If holding any direction
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            Vector3 moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;

            Vector3 mag = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // Dash movement
            if (!dashUsed && Input.GetKeyDown(KeyCode.LeftShift) && dashTimer < 0)
            {
                groundedTimer = 0.2f;
                dashTimer = dashCooldown;
                grounded = false;
                dashUsed = true;

                rb.linearVelocity = new Vector3(moveDir.x * dashStrength, dashStrength / 2.5f, moveDir.z * dashStrength);
            }

            // Air movement say your prayers if you're editing this
            if (!grounded)
            {
                Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float currentMag = horizontalVel.magnitude;

                float angle = Vector3.Angle(horizontalVel, moveDir);

                if (angle > 75 && currentMag > airSpeed)
                {
                    float t = Mathf.InverseLerp(75f, 180f, angle);
                    float penalty = Mathf.Lerp(1f, slowdownMulti, t);
                    float targetSpeed = currentMag * penalty;
                    float decel = airDecel;
                    float newSpeed = Mathf.MoveTowards(currentMag, targetSpeed, decel * Time.deltaTime);

                    Vector3 newHorizontal = horizontalVel.normalized * newSpeed;

                    rb.linearVelocity = new Vector3(newHorizontal.x, rb.linearVelocity.y, newHorizontal.z);

                    horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                    currentMag = horizontalVel.magnitude;
                }

                Vector3 accel = moveDir * airAccel * Time.deltaTime;
                Vector3 newVel = horizontalVel + accel;

                if (currentMag < airSpeed)
                {
                    if (newVel.magnitude > airSpeed)
                        newVel = newVel.normalized * airSpeed;
                }
                else
                {
                    newVel = newVel.normalized * currentMag;
                }

                rb.linearVelocity = new Vector3(newVel.x, rb.linearVelocity.y, newVel.z);
            }

            // Ground movement
            else
            {
                if (IsOnSlope())
                {
                    if (!crouching)
                    {
                        moveDir = GetSlopeMovementDirection(moveDir);

                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(moveDir.x * speed, moveDir.y * speed, moveDir.z * speed), groundAccel);
                    }
                }
                else
                {
                    if (crouching && rb.linearVelocity.magnitude > slideMinSpeed)
                    {
                        slideTimer -= 1 * Time.deltaTime;

                        if (slideTimer <= 0)
                        {
                            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.005f);
                        }
                    }
                    else
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed), groundAccel);
                    }
                }
            }
        }
        else if (grounded)
        {
            if (IsOnSlope())
            {
                // Prevents the bump up when stopping on a ramp
                if (rb.linearVelocity.y > 0 && !crouching)
                {
                    rb.AddForce(Vector3.down * 50f);
                }

                if (!crouching)
                {
                    SlowDownGroundMovement();
                }
            }
            else
            {
                if (crouching && rb.linearVelocity.magnitude > slideMinSpeed)
                {
                    slideTimer -= 1 * Time.deltaTime;

                    if (slideTimer <= 0)
                    {
                        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 0.005f);
                    }
                }
                else
                {
                    SlowDownGroundMovement();
                }
            }
        }
    }

    void SlowDownGroundMovement()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x * slowdownMulti, rb.linearVelocity.y, rb.linearVelocity.z * slowdownMulti);
    }

    void CheckGrounded()
    {
        //Checks for if the player is on the ground so they can jump etc.
        if (Physics.BoxCast(transform.position, new Vector3(0.3f, 0.3f, 0.3f), Vector3.down, Quaternion.identity, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore) && groundedTimer < 0)
        {
            grounded = true;
            dashUsed = false;
            doubleJumpUsed = false;
        }
        else
        {
            if (groundedTimer >= 0)
            {
                groundedTimer -= Time.deltaTime;
            }

            grounded = false;
        }
    }

    private bool IsOnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.2f + 0.3f, groundMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMovementDirection(Vector3 moveDir)
    {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
                if (slideTimer > 0 && crouching)
                {
                    rb.linearVelocity *= 1.1f;
                }

                slideTimer = slideGracePeriod;
                groundedTimer = 0.2f;
                grounded = false;

                if (rb.linearVelocity.y <= 0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                }
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
            else if (!grounded && !doubleJumpUsed)
            {
                doubleJumpUsed = true;
                if (rb.linearVelocity.y <= 0)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                }
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            }
        }
    }

    void Crouching()
    {
        if (sprinting)
        {
            sprinting = false;
        }

        //Crouched
        if (Input.GetKey(KeyCode.LeftControl))
        {
            crouching = true;
        }

        //Uncrouched
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            crouching = false;
        }

        if (crouching)
        {
            if (!grounded)
            {
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, 0.7f, cam.transform.localPosition.z), 0.045f);
            }
            else
            {
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, 0, cam.transform.localPosition.z), 0.045f);
            }
        }
        else
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, 0.7f, cam.transform.localPosition.z), 0.045f);
        }
    }

    void Sprinting()
    {
        if (crouching)
        {
            return;
        }

        //Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && !sprinting)
        {
            sprinting = true;
        }

        //Not Sprinting
        if (Input.GetKeyUp(KeyCode.LeftShift) && sprinting)
        {
            sprinting = false;
        }
    }

    void Update()
    {
        //spedometer.SetText($"{Math.Round(new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude, 2)}"); 

        if (cam == null || rb == null) return;

        if (!IsOnSlope() || crouching || !PlayerManager.Instance.IsAlive)
        {
            rb.linearVelocity += new Vector3(playerGravity.x * Time.deltaTime, playerGravity.y * Time.deltaTime, playerGravity.z * Time.deltaTime);
        }

        if (dashTimer >= 0)
        {
            dashTimer -= Time.deltaTime;
        }

        groundedLerp = Mathf.MoveTowards(groundedLerp, grounded ? 1f : 0f, Time.deltaTime * 5f);

        float airSlowdown = Mathf.Pow(0.98f, Time.deltaTime / 0.0069f);
        float groundTarget = Mathf.Lerp(slowdownMulti, 0.82f, 0.99f);
        float groundSlowdown = Mathf.Pow(groundTarget, Time.deltaTime / 0.0069f);

        slowdownMulti = Mathf.Lerp(airSlowdown, groundSlowdown, groundedLerp);

        if (!isFrozen)
        {
            Jump();
            Crouching();
            Sprinting();
        }

        CheckGrounded();
    }

    public void ToggleFreeze(bool isFrozen)
    {
        this.isFrozen = isFrozen;
        
        if (!PlayerManager.Instance.IsAlive)
        {
            GetComponent<MeshCollider>().material = null;
            rb.AddRelativeTorque(new Vector3(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30)));
        }
    }

    public bool GetPlayerFrozen()
    {
        return isFrozen;
    }

    void LateUpdate()
    {
        float currentSpeed = 0;

        if (sprinting && !crouching && grounded)
        {
            currentSpeed = sprintMoveSpeed;
        }

        else if (!sprinting && crouching && grounded)
        {
            currentSpeed = crouchMoveSpeed;
        }

        else
        {
            currentSpeed = moveSpeed;
        }

        if (temporarySpeed > 1)
        {
            temporarySpeed -= tempSpeedDecay;
            speed = currentSpeed * temporarySpeed;
        }
        else
        {
            speed = currentSpeed;
        }

        if (!isFrozen)
        {
            UserInput();
        }
        else if (PlayerManager.Instance.IsAlive)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * slowdownMulti, rb.linearVelocity.y, rb.linearVelocity.z * slowdownMulti);
        }
    }
}