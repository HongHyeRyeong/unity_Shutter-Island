using UnityEngine;
using System.Collections;
using NetworkModule;
using GameServer;

[System.Serializable]
public class AnimPlayer
{
    public AnimationClip Idle;
    public AnimationClip Run;
    public AnimationClip SlowRun;
    public AnimationClip Down;
}

public class SurvivorCtrl : MonoBehaviour
{
    public AnimPlayer anim;
    public Animation _animation;

    NetworkManager network_manager;
    
    private int Character;
    public int State;

    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10;
    private float Stamina = 4f;
    private float maxStamina;
    public float MoveSpeed = 4f;
    private int WorkSpeed = 10;
    
    float AttackTime = 0.5f;

    public bool Prison = false;
    private bool PrisonTP = false;

    //
    const int Cha_Default = 0;
    const int Cha_Stamina = 1;
    const int Cha_WorkSpeed = 2;
    const int Cha_Damage = 3;

    const int State_Idle = 0;
    const int State_SlowRun = 1;
    const int State_Run = 2;
    const int State_Down = 3;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
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

        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.Idle;
        _animation.Play();
    }

    private void Awake()
    {
        //this.network_manager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float rotationSpeed = 100f;
        transform.Rotate(0, h * rotationSpeed * Time.deltaTime, 0);

        float v = Input.GetAxis("Vertical");
        //transform.Translate(0, 0, v * MoveSpeed * Time.deltaTime);

        // State
        if (State == State_AttackW || State == State_AttackL)
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
                        State = State_Idle;
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (State == State_AttackW)
                        DamageByBoss();
                    else
                    {
                        State = State_Idle;
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }

                }
            }
            else
            {
                DamageByBoss();
            }
        }

        if (!_animation.isPlaying)
        {
            if (v != 0 || h != 0)
                State = State_SlowRun;
            else
                State = State_Idle;
        }

        if (Prison)
            PrisonTrue();

        InputGet();

        AnimationByState();

        // End
        if (Life == 0 || Hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void AnimationByState()
    {
        if (State == State_Idle)
            _animation.CrossFade(anim.Idle.name, 0.2f);
        else if (State == State_SlowRun)
            _animation.CrossFade(anim.SlowRun.name, 0.2f);
        else if (State == State_Run)
            _animation.CrossFade(anim.Run.name, 0.2f);
        else if (State == State_Down)
            _animation.CrossFade(anim.Down.name, 0.2f);
        else if (State == State_AttackW)
            _animation.CrossFade(anim.Idle.name, 0.2f);
        else if (State == State_AttackL)
            _animation.CrossFade(anim.Idle.name, 0.2f);
    }

    public void InputGet()
    {
        // Stamina
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 4f;
            State = State_SlowRun;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Stamina > 0)
            {
                MoveSpeed = 6f;
                State = State_Run;
                Stamina -= Time.deltaTime;
            }
            else
            {
                MoveSpeed = 4f;
                State = State_SlowRun;
            }
        }
        else
        {
            if (Stamina < maxStamina)
                Stamina += 0.1f * Time.deltaTime;
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
                State = State_Down;
                Hp = 50f;
            }
            else if (Hp == 50f)
            {
                Life -= 1;
                Prison = true;
            }
        }
    }

    void PrisonTrue()
    {
        Hp -= 0.5f * Time.deltaTime;
        //print("HP: " + Hp);

        if (PrisonTP == false)
        {
            PrisonTP = true;
            _animation.CrossFade(anim.Idle.name, 0.3f);

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