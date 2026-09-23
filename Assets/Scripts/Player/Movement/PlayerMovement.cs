using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        Idle,
        Walking,
        Sprinting,
        WallDraging,
        Grounded,
        Airborne
    }

    public enum CombatState
    {
        Attacking
    }

    public MovementState movementState;

    [Header("Player General Movement Variables")]
    public Rigidbody2D rb;
    public float moveSpeed;    
    public Vector2 _moveDirection;
    

    [Header("Player Dash Variables")]
    public float dashForceX;
    public float dashVelocityX;
    public float dashFallOffDuration;
    public float residueSpeedX;
    public float residueSpeedY;
    public float dashDuration;
    public float dashTime;
    public float dashFalloff;

    [Header("Player Condition Variables")]
    public bool isGrounded;
    public bool isDashing;
    public bool isLunging;
    bool enableDoubleJump = true;

    [Header("Player Knockback Variables")]
    public bool isKnockback;
    public Vector2 knockbackVelocity;
    public float knockbackX;
    public float knockbackTime;
    public float knockbackFalloff;
    public float knockbackFallOffDuration;
    public float knockbackDuration;
    public bool localKnockback;

    [Header("Player Jump Variables")]
    int jumpCount = 0;
    int maxJump = 2;
    public float jumpForce;

    public InputActionReference move;
    public InputActionReference jump;
    public InputActionReference dash;

    PlayerInput pi;
    Gamepad currentGamepad;

    SwordAttack swordAttack;
    SpearAttack spearAttack;
    HammerAttack hammerAttack;
    BowAttack bowAttack;

    Vector2 normal;

    [SerializeField] float landSoundThreshold = 3;

    public float baseLungeDist;

    public enum DirX
    {
        Left,
        Right,
        None,
    }
    public enum DirY
    {
        Up,
        Down,
        None
    }

    public DirX dirX;
    public DirY dirY;


    private void Start()
    {
        pi = GetComponent<PlayerInput>();
        isKnockback = false;    
        if (name == "PlayerSword")
        {
            swordAttack = GetComponent<SwordAttack>();
        }
        if (name == "PlayerSpear Variant 1")
        {
            spearAttack = GetComponent < SpearAttack>();
        }
        if (name == "PlayerHammer")
        {
            hammerAttack = GetComponent<HammerAttack>();
        }
        if (name == "PlayerBow")
        {
            bowAttack = GetComponent<BowAttack>();
        }
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            DashingFunction();
        }
        else if (isKnockback)
        {
            KnockBackFunction();
        }
        else if (residueSpeedX != 0)
        {
            PassEnableDash(false);
            residueSpeedX = Mathf.MoveTowards(residueSpeedX, 0, 1);
            rb.linearVelocity = new Vector2(residueSpeedX, 0) + new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, rb.linearVelocity.y);
        }
        if (rb.linearVelocityY < 0 && movementState == MovementState.WallDraging)
        {
            rb.gravityScale = 1;
        }
        else
        {
            rb.gravityScale = 5;
        }
    }

    public void OnMove(InputValue value)
    {
        _moveDirection = value.Get<Vector2>();
        CalculateDirections();
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        dash.action.Enable();
        jump.action.started += Jump;
        dash.action.started += Dash;
        InputSystem.onDeviceChange += OnDeviceChange;
        pi.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {

        move.action.Disable();
        jump.action.Disable();
        dash.action.Disable();
        jump.action.started -= Jump;
        dash.action.started -= Dash;
        InputSystem.onDeviceChange -= OnDeviceChange;
        pi.onControlsChanged -= OnControlsChanged;
    }
    

    public void OnControlsChanged(PlayerInput currentInput)
    {
        if (currentInput.currentControlScheme == "Gamepad")
        {
            pi.SwitchCurrentControlScheme("Gamepad", Gamepad.current);
        }
        else if (currentInput.currentControlScheme == "Keyboard")
        {
            pi.SwitchCurrentControlScheme("Keyboard", Keyboard.current);
        }
        Debug.Log("Changed input to" + currentInput.currentControlScheme);
    }

    public void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    Debug.Log("Added device");
                    break;
                case InputDeviceChange.Removed:
                    Debug.Log("Removed device");
                    break;
            }
        }
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded || movementState == MovementState.WallDraging || enableDoubleJump && jumpCount < maxJump && obj.performed)
        {           
            jumpCount++;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            if (name == "PlayerSword")
            {
                swordAttack.BufferAerial();
            }
            if (name == "PlayerSpear Variant 1")
            {
                spearAttack.BufferAerial();
            }

        }
    }

    private void Dash(InputAction.CallbackContext obj)
    {
        if (!isDashing)
        {
            AudioManager.Instance.PlaySoundAmbientPitch("Whoosh",2.5f,0.8f);
            PassEnableDash(true);
            dashVelocityX = _moveDirection.x * dashForceX;
            dashTime = 0;
            isDashing = true;
            dashFallOffDuration = 2;
            dashFalloff = 10;
            dashDuration = 0.25f;
        }
    }

    public void Lunge(float modifier)
    {
        dashVelocityX = (_moveDirection.x + baseLungeDist) * modifier ;
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
            foreach (ContactPoint2D contact in collision.contacts)
            {
                normal = contact.normal;
            }
            
            if (normal.x != 0 && rb.linearVelocityY < 0)
            {
                movementState = MovementState.WallDraging;
                if (rb.linearVelocityY < landSoundThreshold)
                {
                    AudioManager.Instance.PlaySoundAmbientPitch("GroundLand", 2.5f, 0.3f);   
                }
            }
            else if (normal.y != -1)
            {
                AudioManager.Instance.PlaySoundAmbientPitch("GroundLand", AudioManager.Instance.defaultPitch, 0.3f);
                movementState = MovementState.Grounded;
                isGrounded = true;
                jumpCount = 0;
            }           
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            
            if (rb.linearVelocityY != 0)
            {
                movementState = MovementState.Airborne;
                isGrounded = false;
                AudioManager.Instance.PlaySoundAmbientPitch("GroundLand", 2.5f, 0.3f);
            }
            
            

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

    public void Hop(float y)
    {
        rb.linearVelocity = Vector2.zero;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + y);
        isGrounded = false;
    }

    public Vector2 ReturnMoveDir()
    {
        return _moveDirection;
    }

    public void DashingFunction()
    {
        dashTime += Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(dashVelocityX, rb.linearVelocity.y);
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
    public void KnockBackFunction()
    {
        knockbackTime += Time.fixedDeltaTime;
        rb.linearVelocity = new Vector2(knockbackVelocity.x, rb.linearVelocity.y);
        if (knockbackX > knockbackFalloff)
        {
            knockbackVelocity.x = Mathf.Lerp(knockbackVelocity.x, 0, knockbackTime / knockbackFallOffDuration);
        }
        if (knockbackTime >= knockbackDuration)
        {
            knockbackX = 0;
            isKnockback = false;
        }
        residueSpeedX = knockbackVelocity.x;
    }

    public void EnableKnockBack(Vector2 knockbackVelocity, float knockbackX, float knockbackFalloff, float knockbackFallOffDuration, float knockbackDuration)
    {
        knockbackTime = 0;
        localKnockback = false;
        isKnockback = true;
        this.knockbackVelocity = knockbackVelocity;
        this.knockbackX = knockbackX;
        this.knockbackFalloff = knockbackFalloff;
        this.knockbackFallOffDuration = knockbackFallOffDuration;
        this.knockbackDuration = knockbackDuration;
    }

    public void PassEnableDash(bool condition)
    {
        if (name == "PlayerSword")
        {
            swordAttack.EnableDashAttack(condition);
        }
        if (name == "PlayerSpear Variant 1")
        {
            spearAttack.EnableDashAttack(condition);
        }
        if (name == "PlayerHammer")
        {
            hammerAttack.EnableDashAttack(condition);
        }
        if (name == "PlayerBow")
        {
            bowAttack.EnableDashAttack(condition);
        }
    }

    public void CalculateDirections()
    {
        if (Mathf.Abs(_moveDirection.x) > 0.2f)
        {
            if (_moveDirection.x < 0)
            {
                dirX = DirX.Left;
                baseLungeDist = -baseLungeDist;
                spearAttack.xDirMod = -1;
            }
            else if (_moveDirection.x > 0)
            {
                dirX = DirX.Right;
                baseLungeDist = 0.6f;
                spearAttack.xDirMod = 1;
            }
            else
            {
                dirX = DirX.None;
            }
        }
        if (Mathf.Abs(_moveDirection.y) > 0.2f)
        {
            if (_moveDirection.y < 0)
            {
                dirY = DirY.Down;
                spearAttack.yDirMod = -1;
            }
            else if (_moveDirection.y > 0)
            {
                dirY = DirY.Up;
                spearAttack.yDirMod = 1;
            }
            
        }
        else
        {
            dirY = DirY.None;
        }
    }

}



    



