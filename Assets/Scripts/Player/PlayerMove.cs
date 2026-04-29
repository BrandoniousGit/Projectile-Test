using System;
using TMPro;
using UnityEngine;

public class LocalPlayerMove : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed, groundAccel, airSpeed, airAccel, airDecel, jumpForce, dashStrength, playerGravity, groundCheckDistance, sprintMoveSpeed, crouchMoveSpeed;
    [SerializeField] private bool grounded, crouching, sprinting, dashUsed, doubleJumpUsed;
    private bool canMove = true;

    private float slowdownMulti, speed, groundedLerp;
    public TextMeshProUGUI spedometer;

    public LayerMask groundMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void UserInput()
    {
        float xMove = Input.GetAxisRaw("Horizontal");
        float zMove = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(xMove, 0, zMove).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            Vector3 moveDir = (Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward).normalized;

            Vector3 mag = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            if (!grounded && !dashUsed && Input.GetKeyDown(KeyCode.LeftShift))
            {
                rb.linearVelocity = new Vector3(moveDir.x * dashStrength, 0, moveDir.z * dashStrength);
                dashUsed = true;
            }

            if (!grounded)
            {
                Vector3 horizontalVel = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                float currentMag = horizontalVel.magnitude;

                float angle = Vector3.Angle(horizontalVel, moveDir);

                if (angle > 60 && currentMag > airSpeed)
                {
                    float t = Mathf.InverseLerp(60f, 180f, angle);
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

            else
            {
                rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(moveDir.x * speed, rb.linearVelocity.y, moveDir.z * speed), groundAccel);
            }
        }
        else if (grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * slowdownMulti, rb.linearVelocity.y, rb.linearVelocity.z * slowdownMulti);
        }
    }

    void CheckGrounded()
    {
        RaycastHit hit;

        //Checks for if the player is on the ground so they can jump etc.
        if (Physics.SphereCast(transform.position, 0.41f, Vector3.down, out hit, groundCheckDistance, groundMask) && rb.linearVelocity.y <= 0)
        {
            grounded = true;
            dashUsed = false;
            doubleJumpUsed = false;
        }
        else
        {
            grounded = false;
        }
    }

    void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grounded)
            {
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
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x, 0, cam.transform.localPosition.z), 0.045f);
        }

        //Uncrouched
        if (!Input.GetKey(KeyCode.LeftControl))
        {
            crouching = false;
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

        if (Input.GetKeyDown(KeyCode.X))
        {
            FreeMe();
        }
        if (cam == null || rb == null) return;

        rb.linearVelocity += new Vector3(0, playerGravity * Time.deltaTime, 0);

        groundedLerp = Mathf.MoveTowards(groundedLerp, grounded ? 1f : 0f, Time.deltaTime * 5f);

        float airSlowdown = Mathf.Pow(0.98f, Time.deltaTime / 0.0069f);
        float groundTarget = Mathf.Lerp(slowdownMulti, 0.82f, 0.99f);
        float groundSlowdown = Mathf.Pow(groundTarget, Time.deltaTime / 0.0069f);

        slowdownMulti = Mathf.Lerp(airSlowdown, groundSlowdown, groundedLerp);

        if (canMove)
        {
            Jumping();
            Crouching();
            Sprinting();
        }

        CheckGrounded();
    }

    public void FreeMe()
    {
        canMove = !canMove;
    }

    void LateUpdate()
    {
        if (sprinting && !crouching && grounded)
        {
            speed = sprintMoveSpeed;
        }

        else if (!sprinting && crouching && grounded)
        {
            speed = crouchMoveSpeed;
        }

        else
        {
            speed = moveSpeed;
        }

        if (canMove)
        {
            UserInput();
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * slowdownMulti, rb.linearVelocity.y, rb.linearVelocity.z * slowdownMulti);
        }
    }
}