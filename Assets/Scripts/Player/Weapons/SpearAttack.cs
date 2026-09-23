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

    public float[] basicUpAnimationTime = new float[1];
    public bool enableBasicAttack;
    [SerializeField] GameObject[] hitBoxBasic = new GameObject[3];

    [SerializeField] GameObject[] hitBoxUpBasic = new GameObject[1];
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

    PlayerInput pi;
    PlayerMovement pm;
    Rigidbody2D rb;

    [Header("Input Assignment")]
    public InputActionReference attack;

    public int xDirMod = -1;
    public int yDirMod = -1;

    void Start()
    {
        xDirMod = -1;
        yDirMod = -1;
        pi = GetComponent<PlayerInput>();
        //attatches other components
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        enableAttack = true;
        enableBasicAttack = true;
        enableBasicAerial = false;
        isSpearSpam = false;
    }


    void Update()
    {
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
        if (enableAttack && enableBasicAttack && pm.ReturnIsGrounded() && !pm.ReturnIsDashing() && obj.started)
        {
            BasicAttack();
            Debug.Log("pass1");
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
        Debug.Log("pass2");
        pm.CalculateDirections();
        //calls function in player movment to lunge
        pm.Lunge(basicAttackLungeDist[animationCount]);
        int random = Random.Range(-25, 25);
        if (!isSpearSpam)
        {
            if (pm.dirX != PlayerMovement.DirX.None && pm.dirY == PlayerMovement.DirY.None)
            {
                Debug.Log("pass3");
                GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity, transform);
                temp.transform.localScale *= new Vector2(xDirMod, yDirMod);
                Destroy(temp, 1f);
                StartCoroutine(WaitAnimation(basicAnimationTime[animationCount], true));
            }
            else if (pm.dirY == PlayerMovement.DirY.Up)
            {
                animationCount = 0;
                GameObject temp = Instantiate(hitBoxUpBasic[animationCount], new Vector2(this.transform.position.x, this.transform.position.y), Quaternion.identity, transform);
                temp.transform.localScale *= new Vector2(xDirMod, 1);
                AnimationClip clipInfo = temp.GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
                StartCoroutine(WaitAnimation(clipInfo.length, true));
                Destroy(temp, clipInfo.length + 0.5f);
                animationCount = 0;
                isSpearSpam = false;
            }
            
        }
        else if (isSpearSpam && animationCount == 2)
        {
            GameObject temp = Instantiate(hitBoxBasic[animationCount], new Vector2(transform.position.x, transform.position.y), Quaternion.Euler(0, 0, random), transform);
            temp.transform.localScale *= new Vector2(xDirMod, yDirMod);
            spamTime = Time.time;
            StartCoroutine(CheckSpamTime());
            Destroy(temp, 1f);
        }



    }

    public void AerialAttack()
    {
        pm.dirY = PlayerMovement.DirY.None;
        //in air facing left or left + no vertical, listens for key presses
        if (pm._moveDirection.x != 0 && Mathf.Abs(pm._moveDirection.y) < 0.4f)
        {
            
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x + pm._moveDirection.x, transform.position.y), Quaternion.identity);
            if (pm.dirX == PlayerMovement.DirX.Left)
            {
                temp.transform.rotation = Quaternion.Euler(0, 0, -180);
                pm.EnableKnockBack(new Vector2(hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            if (pm.dirX == PlayerMovement.DirX.Right)
            {
                pm.EnableKnockBack(new Vector2(-hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //detroys object x time after calling this
            Destroy(temp, aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal, false));
        }
        //if none are pressed
        else if (pm._moveDirection.magnitude == 0)
        {
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x + pm._moveDirection.x, transform.position.y), Quaternion.identity);
            if (pm.dirX == PlayerMovement.DirX.Right)
            {
                pm.EnableKnockBack(new Vector2(-hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            if (pm.dirX == PlayerMovement.DirX.Left)
            {
                temp.transform.rotation = Quaternion.Euler(0, 0, -180);
                pm.EnableKnockBack(new Vector2(hopModifierX, 0), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            Destroy(temp, aerialDestroyTime);
            StartCoroutine(WaitAnimation(basicAerialAnimationTimeHorizontal, false));
        }
        //in air up or down + movment
        else if ((Mathf.Abs(pm._moveDirection.x) > 0.4f && Mathf.Abs(pm._moveDirection.y) > 0.2f) || (Mathf.Abs(pm._moveDirection.x) < 0.2f && pm._moveDirection.y != 0))
        {
            GameObject temp = Instantiate(spearProjectile, new Vector2(transform.position.x, transform.position.y + pm._moveDirection.y), Quaternion.identity);
            //Up Left
            if (pm.dirY == PlayerMovement.DirY.Up && pm.dirX == PlayerMovement.DirX.Left && Mathf.Abs(pm._moveDirection.x) > 0.2f)
            {
                Debug.Log("Up Right");
                temp.transform.rotation = Quaternion.Euler(0, 0, 135);
                pm.EnableKnockBack(new Vector2(hopModifierX, -hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Up left
            else if (pm.dirY == PlayerMovement.DirY.Up && pm.dirX == PlayerMovement.DirX.Right && Mathf.Abs(pm._moveDirection.x) > 0.2f )
            {
                Debug.Log("Up left");
                temp.transform.rotation = Quaternion.Euler(0, 0, 45);
                pm.EnableKnockBack(new Vector2(-hopModifierX, -hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down left
            else if (pm.dirY == PlayerMovement.DirY.Down && pm.dirX == PlayerMovement.DirX.Left && Mathf.Abs(pm._moveDirection.x) > 0.2f)
            {
                Debug.Log("Down left");
                temp.transform.rotation = Quaternion.Euler(0, 0, -135);

                pm.EnableKnockBack(new Vector2(hopModifierX, hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down right
            else if(pm.dirY == PlayerMovement.DirY.Down && pm.dirX == PlayerMovement.DirX.Right && Mathf.Abs(pm._moveDirection.x) > 0.2f)
            {
                Debug.Log("Down right");
                temp.transform.rotation = Quaternion.Euler(0, 0, -45);
                pm.EnableKnockBack(new Vector2(-hopModifierX, hopModifierY), hopModifierX, knockbackFallOff, knockbackFallOffDuration, knockbackDuration);
                pm.Hop(hopModifierY);
            }
            //Down
            else if(pm.dirY == PlayerMovement.DirY.Down && Mathf.Abs(pm._moveDirection.x) < 0.2f)
            {
                Debug.Log("Down");
                temp.transform.rotation = Quaternion.Euler(0, 0, -90);
                pm.Hop(hopModifierY);
            }
            //Up
            else if(pm.dirY == PlayerMovement.DirY.Up && Mathf.Abs(pm._moveDirection.x) < 0.2f)
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
