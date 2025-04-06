using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttack : MonoBehaviour
{
    //determines if player should do a combo
    public int animationCount = 0;
    public float[] basicAnimationTime = new float[3];
    public bool enableAttack = true;
    public float[] basicAttackLunge = new float[3];
    //determines player directoin
    int facing = 0;

    public int animationCountMax = 3;

    PlayerMovement pm;
    Rigidbody2D rb;

    [SerializeField] GameObject[] hitBoxSpawnBasic = new GameObject[3];

    public InputActionReference attack;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        facing = 1;
        enableAttack = true;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            facing = -1;
        }
        if (Input.GetKeyDown(KeyCode.D)) 
        {
            facing = 1;
        }
        if (animationCount == animationCountMax)
        {
            animationCount = 0;
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
        if (enableAttack) 
        {
            pm.Lunge(basicAttackLunge[animationCount]);
            GameObject temp = Instantiate(hitBoxSpawnBasic[animationCount],new Vector2(transform.position.x + facing,transform.position.y),Quaternion.identity,transform);
            if (facing == 1)
            {
                temp.transform.localScale *= new Vector2(facing, facing);
            }
            if (facing == -1)
            {
                temp.transform.localScale *= new Vector2(facing, -facing);
            }

            Destroy(temp,0.25f);
            StartCoroutine(WaitAnimation(basicAnimationTime[animationCount]));
        }      
    }

    IEnumerator WaitAnimation(float time)
    {
        enableAttack = false;
        yield return new WaitForSeconds(time);
        enableAttack = true;
        animationCount++;
    }
}
