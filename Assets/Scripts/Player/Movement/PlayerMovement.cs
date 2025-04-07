using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player General Movement Variables")]
    public Rigidbody2D rb;
    public float moveSpeed;    
    public int facingHorizontal;
    public Vector2 _moveDirection;

    [Header("Player Dash Variables")]
    public float dashForceX;
    public float dashVelocityX;
    public float dashFallOffDuration;
    public float residueSpeedX;
    public float residueSpeedY;
    public float dashDuration = 0.25f;
    public float dashTime = 0;
    public float dashFalloff;

    [Header("Player Condition Variables")]
    public bool isGrounded;
    public bool isDashing;
    public bool isLunging;
    bool enableDoubleJump = true;

    [Header("Player Jump Variables")]
    int jumpCount = 0;
    int maxJump = 2;
    public float jumpForce;

    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference dash;

    SwordAttack SA;

    private void Start()
    {
        SA = GetComponent<SwordAttack>();
    }

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            facingHorizontal = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            facingHorizontal = 1;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)  
        {
            dashTime += Time.fixedDeltaTime;
            rb.velocity = new Vector2(dashVelocityX, rb.velocity.y);
            if (dashForceX > dashFalloff)
            {
                dashVelocityX = Mathf.Lerp(dashVelocityX, 0, dashTime / dashFallOffDuration);                
            }
            if (dashTime >= dashDuration) 
            {
                dashForceX = 15;
                isDashing = false;
                if (isLunging)
                {
                    isLunging = false;
                }
                residueSpeedX = dashVelocityX;
            }
        }
        else if (residueSpeedX != 0)
        {
            residueSpeedX = Mathf.MoveTowards(residueSpeedX,0,1);
            rb.velocity = new Vector2(residueSpeedX,0) + new Vector2(_moveDirection.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(_moveDirection.x * moveSpeed, rb.velocity.y);
        }
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        dash.action.Enable();
        jump.action.started += Jump;
        dash.action.started += Dash;
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        dash.action.Disable();

        jump.action.started -= Jump;
        dash.action.started -= Dash;
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded || enableDoubleJump && jumpCount < maxJump)
        {           
            jumpCount++;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
            SA.BufferAerial();
        }
    }

    private void Dash(InputAction.CallbackContext obj)
    {
        if (!isDashing)
        {
            dashVelocityX = _moveDirection.x + facingHorizontal * dashForceX;
            dashTime = 0;
            isDashing = true;
            dashFallOffDuration = 2;
            dashFalloff = 10;
            dashDuration = 0.25f;
        }
    }

    public void Lunge(float modifier)
    {
        dashVelocityX = _moveDirection.x + facingHorizontal * modifier;
        dashTime = 0;
        isDashing = true;
        isLunging = true;
        dashFallOffDuration = 0.25f;
        dashFalloff = 0.0005f;
        dashDuration = 0.0005f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    public bool ReturnIsDashing()
    {
        return isDashing;
    }

    public bool ReturnIsLunging()
    {
        return isLunging;
    }

    public bool ReturnIsGrounded()
    {
        return isGrounded;
    }

    public void Jump(float jump)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jump);
        isGrounded = false;
    }
}
