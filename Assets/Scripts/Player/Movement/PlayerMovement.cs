using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed;
    public float jumpForce;
    public float dashForceX;
    public float dashVelocityX;
    public float dashFallOffDuration;
    private Vector2 _moveDirection;
    public bool isGrounded;
    public bool isDashing;

    public int facing;

    bool enableDoubleJump = true;
    int jumpCount = 0;
    int maxJump = 2;

    float residueSpeedX;
    float residueSpeedY;

    float dashDuration = 0.25f;
    float dashTime = 0;
    float dashFalloff;

    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference dash;

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        if (Input.GetKeyDown(KeyCode.A))
        {
            facing = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            facing = 1;
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
        }
    }

    private void Dash(InputAction.CallbackContext obj)
    {
        if (!isDashing)
        {
            dashVelocityX = _moveDirection.x + facing * dashForceX;
            dashTime = 0;
            isDashing = true;
            dashFalloff = 10;
            dashDuration = 2f;
        }
    }

    public void Lunge(float modifier)
    {
        dashVelocityX = _moveDirection.x + facing * modifier;
        dashTime = 0;
        isDashing = true;
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


    public void AddVelocity()
    {

    }
}
