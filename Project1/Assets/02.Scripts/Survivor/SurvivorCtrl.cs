using UnityEngine;
using System.Collections;

public class SurvivorCtrl : MonoBehaviour
{
    private PhotonView pv = null;   // PhotonView 컴포넌트를 할당할 변수

    // 위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;

    //
    private Animator Ani;
    private Transform trModel;

    SurvivorUICtrl SurvivorUI;
    GameObject Murderer;

    //
    public int Type;
    private int State = 0;

    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10f;
    private float Stamina = 4f;
    private float MoveSpeed = 4f;
    private float WorkSpeed = 1f;

    float maxStamina;
    float saveStamina;
    float saveWorkSpeed;

    int Attack = 0;
    int WorkMachine = 0;

    float AttackTime = 1f;
    float PrisonTime = 3f;

    bool Prison = false;
    bool PrisonTP = false;
    GameObject inPrison = null;

    //
    const int Cha_Default = 0;
    const int Cha_Stamina = 1;
    const int Cha_WorkSpeed = 2;
    const int Cha_Damage = 3;

    const int State_Die = -1;
    const int State_Idle = 0;
    const int State_SlowRun = 1;
    const int State_Run = 2;
    const int State_Hit = 3;
    const int State_PickItem = 4;
    const int State_Repair = 5;
    const int State_AttackW = 10;
    const int State_AttackL = 11;
    const int State_ParryToMurdererW = 12;
    const int State_ParryToMurdererL = 13;

    void Start()
    {
        // PhotonView 컴포넌트 할당
        pv = GetComponent<PhotonView>();
        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        // PhotonView Observed Components 속성에 Ctrl 스크립트 연결
        pv.ObservedComponents[0] = this;

        // 원격 플레이어의 위치 및 회전 값을 처리할 변수의 초기값 설정
        currPos = transform.position;
        currRot = transform.rotation;

        Ani = this.gameObject.transform.Find("SurvivorModel").GetComponent<Animator>();

        if (pv.isMine)
        {
            trModel = this.gameObject.transform.Find("SurvivorModel").GetComponent<Transform>();
            SurvivorUI = GameObject.Find("SurvivorController").GetComponent<SurvivorUICtrl>();

            GameObject.Find("MainCamera").GetComponent<CameraCtrl>().targetSurvivorComPivot =
                this.gameObject.transform.Find("SurvivorCamPivot");

            Type = Cha_Stamina;  // Demo

            if (Type == Cha_Stamina)
            {
                Stamina = 10f;  // Demo 6
            }
            else if (Type == Cha_WorkSpeed)
            {
                WorkSpeed = 1.1f;
            }
            else if (Type == Cha_Damage)
            {
                Power = 15f;
            }

            maxStamina = Stamina;
            saveStamina = Stamina;
            saveWorkSpeed = WorkSpeed;

            SurvivorUI.DispHP(Hp);
            SurvivorUI.DispStamina(Stamina, maxStamina);
        }
    }

    void Update()
    {
        if (pv.isMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            // State
            if (State == State_Idle || State == State_SlowRun || State == State_Run)
            {
                if (v != 0 || h != 0)
                {
                    State = State_SlowRun;
                    Ani.SetBool("isSlowRun", true);
                }
                else
                {
                    State = State_Idle;
                    Ani.SetBool("isSlowRun", false);
                    Ani.SetBool("isRun", false);
                }

                if (!SurvivorUI.Inven.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        State = State_AttackW;
                        Ani.SetTrigger("trAttackW");
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        State = State_AttackL;
                        Ani.SetTrigger("trAttackL");
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                }
            }

            // Movement
            Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

            if (State == State_Run || State == State_SlowRun)
            {
                transform.Translate(moveDir.normalized * Time.deltaTime * MoveSpeed, Space.Self);

                float angle = 0;

                if (v > 0 && h == 0) angle = 0;
                else if (v < 0 && h == 0) angle = 180;
                else if (v == 0 && h > 0) angle = 90;
                else if (v == 0 && h < 0) angle = -90;
                else if (v > 0 && h < 0) angle = -45;
                else if (v > 0 && h > 0) angle = 45;
                else if (v < 0 && h > 0) angle = 135;
                else if (v < 0 && h < 0) angle = -135;

                angle += transform.eulerAngles.y;

                Quaternion rot = Quaternion.Euler(0, angle, 0);
                trModel.rotation = Quaternion.Slerp(trModel.rotation, rot, Time.deltaTime * 10f);
            }

            if (State == State_Idle || State == State_Run || State == State_SlowRun)
                transform.Rotate(Vector3.up * Time.deltaTime * 100 * Input.GetAxis("Mouse X"));

            InputGet();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);
        }

        if (Prison)
            pv.RPC("PrisonTrue", PhotonTargets.All);
    }

    public void InputGet()
    {
        // Stamina
        float FillStaminaSpeed = 0.15f;

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 4f;
            Ani.SetBool("isRun", false);

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
                    Ani.SetBool("isRun", true);

                    MoveSpeed = 6f;
                    Stamina -= Time.deltaTime;

                    if (Stamina < 0)
                        Stamina = 0;

                    SurvivorUI.DispStamina(Stamina, maxStamina);
                }
                else
                {
                    State = State_SlowRun;
                    Ani.SetBool("isRun", false);
                    MoveSpeed = 4f;
                }
            }
            else
            {
                if (Stamina < maxStamina)
                {
                    Stamina += FillStaminaSpeed * Time.deltaTime;
                    SurvivorUI.DispStamina(Stamina, maxStamina);
                }
            }
        }
        else
        {
            if (Stamina < maxStamina)
            {
                Stamina += FillStaminaSpeed * Time.deltaTime;
                SurvivorUI.DispStamina(Stamina, maxStamina);
            }
        }

        // Attack
        if (Attack != 0)
        {
            AttackTime -= Time.deltaTime;

            if (AttackTime > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Attack == State_AttackL)
                    {
                        DamageByMurderer();
                    }
                    else
                    {
                        State = State_ParryToMurdererW;
                        pv.RPC("ParryWAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (Attack == State_AttackW)
                    {
                        DamageByMurderer();
                    }
                    else
                    {
                        State = State_ParryToMurdererL;
                        pv.RPC("ParryLAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                    }

                }
            }
            else
            {
                if (Attack != 0)
                    DamageByMurderer();
            }
        }
    }

    [PunRPC]
    public void AttackEnd()
    {
        Attack = 0;
    }

    [PunRPC]
    public void DamageToMurderer()
    {
        Murderer.GetComponent<MurdererCtrl>().DamageByPlayer(Power);
    }

    public void AttackByMurderer(GameObject m, int MurdererAttack)
    {
        if (!Prison)
        {
            if (Murderer == null)
            {
                Murderer = m;
            }

            if (State == State_AttackW || State == State_AttackL)
            {
                if (State == MurdererAttack)
                {
                    if (State == State_AttackW)
                    {
                        State = State_ParryToMurdererW;
                        Ani.SetTrigger("trAttackW");
                    }
                    else
                    {
                        State = State_ParryToMurdererL;
                        Ani.SetTrigger("trAttackL");
                    }
                    Ani.SetBool("isSlowRun", false);
                    Ani.SetBool("isRun", false);

                    pv.RPC("DamageToMurderer", PhotonTargets.All);
                }
                else
                {
                    DamageByMurderer();
                }
            }
            else
            {
                Attack = MurdererAttack;
                AttackTime = 1f;
            }
        }
    }

    void DamageByMurderer()
    {
        State = State_Idle;
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);
        pv.RPC("AttackEnd", PhotonTargets.All);

        if (Hp == 100f)
        {
            Hp = 50f;
            State = State_Hit;
            pv.RPC("HitAnim", PhotonTargets.All);

            if (pv.isMine)
                SurvivorUI.DispHP(Hp);

        }
        else if (Hp == 50f)
        {
            Prison = true;
            Life -= 1;

            if (pv.isMine)
                SurvivorUI.DispLife(Life);
        }

        if (Life == 0)
        {
            inPrison.GetComponent<PrisonCtrl>().SurvivorExit(this.gameObject);
            pv.RPC("DieAnim", PhotonTargets.All);
        }
    }

    public void PrisonStay(GameObject prison)
    {
        if (GetComponent<SurvivorItem>().ItemGet(5) == 1)
        {
            if (Input.GetKey(KeyCode.R))
            {
                if (SurvivorUI.Message.activeSelf)
                    SurvivorUI.Message.SetActive(false);
                SurvivorUI.DisTime(PrisonTime, 3);

                PrisonTime -= Time.deltaTime;

                if (PrisonTime < 0)
                {
                    SurvivorUI.Time.SetActive(false);

                    GetComponent<SurvivorItem>().ItemSet(5, 0);

                    prison.GetComponent<PrisonCtrl>().OpenDoor();
                    PrisonTime = 3f;
                }
            }
            else
            {
                if (pv.isMine)
                {
                    if (!SurvivorUI.Message.activeSelf)
                        SurvivorUI.DisMessage(3);

                    if (SurvivorUI.Time.activeSelf)
                        SurvivorUI.Time.SetActive(false);
                }

                PrisonTime = 3f;
            }
        }
    }

    public void PrisonExit()
    {
        PrisonTime = 3f;

        SurvivorUI.Message.SetActive(false);
        SurvivorUI.Time.SetActive(false);
    }

    [PunRPC]
    void PrisonTrue()
    {
        Hp -= 3.0f * Time.deltaTime;    // Demo

        if(pv.isMine)
            SurvivorUI.DispHP(Hp);

        if (Hp <= 0)
        {
            inPrison.GetComponent<PrisonCtrl>().SurvivorExit(this.gameObject);
            pv.RPC("DieAnim", PhotonTargets.All);
            Prison = false;
        }

        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject[] respawns = GameObject.FindGameObjectsWithTag("Prison");
            GameObject minObject = null;
            float minDist = 10000f;

            foreach (GameObject respawn in respawns)
            {
                if (respawn.GetComponent<PrisonCtrl>().GetOpen() == false)
                {
                    float dist = Vector3.Distance(transform.position, respawn.transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minObject = respawn;
                    }
                }
            }

            if (minObject != null)
            {
                inPrison = minObject;
                minObject.GetComponent<PrisonCtrl>().SurvivorEnter(this.gameObject);
                transform.position = minObject.transform.position;

                currPos = transform.position;
            }
            else
            {
                inPrison.GetComponent<PrisonCtrl>().SurvivorExit(this.gameObject);
                pv.RPC("DieAnim", PhotonTargets.All);
            }
        }
    }

    public void PrisonFalse()
    {
        Prison = false;
        PrisonTP = false;

        Hp = 50;
        SurvivorUI.DispHP(Hp);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Machine")
        {
            GameObject machine = other.gameObject.GetComponent<MachineRangeCtrl>().Machine;

            if (machine.gameObject.GetComponent<MachineCtrl>().GetComplete())
                return;

            machine.GetComponent<MachineCtrl>().DisHUD(transform.position);

            if (other.gameObject.GetComponent<MachineRangeCtrl>().GetMachineUse())
            {
                if (WorkMachine == machine.gameObject.GetComponent<MachineCtrl>().MachineNum)
                {
                    if (Input.GetKey(KeyCode.R))
                    {
                        if (State != State_Repair)
                        {
                            State = State_Repair;
                            Ani.SetBool("isRepair", true);
                            Ani.SetBool("isSlowRun", false);
                            Ani.SetBool("isRun", false);
                        }
                        else
                        {
                            transform.Rotate(Vector3.up * Time.deltaTime * 100 * Input.GetAxis("Mouse X"));
                            trModel.rotation = Quaternion.Euler(other.transform.eulerAngles);
                        }

                        bool complete = machine.gameObject.GetComponent<MachineCtrl>().Install(Time.deltaTime * WorkSpeed);

                        if (complete)
                        {
                            State = State_Idle;
                            Ani.SetBool("isRepair", false);
                            WorkMachine = 0;

                            other.gameObject.GetComponent<MachineRangeCtrl>().SetMachineUse(false);
                        }
                    }
                    else
                    {
                        State = State_Idle;
                        Ani.SetBool("isRepair", false);
                        WorkMachine = 0;

                        other.gameObject.GetComponent<MachineRangeCtrl>().SetMachineUse(false);
                    }
                }
            }
            else
            {
                if (machine.gameObject.GetComponent<MachineCtrl>().GetGadgetUse())
                {
                    if (!SurvivorUI.Message.activeSelf)
                        SurvivorUI.DisMessage(2);

                    if (Input.GetKey(KeyCode.R))
                    {
                        SurvivorUI.Message.SetActive(false);
                        other.gameObject.GetComponent<MachineRangeCtrl>().SetMachineUse(true);
                        WorkMachine = machine.gameObject.GetComponent<MachineCtrl>().MachineNum;
                    }
                }
                else
                {
                    int GadgetNum = GetComponent<SurvivorItem>().ItemGet(4);

                    if (GadgetNum > 0)
                    {
                        if (!SurvivorUI.Message.activeSelf)
                            SurvivorUI.DisMessage(1);

                        if (Input.GetKeyDown(KeyCode.T))
                        {
                            SurvivorUI.Message.SetActive(false);
                            machine.gameObject.GetComponent<MachineCtrl>().SetGadgetUse(true);
                            GetComponent<SurvivorItem>().ItemSet(4, GadgetNum - 1);
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Machine")
        {
            if (SurvivorUI.Message.activeSelf)
                SurvivorUI.Message.SetActive(false);

            other.gameObject.GetComponent<MachineRangeCtrl>().Machine.GetComponent<MachineCtrl>().SetHUD(false);
        }
    }

    public void SetState(int s)
    {
        State = s;
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);

        if (State == State_PickItem)
        {
            State = State_PickItem;
            pv.RPC("PickItemAnim", PhotonTargets.All);
        }
        else if (State == State_Die)
            gameObject.SetActive(false);
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

            SurvivorUI.DispStamina(Stamina, maxStamina);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 플레이어의 정보 송신
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else // 원격 플레이어의 정보 송신
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
        }
    }

    [PunRPC]
    public void ParryWAnim()
    {
        Ani.SetTrigger("trParryW");
    }

    [PunRPC]
    public void ParryLAnim()
    {
        Ani.SetTrigger("trParryL");
    }

    [PunRPC]
    public void HitAnim()
    {
        Ani.SetTrigger("trHit");
    }

    [PunRPC]
    public void PickItemAnim()
    {
        Ani.SetTrigger("trPickItem");
    }

    [PunRPC]
    public void DieAnim()
    {
        Ani.SetTrigger("trDie");
    }
}