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
        ani = GameObject.Find("MurdererModel").gameObject.GetComponent<Animator>();

        State = State_Idle;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        // State
        if ((ani.GetCurrentAnimatorStateInfo(0).IsName("AttackW") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("AttackL") ||
            ani.GetCurrentAnimatorStateInfo(0).IsName("Parry")) &&
            ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
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
        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;

        if (State == State_Run)
            transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);


        if (State != State_Parry)
        {
            Quaternion rotation = Quaternion.Euler(0, x, 0);
            transform.rotation = rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            other.gameObject.GetComponent<SurvivorCtrl>().AttackByMurderer(State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        State = State_Parry;
        ani.SetTrigger("isParry");
        Hp -= Power;
    }
}
