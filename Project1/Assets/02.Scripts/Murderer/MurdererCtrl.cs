using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererCtrl : MonoBehaviour
{
    private Animator Ani;

    MurdererUICtrl MurdererUI;

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
        GameObject.Find("GameController").GetComponent<GameCtrl>().SetGame(2, null, this.gameObject);

        //
        Ani = GetComponent<Animator>();
        MurdererUI = GameObject.Find("MurdererController").GetComponent<MurdererUICtrl>();

        GameObject.Find("MainCamera").GetComponent<CameraCtrl>().targetMurderer = this.gameObject.transform;
        GameObject.Find("MainCamera").GetComponent<CameraCtrl>().targetMurdererCamPivot =
            this.gameObject.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 Head/MurdererCamPivot").transform;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // State
        if (State == State_Idle || State == State_Run)
        {
            if (v != 0 || h != 0)
            {
                State = State_Run;

                if (v < 0)
                {
                    MoveSpeed = 3f;
                    Ani.SetBool("isBackRun", true);
                    Ani.SetBool("isRun", false);
                }
                else
                {
                    MoveSpeed = 4.5f;
                    Ani.SetBool("isRun", true);
                    Ani.SetBool("isBackRun", false);
                }
            }
            else
            {
                State = State_Idle;
                Ani.SetBool("isRun", false);
                Ani.SetBool("isBackRun", false);
            }

            if (Input.GetMouseButtonDown(0))
            {
                State = State_AttackW;
                Ani.SetTrigger("trAttackW");
                Ani.SetBool("isRun", false);
                Ani.SetBool("isBackRun", false);

                isAttack = true;
                MurdererUI.DisAttackBack(0, true);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                State = State_AttackL;
                Ani.SetTrigger("trAttackL");
                Ani.SetBool("isRun", false);
                Ani.SetBool("isBackRun", false);

                isAttack = true;
                MurdererUI.DisAttackBack(1, true);
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

        if (Hp < 0)
            Dead();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            other.gameObject.GetComponent<SurvivorCtrl>().AttackByMurderer(this.gameObject, State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        State = State_Parry;
        Ani.SetTrigger("trParry");
        Ani.SetBool("isRun", false);
        Ani.SetBool("isBackRun", false);

        Hp -= Power;
        MurdererUI.DispHP(Hp);
    }

    public void DamageByMachine(float Power)
    {
        Hp -= Power;
        MurdererUI.DispHP(Hp);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }

    public int GetState()
    {
        return State;
    }

    public void SetState(int s)
    {
        State = s;
        Ani.SetBool("isRun", false);
        Ani.SetBool("isBackRun", false);

        MurdererUI.DisAttackBack(0, false);
        MurdererUI.DisAttackBack(1, false);
    }
}
