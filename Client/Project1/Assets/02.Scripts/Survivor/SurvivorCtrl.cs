using UnityEngine;
using System.Collections;
using NetworkModule;
using GameServer;

public class SurvivorCtrl : MonoBehaviour
{
    NetworkManager network_manager;

    private Animator ani;

    private int Character;
    private int State;

    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10;
    private float Stamina = 4f;
    private float maxStamina;
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
    const int State_Down = 3;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        ani = this.gameObject.GetComponent<Animator>();

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
        transform.Translate(0, 0, v * MoveSpeed * Time.deltaTime);

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
                        ani.SetBool("isIdle", true);    // 반격
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
                        ani.SetBool("isIdle", true);    // 반격
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }

                }
            }
            else
            {
                DamageByBoss();
            }
        }
        else
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
                }
                else
                {
                    State = State_SlowRun;
                    ani.SetBool("isRun", false);
                    MoveSpeed = 4f;
                }
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
                ani.SetTrigger("isDown");
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