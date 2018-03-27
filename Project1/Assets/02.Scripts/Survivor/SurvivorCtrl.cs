using UnityEngine;
using System.Collections;

public class SurvivorCtrl : MonoBehaviour
{
    private Animator ani;
    Rigidbody rid;

    Vector3 movement;

    //
    private int Character;
    private int State;

    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10;
    private float Stamina = 4f;
    public float maxStamina;
    private float MoveSpeed = 4f;
    private int WorkSpeed = 10;

    float AttackTime = 0.5f;

    private bool Prison = false;
    private bool PrisonTP = false;

    //
    const int Cha_Default = 0;
    const int Cha_Stamina = 1;
    const int Cha_WorkSpeed = 2;
    const int Cha_Damage = 3;

    const int State_Idle = 0;
    const int State_SlowRun = 1;
    const int State_Run = 2;
    const int State_Hit = 3;
    const int State_Parry = 4;
    const int State_PickItem = 5;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        ani = GameObject.Find("SurvivorModel").gameObject.GetComponent<Animator>();
        rid = GetComponent<Rigidbody>();

        Character = Cha_Default;
        State = State_Idle;

        if (Character == Cha_Stamina)
        {
            Stamina = 6f;
        }
        else if (Character == Cha_WorkSpeed)
        {
            WorkSpeed = 15;
        }
        else if (Character == Cha_Damage)
        {
            Power = 15;
        }

        maxStamina = Stamina;
        GameObject.Find("GameController").GetComponent<UICtrl>().DispStamina(Stamina);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // State
        if ((ani.GetCurrentAnimatorStateInfo(0).IsName("AttackW") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("AttackL") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("Hit") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("PickItem")) &&
            ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
        {
            State = State_Idle;
        }

        if (State == State_Idle || State == State_SlowRun || State == State_Run)
        {
            if (v != 0 || h != 0)
            {
                State = State_SlowRun;
                ani.SetBool("isSlowRun", true);
            }
            else
            {
                State = State_Idle;
                ani.SetBool("isSlowRun", false);
                ani.SetBool("isRun", false);
            }
        }
        else if (State == State_AttackW || State == State_AttackL)
        {
            AttackTime -= Time.deltaTime;

            if (AttackTime > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (State == State_AttackL)
                        DamageByBoss();
                    else
                    {
                        State = State_Parry;
                        ani.SetTrigger("isAttackW");
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (State == State_AttackW)
                        DamageByBoss();
                    else
                    {
                        State = State_Parry;
                        ani.SetTrigger("isAttackL");
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }

                }
            }
            else
            {
                DamageByBoss();
            }
        }

        // Movement
        if (State == State_Run || State == State_SlowRun)
        {
            movement.Set(h, 0, v);
            movement = movement.normalized * MoveSpeed * Time.deltaTime;

            rid.MovePosition(transform.position + movement);
        }

        if (h != 0 || v != 0)
        {
            float rotationSpeed = 10f;

            Quaternion newRotation = Quaternion.LookRotation(movement);

            if (State == State_Run || State == State_SlowRun)
                rid.rotation = Quaternion.Slerp(rid.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }

        if (Prison)
            PrisonTrue();

        InputGet();

        // End
        if (Life == 0 || Hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void InputGet()
    {
        // Stamina
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 4f;

            if (State == State_Run)
            {
                State = State_SlowRun;
            }
            ani.SetBool("isRun", false);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (State == State_SlowRun || State == State_Run)
            {
                if (Stamina > 0)
                {
                    State = State_Run;
                    ani.SetBool("isRun", true);

                    MoveSpeed = 6f;
                    Stamina -= Time.deltaTime;

                    if (Stamina < 0)
                        Stamina = 0;

                    GameObject.Find("GameController").GetComponent<UICtrl>().DispStamina(Stamina);
                }
                else
                {
                    State = State_SlowRun;
                    ani.SetBool("isRun", false);
                    MoveSpeed = 4f;
                }
            }
            else
            {
                if (Stamina < maxStamina)
                {
                    Stamina += 0.1f * Time.deltaTime;
                    GameObject.Find("GameController").GetComponent<UICtrl>().DispStamina(Stamina);
                }
            }
        }
        else
        {
            if (Stamina < maxStamina)
            {
                Stamina += 0.1f * Time.deltaTime;
                GameObject.Find("GameController").GetComponent<UICtrl>().DispStamina(Stamina);
            }
        }
    }

    public void AttackByBoss(int BossState)
    {
        State = BossState;
        AttackTime = 0.5f;
    }

    void DamageByBoss()
    {
        State = State_Idle;

        if (Life != 0 && Prison == false)
        {
            if (Hp == 100f)
            {
                State = State_Hit;
                ani.SetTrigger("isHit");
                Hp = 50f;
            }
            else if (Hp == 50f)
            {
                Life -= 1;
                Prison = true;
            }
        }
    }

    public void SetAnimation(string name)
    {
        if (name == "isPickItem")
            State = State_PickItem;

        ani.SetTrigger(name);
    }

    void PrisonTrue()
    {
        Hp -= 0.5f * Time.deltaTime;

        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Prison");
            Transform minTransform = transform;
            float minDist = 10000f;

            foreach (GameObject respawn in respawns)
            {
                float dist = Vector3.Distance(transform.position, respawn.transform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    minTransform = respawn.transform;
                }
            }

            transform.position = minTransform.position;
        }
    }

    void PrisonFalse()
    {
        Prison = false;
        Hp = 50;
    }
}