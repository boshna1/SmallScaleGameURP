using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpearAttack : MonoBehaviour
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
    public bool isSpearSpam;
    public float spamTime;

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

    [Header("Player Dash Combo Variables")]
    public bool enableDashAttack;
    public float dashDistance;
    [SerializeField] GameObject[] hitBoxDashAttack = new GameObject[1];

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
        isSpearSpam = false;
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
        if (enableAnimation && !isSpearSpam)
        {
            animationCount++;
        }
        if (animationCount == 2 && !isSpearSpam)
        {
            isSpearSpam = true;
        }
    }

    IEnumerator CheckSpamTime()
    {
        yield return new WaitForSeconds(0.4f);
        Debug.Log(spamTime - Time.time);
        if (spamTime - Time.time < -0.4f && isSpearSpam)
        {
            isSpearSpam = false;
            animationCount = 0;
        }
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
        int random = Random.Range(-25, 25);
        if (!isSpearSpam)
        {
            GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(this.transform.position.x + facingHorizontal, this.transform.position.y), Quaternion.identity, transform);
            if (facingHorizontal == 1) //changes direction of hitbox depending on where the player is facing by mirroring it using scale
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);
            }
            Destroy(temp, 1f);
            StartCoroutine(WaitAnimation(basicAnimationTime[animationCount], true));
        }
        if (isSpearSpam)
        {
            GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.Euler(0, 0, random), transform);
            if (facingHorizontal == 1) //changes direction of hitbox depending on where the player is facing by mirroring it using scale
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, facingHorizontal);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.localScale *= new Vector2(facingHorizontal, -facingHorizontal);
            }
            spamTime = Time.time;
            StartCoroutine(CheckSpamTime());
            Destroy(temp, 1f);
        }



    }

    public void AerialAttack()
    {
        //in air facing left or right + no vertical, listens for key presses
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.identity, transform);
            if (Input.GetKey(KeyCode.A))
            {
                temp.transform.rotation = Quaternion.Euler(0, 0, -180);
                pm.EnableKnockBack(new Vector2(hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            if (Input.GetKey(KeyCode.D))
            {
                pm.EnableKnockBack(new Vector2(-hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //detroys object x time after calling this
            Destroy(temp, aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal, false));
        }
        //if none are pressed
        else if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x + facingHorizontal, transform.position.y), Quaternion.identity, transform);
            if (facingHorizontal == 1)
            {
                pm.EnableKnockBack(new Vector2(-hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            if (facingHorizontal == -1)
            {
                temp.transform.rotation = Quaternion.Euler(0, 0, -180);
                pm.EnableKnockBack(new Vector2(hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            Destroy(temp, aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal, false));
        }
        //in air up or down + movment
        else if (((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) || ((!Input.GetKey(KeyCode.A) || !Input.GetKey(KeyCode.D))) && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))))
        {
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x, transform.position.y + facingVertical), Quaternion.identity, transform);
            //Up Left
            if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.A)))
            {
                Debug.Log("Up Left");
                temp.transform.rotation = Quaternion.Euler(0, 0, 135);
                pm.EnableKnockBack(new Vector2(hopModifierX, -hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Up Right
            if (Input.GetKey(KeyCode.W) && (Input.GetKey(KeyCode.D)))
            {
                Debug.Log("Up Right");
                temp.transform.rotation = Quaternion.Euler(0, 0, 45);
                pm.EnableKnockBack(new Vector2(-hopModifierX, -hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down Left
            if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.A)))
            {
                Debug.Log("Down Left");
                temp.transform.rotation = Quaternion.Euler(0, 0, -135);

                pm.EnableKnockBack(new Vector2(hopModifierX, hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down Right
            if (Input.GetKey(KeyCode.S) && (Input.GetKey(KeyCode.D)))
            {
                Debug.Log("Down Right");
                temp.transform.rotation = Quaternion.Euler(0, 0, -45);
                pm.EnableKnockBack(new Vector2(-hopModifierX, hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                Debug.Log("Down");
                temp.transform.rotation = Quaternion.Euler(0, 0, -90);
                pm.Hop(hopModifierY);
            }
            //Up
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                Debug.Log("Up");
                temp.transform.rotation = Quaternion.Euler(0, 0, 90);
                pm.Hop(-hopModifierY);
            }

            StartCoroutine(WaitAnimation(basicAerialAnimationTimeVertical, false));
            Destroy(temp, aerialDestroyTime);
        }
    }

    public void BufferAerial()
    {
        StartCoroutine(BufferAerial(airBufferTime));
    }

    public void EnableDashAttack(bool condition)
    {
        enableDashAttack = condition;
    }

}
