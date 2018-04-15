using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererCtrl : MonoBehaviour
{
    private Animator ani;

    private int State = 0;
    private float Hp = 200f;
    private float MoveSpeed = 4.5f;
    private bool isAttack = false;

    private float MouseX;

    //
    const int State_Idle = 0;
    const int State_Run = 1;
    const int State_Parry = 2;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // State
        if ((ani.GetCurrentAnimatorStateInfo(0).IsName("AttackW") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("AttackL") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("Parry")) &&
            ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            State = State_Idle;
        }

        if (State == State_Idle || State == State_Run)
        {
            if (v != 0 || h != 0)
            {
                State = State_Run;

                if (v < 0)
                {
                    ani.SetBool("isBackRun", true);
                    ani.SetBool("isRun", false);
                }
                else
                {
                    ani.SetBool("isRun", true);
                    ani.SetBool("isBackRun", false);
                }
            }
            else
            {
                State = State_Idle;
                ani.SetBool("isRun", false);
                ani.SetBool("isBackRun", false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                State = State_AttackW;
                ani.SetTrigger("isAttackW");
                isAttack = true;
            }
            else if (Input.GetMouseButtonDown(1))
            {
                State = State_AttackL;
                ani.SetTrigger("isAttackL");
                isAttack = true;
            }
        }

        // Movement
        if (State == State_Run)
            transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);

        if (State != State_Parry)
        {
            MouseX += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
            transform.rotation = Quaternion.Euler(0, MouseX, 0);
        }

        // 
        if(Input.GetKey(KeyCode.E) && State != State_Parry)
        {
            State = State_Parry;
            ani.SetTrigger("isParry");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            other.gameObject.GetComponent<test>().AttackByMurderer(State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        State = State_Parry;
        ani.SetTrigger("isParry");
        Hp -= Power;
    }

    public int GetState()
    {
        return State;
    }
}
