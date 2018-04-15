using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{ 
    private Transform tr;
    private Transform trModel;
    private Animator ani;

    //
    public int Character = 0;
    private int State;

    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10f;
    private float Stamina = 4f;
    private float MoveSpeed = 4f;
    private float WorkSpeed = 1f;

    float maxStamina;
    float saveStamina;
    float saveWorkSpeed;

    int WorkMachine = 0;

    float AttackTime = 0.5f;
    float PrisonTime = 5f;

    bool Prison = false;
    bool PrisonTP = false;

    //
    const int Cha_Default = 0;
    const int Cha_Stamina = 1;
    const int Cha_WorkSpeed = 2;
    const int Cha_Damage = 3;

    const int State_Idle = 0;
    const int State_SlowRun = 1;
    const int State_Run = 2;
    const int State_Hit = 3;
    const int State_ParryToMurderer = 4;
    const int State_PickItem = 5;
    const int State_Repair = 6;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        tr = GetComponent<Transform>();
        ani = GameObject.Find("idleClothes").gameObject.GetComponent<Animator>();

        State = State_Idle;

        if (Character == Cha_Stamina)
        {
            Stamina = 6f;
        }
        else if (Character == Cha_WorkSpeed)
        {
            WorkSpeed = 1.1f;
        }
        else if (Character == Cha_Damage)
        {
            Power = 15f;
        }

        maxStamina = Stamina;
        saveStamina = Stamina;
        saveWorkSpeed = WorkSpeed;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // State
        if ((ani.GetCurrentAnimatorStateInfo(0).IsName("AttackW") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("AttackL") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("Hit") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("PickItem")) &&
            ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
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
                        DamageByMurderer();
                    else
                    {
                        State = State_ParryToMurderer;
                        ani.SetTrigger("isAttackW");
                        ani.SetBool("isSlowRun", false);
                        ani.SetBool("isRun", false);
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (State == State_AttackW)
                        DamageByMurderer();
                    else
                    {
                        State = State_ParryToMurderer;
                        ani.SetTrigger("isAttackL");
                        ani.SetBool("isSlowRun", false);
                        ani.SetBool("isRun", false);
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }

                }
            }
            else
                DamageByMurderer();
        }
    }

    public int GetState()
    {
        return State;
    }

    public void SetAnimation(string name)
    {
        if (name == "isPickItem")
        {
            State = State_PickItem;
            ani.SetBool("isSlowRun", false);
            ani.SetBool("isRun", false);
        }

        ani.SetTrigger(name);
    }

    public void AttackByMurderer(int MurdererState)
    {
        if (!Prison)
        {
            State = MurdererState;
            AttackTime = 0.5f;
        }
    }

    void DamageByMurderer()
    {
        State = State_Idle;

        if (Hp == 100f)
        {
            State = State_Hit;
            ani.SetTrigger("isHit");
            Hp = 50f;
        }
        else if (Hp == 50f)
        {
            Prison = true;
            Life -= 1;
        }

        if (Life == 0)
            Dead();
    }

    void PrisonTrue()
    {
        Hp -= 0.5f * Time.deltaTime;

        if (Hp <= 0)
            Dead();

        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Prison");
            GameObject minObject = null;
            float minDist = 10000f;

            foreach (GameObject respawn in respawns)
            {
                if (respawn.GetComponent<PrisonCtrl>().isOpen == false)
                {
                    float dist = Vector3.Distance(transform.position, respawn.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minObject = respawn;
                    }
                }
            }

            if (minObject != null)
            {
                minObject.GetComponent<PrisonCtrl>().SurvivorEnter(this.gameObject);
                transform.position = minObject.transform.position;
            }
            else
                Dead();
        }
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
