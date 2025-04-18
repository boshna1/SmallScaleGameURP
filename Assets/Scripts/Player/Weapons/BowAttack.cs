using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BowAttack : MonoBehaviour
{
    [Header("Player General Combo Variables")]
    //determines if player should do a combo
    public int animationCount = 0;
    public bool enableAttack = true;
    public int animationCountMax = 3;

    [Header("Player Basic Combo Variables")]
    //basic
    public float[] basicAnimationTime = new float[3];
    public float[] basicAttackLungeDist = new float[3];
    public bool enableBasicAttack;
    [SerializeField] GameObject[] hitBoxBasic = new GameObject[3];
    public float basicDestroyTime;

    [Header("Player Aerial Combo Variables")]
    //aerial
    public float basicAerialAnimationTimeHorizontal;
    public float basicAerialAnimationTimeVertical;
    public bool enableBasicAerial;
    [SerializeField] GameObject hitBoxBasicAerialHorizontal;
    [SerializeField] GameObject hitBoxBasicAerialVertical;
    public float aerialDestroyTime;
    public float hopModifierX;
    public float hopModifierY;
    public float knockbackFallOff;
    public float knockbackFallOffDuration;
    public float knockbackDuration;
    public float airBufferTime;
    [SerializeField] GameObject spearProjectile;

    [Header("Player Facing")]
    //determines player directoin
    int facingHorizontal = 0;
    int facingVertical = 0;

    PlayerMovement pm;
    Rigidbody2D rb;

    [Header("Input Assignment")]
    public InputActionReference attack;

    void Start()
    {
        //attatches other components
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        facingHorizontal = 1;
        facingVertical = 1;
        enableAttack = true;
        enableBasicAttack = true;
        enableBasicAerial = false;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            facingHorizontal = -1;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            facingHorizontal = 1;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            facingVertical = -1;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            facingVertical = 1;
        }
        //calls if player is grounded from player movment script
        if (animationCount == animationCountMax && pm.ReturnIsGrounded())
        {
            animationCount = 0;
        }
        if (!pm.ReturnIsGrounded())
        {
            enableBasicAttack = false;
        }
        if (pm.ReturnIsGrounded())
        {
            enableBasicAerial = false;
            enableBasicAttack = true;
        }


    }
    private void OnEnable()
    {
        attack.action.Enable();
        attack.action.started += Attack;
    }

    private void OnDisable()
    {
        attack.action.Disable();
        attack.action.started -= Attack;
    }

    private void Attack(InputAction.CallbackContext obj)
    {
        if (enableAttack && enableBasicAttack && pm.ReturnIsGrounded() && !pm.ReturnIsDashing())
        {
            BasicAttack();
        }
        else if (enableAttack && !pm.ReturnIsGrounded() && enableBasicAerial)
        {
            AerialAttack();
        }
    }

    //coroutine, plays like an aditional seperate update function, runs independant from update
    IEnumerator WaitAnimation(float time, bool enableAnimation)
    {
        enableAttack = false;
        enableBasicAttack = false;
        yield return new WaitForSeconds(time);
        enableBasicAttack = true;
        enableAttack = true;
    }


    IEnumerator BufferAerial(float time)
    {
        yield return new WaitForSeconds(time);
        enableBasicAerial = true;
    }

    public void BasicAttack()
    {
        //calls function in player movment to lunge
        pm.Lunge(basicAttackLungeDist[animationCount]);
        GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.identity);
        if (facingHorizontal == 1)
        {
            temp.transform.localScale *= new Vector2(facingHorizontal,facingVertical);
        }
        if (facingHorizontal == -1)
        {
            temp.transform.localScale *= new Vector2(facingHorizontal, -facingVertical);
        }
        StartCoroutine(WaitAnimation(basicAnimationTime[animationCount],true));
        Destroy(temp, basicDestroyTime);
    }

    public void AerialAttack()
    {
        //in air facing left or right + no vertical, listens for key presses
        
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeVertical, false));
            //Destroy(temp, aerialDestroyTime);
        
    }

    public void BufferAerial()
    {
        StartCoroutine(BufferAerial(airBufferTime));
    }

}
