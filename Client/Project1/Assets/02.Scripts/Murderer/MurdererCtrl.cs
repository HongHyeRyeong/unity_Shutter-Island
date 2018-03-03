using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimBoss
{
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip AttackW;
    public AnimationClip AttackL;
    public AnimationClip Parry;
}

public class MurdererCtrl : MonoBehaviour
{
    public AnimBoss anim;
    public Animation _animation;

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
        State = State_Idle;

        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.Idle;
        _animation.Play();
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;

        if (State != State_AttackW && State != State_AttackL && State != State_Parry)
        {
            transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(0, x, 0);
        transform.rotation = rotation;

        // State
        if (_animation.isPlaying)
        {
            if (State != State_AttackW && State != State_AttackL && State != State_Parry)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    State = State_AttackW;
                    isAttack = true;
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    State = State_AttackL;
                    isAttack = true;
                }
            }
        }
        else
        {
            State = State_Idle;
            isAttack = false;
        }

        if (State == State_Idle)
        {
            if (v != 0 || h != 0)
                State = State_Run;
        }
        else if (State == State_Run)
        {
            if (v == 0 && h == 0)
                State = State_Idle;
        }

        AnimationByState();
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
        Hp -= Power;
    }

    void AnimationByState()
    {
        if (State == State_Idle)
            _animation.CrossFade(anim.Idle.name, 0.2f);
        else if (State == State_Run)
            _animation.CrossFade(anim.Run.name, 0.2f);
        else if (State == State_AttackW)
            _animation.CrossFade(anim.AttackW.name, 0.2f);
        else if (State == State_AttackL)
            _animation.CrossFade(anim.AttackL.name, 0.2f);
        else if (State == State_Parry)
            _animation.CrossFade(anim.Parry.name, 0.2f);
    }
}
