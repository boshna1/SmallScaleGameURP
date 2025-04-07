using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
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
    public float hopModifier;

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
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        facingHorizontal = 1;
        facingVertical = 1;
        enableAttack = true;
        enableBasicAttack = true;
        enableBasicAerial = true;
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
        else if (enableAttack && !pm.ReturnIsGrounded())
        {
            AerialAttack();
        }
    }
    IEnumerator WaitAnimation(float time,bool enableAnimation)
    {
        enableAttack = false;
        enableBasicAttack = false;
        yield return new WaitForSeconds(time);
        enableBasicAttack = true;
        enableAttack = true;
        if (enableAnimation)
        {
            animationCount++;
        }           
    }
    public void BasicAttack()
    {
        pm.Lunge(basicAttackLungeDist[animationCount]);
        GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.identity, transform);
        if (facingHorizontal == 1)
        {
            temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
        }
        if (facingHorizontal == -1)
        {
            temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);
        }

        Destroy(temp, 0.25f);
        StartCoroutine(WaitAnimation(basicAnimationTime[animationCount],true));
    }

    public void AerialAttack()
    {
        //in air facing left or right + no vertical
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GameObject temp = Instantiate(hitBoxBasicAerialHorizontal,new Vector2(transform.position.x + facingHorizontal, transform.position.y),Quaternion.identity,transform);
            if (facingHorizontal == 1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);
            }
            Destroy(temp,aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal,false));
        }
        //if none are pressed
        else if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GameObject temp = Instantiate(hitBoxBasicAerialHorizontal, new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.identity, transform);
            if (facingHorizontal == 1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);
            }
            Destroy(temp, aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal,false));
        }
        //in air up or down + movment
        else if (((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || ((!Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))))
        {
            GameObject temp = Instantiate(hitBoxBasicAerialVertical, new Vector2(transform.position.x, transform.position.y + facingVertical), Quaternion.identity, transform);
            if (facingHorizontal == 1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);    
            }
            if (facingVertical == -1)
            {
                pm.Jump(hopModifier);
            }
            
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeVertical,false));
            Destroy(temp, aerialDestroyTime);
        }
    }
}
