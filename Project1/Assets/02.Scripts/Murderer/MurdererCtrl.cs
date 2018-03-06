using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererCtrl : MonoBehaviour
{
    private Animator ani;

    private int State;
    private float Hp = 200f;
    float MoveSpeed = 4.5f;
    bool isAttack = false;

    // 카메라
    private float x;
    float xSpeed = 100.0f;

    //
    const int State_Idle = 0;
    const int State_Run = 1;
    const int State_Parry = 2;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        ani = this.gameObject.GetComponent<Animator>();

        State = State_Idle;
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;

        if (State == State_Run)
            transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);


        if (State != State_Parry)
        {
            Quaternion rotation = Quaternion.Euler(0, x, 0);
            transform.rotation = rotation;
        }

        // State
        if ((ani.GetCurrentAnimatorStateInfo(0).IsName("AttackW") ||
                ani.GetCurrentAnimatorStateInfo(0).IsName("AttackL") ||
                ani.GetCurrentAnimatorStateInfo(0).IsName("Parry")) &&
                ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            ani.SetBool("isAttackW", false);
            ani.SetBool("isAttackL", false);
            ani.SetBool("isParry", false);
            isAttack = false;

            State = State_Idle;
        }

        if (State == State_Idle || State == State_Run)
        {
            if (v != 0 || h != 0)
            {
                State = State_Run;
                ani.SetBool("isRun", true);
            }
            else
            {
                State = State_Idle;
                ani.SetBool("isRun", false);
            }
        }

        if (State != State_AttackW && State != State_AttackL && State != State_Parry)
        {
            if (Input.GetMouseButtonDown(0))
            {
                State = State_AttackW;
                ani.SetBool("isAttackW", true);
                isAttack = true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                State = State_AttackL;
                ani.SetBool("isAttackL", true);
                isAttack = true;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            col.gameObject.GetComponent<SurvivorCtrl>().AttackByBoss(State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        State = State_Parry;
        ani.SetBool("isParry", true);
        Hp -= Power;
    }
}
