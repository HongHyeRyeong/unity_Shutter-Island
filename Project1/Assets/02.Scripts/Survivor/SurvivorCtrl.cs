using UnityEngine;
using System.Collections;

public class SurvivorCtrl : MonoBehaviour
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
    private float WorkSpeed = 10f;

    public float maxStamina;
    private float saveStamina;
    private float saveWorkSpeed;

    float AttackTime = 0.5f;
    float PrisonTime = 5f;

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
    const int State_Hit = 3;
    const int State_ParryToMurderer = 4;
    const int State_PickItem = 5;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        tr = GetComponent<Transform>();
        trModel = GameObject.Find("SurvivorModel").GetComponent<Transform>();
        ani = GameObject.Find("SurvivorModel").gameObject.GetComponent<Animator>();

        State = State_Idle;

        if (Character == Cha_Stamina)
        {
            Stamina = 6f;
        }
        else if (Character == Cha_WorkSpeed)
        {
            WorkSpeed = 7f;
        }
        else if (Character == Cha_Damage)
        {
            Power = 15f;
        }

        maxStamina = Stamina;
        saveStamina = Stamina;
        saveWorkSpeed = WorkSpeed;

        GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispHP(Hp);
        GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispStamina(Stamina, maxStamina);
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
                        GameObject.Find("Murderer").GetComponent<MurdererCtrl>().DamageByPlayer(Power);
                    }

                }
            }
            else
                DamageByMurderer();
        }

        // Movement
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        if (State == State_Run || State == State_SlowRun)
        {
            // 이동
            tr.Translate(moveDir.normalized * Time.deltaTime * MoveSpeed, Space.Self);

            // 이동하면서 회전
            float angle = 0;

            if (v > 0 && h == 0) angle = 0;
            else if (v < 0 && h == 0) angle = 180;
            else if (v == 0 && h > 0) angle = 90;
            else if (v == 0 && h < 0) angle = -90;
            else if (v > 0 && h < 0) angle = -45;
            else if (v > 0 && h > 0) angle = 45;
            else if (v < 0 && h > 0) angle = 135;
            else if (v < 0 && h < 0) angle = -135;

            angle += tr.eulerAngles.y;

            Quaternion rot = Quaternion.Euler(0, angle, 0);

            trModel.rotation = Quaternion.Slerp(trModel.rotation, rot, Time.deltaTime * 10f);
        }

        // 마우스 회전
        if (State == State_Idle || State == State_Run || State == State_SlowRun)
            tr.Rotate(Vector3.up * Time.deltaTime * 100 * Input.GetAxis("Mouse X"));

        if (Prison)
            PrisonTrue();

        InputGet();
    }

    public void InputGet()
    {
        // Stamina
        float FillStaminaSpeed = 0.15f;

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 4f;
            ani.SetBool("isRun", false);

            if (State == State_Run)
            {
                State = State_SlowRun;
            }
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

                    GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispStamina(Stamina, maxStamina);
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
                    Stamina += FillStaminaSpeed * Time.deltaTime;
                    GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispStamina(Stamina, maxStamina);
                }
            }
        }
        else
        {
            if (Stamina < maxStamina)
            {
                Stamina += FillStaminaSpeed * Time.deltaTime;
                GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispStamina(Stamina, maxStamina);
            }
        }
    }

    public int GetState()
    {
        return State;
    }

    public void SetStatus(string name, float num)
    {
        if (name == "WorkSpeed")
        {
            WorkSpeed = saveWorkSpeed;
            WorkSpeed += num;
        }
        else if (name == "Stamina")
        {
            maxStamina = saveStamina;
            maxStamina += num;

            if (Stamina > maxStamina)
                Stamina = maxStamina;

            GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispStamina(Stamina, maxStamina);
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
            GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispHP(Hp);
        }
        else if (Hp == 50f)
        {
            Prison = true;
            Life -= 1;
            GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispLife(Life);
            GameObject.Find("Prison").GetComponent<PrisonCtrl>().SurvivorEnter(this.gameObject);
        }

        if (Life == 0)
            Dead();
    }

    public void PrisonStay(GameObject prison)
    {
        if (GetComponent<SurvivorItem>().ItemGet(5) == 1)
        {
            if (Input.GetKey(KeyCode.R))
            {
                PrisonTime -= Time.deltaTime;

                if (PrisonTime < 0)
                {
                    GetComponent<SurvivorItem>().ItemSet(5, 0);

                    prison.GetComponent<PrisonCtrl>().OpenDoor();
                    PrisonTime = 5f;
                }
            }
            else
            {
                PrisonTime = 5f;
            }
        }
    }

    void PrisonTrue()
    {
        Hp -= 0.5f * Time.deltaTime;
        GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispHP(Hp);

        if (Hp <= 0)
            Dead();

        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Prison");
            Transform minTransform = null;
            float minDist = 10000f;

            foreach (GameObject respawn in respawns)
            {
                if (respawn.GetComponent<PrisonCtrl>().isOpen == false)
                {
                    float dist = Vector3.Distance(transform.position, respawn.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minTransform = respawn.transform;
                    }
                }
            }

            if (minTransform != null)
                transform.position = minTransform.position;
            else
                Dead();
        }
    }

    public void PrisonFalse()
    {
        Prison = false;
        PrisonTP = false;
        Hp = 100;
        GameObject.Find("GameController").GetComponent<SurvivorUICtrl>().DispHP(Hp);
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}