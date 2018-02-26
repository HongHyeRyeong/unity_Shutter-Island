using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimBoss
{
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip AttackUp;
    public AnimationClip Tired;
    public AnimationClip Down;
}

public class BossCtrl : MonoBehaviour
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
    const int State_AttackUp = 2;
    const int State_AttackDown = 3;
    const int State_Damage = 4;

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

        if (State != State_AttackUp && State != State_AttackDown)
        {
            transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);
        }

        Quaternion rotation = Quaternion.Euler(0, x, 0);
        transform.rotation = rotation;

        // Animation
        if (_animation.isPlaying)
        {
            if (State != State_AttackUp && State != State_AttackDown && State != State_Damage)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    State = State_AttackUp;
                    isAttack = true;
                    _animation.CrossFade(anim.AttackUp.name, 0.2f);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    State = State_AttackDown;
                    isAttack = true;
                    _animation.CrossFade(anim.AttackUp.name, 0.2f);
                }
            }

            if (State == State_Damage)
            {
                _animation.CrossFade(anim.Down.name, 0.2f);
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
            {
                State = State_Run;
                _animation.CrossFade(anim.Run.name, 0.2f);
            }
            else
            {
                _animation.CrossFade(anim.Idle.name, 0.2f);
            }
        }
        else if (State == State_Run)
        {
            if (v == 0 && h == 0)
            {
                State = State_Idle;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player" && isAttack)
        {
            isAttack = false;
            col.gameObject.GetComponent<PlayerCtrl>().AttackByBoss(State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        print("aa");
        State = State_Damage;
        Hp -= Power;
    }
}
