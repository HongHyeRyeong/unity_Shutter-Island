using UnityEngine;
using System.Collections;

public class SurvivorCtrl : MonoBehaviour
{
    private Transform tr;
    private Transform trModel;
    private Animator ani;

    int trModelNum = 1;
    float[] trModelRot = new float[8] { 0, 180, 90, -90, -45, 45, 135, -135 };

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
        tr = GetComponent<Transform>();
        trModel = GameObject.Find("SurvivorModel").GetComponent<Transform>();
        ani = GameObject.Find("SurvivorModel").gameObject.GetComponent<Animator>();

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
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        if (State == State_Run || State == State_SlowRun)
            tr.Translate(moveDir * Time.deltaTime * MoveSpeed, Space.Self);

        if (State == State_Idle || State == State_Run || State == State_SlowRun)
            tr.Rotate(Vector3.up * Time.deltaTime * 100 * Input.GetAxis("Mouse X"));

        if (State == State_Run || State == State_SlowRun)
        {
            if (h != 0 || v != 0)
            {
                if (v > 0 && h == 0 && trModelNum != 1)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[0];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 1;
                }
                else if (v < 0 && h == 0 && trModelNum != 2)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[1];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 2;
                }
                else if (v == 0 && h > 0 && trModelNum != 3)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[2];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 3;
                }
                else if (v == 0 && h < 0 && trModelNum != 4)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[3];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 4;
                }
                else if (v > 0 && h < 0 && trModelNum != 5)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[4];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 5;
                }
                else if (v > 0 && h > 0 && trModelNum != 6)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[5];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 6;
                }
                else if (v < 0 && h > 0 && trModelNum != 7)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[6];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 7;
                }
                else if (v < 0 && h < 0 && trModelNum != 8)
                {
                    float angle = -trModelRot[trModelNum - 1] + trModelRot[7];
                    trModel.RotateAround(tr.position, Vector3.up * Time.deltaTime, angle);
                    trModelNum = 8;
                }
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
        {
            State = State_PickItem;
            ani.SetBool("isSlowRun", false);
            ani.SetBool("isRun", false);
        }

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