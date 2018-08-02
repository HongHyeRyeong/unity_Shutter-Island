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
    private SurvivorItem Item;

    private int State = 0;
    private int Life = 2;
    private float Hp = 100f;
    private float Power = 10f;
    private float Stamina = 4f;
    private float MoveSpeed = 5f;
    private float WorkSpeed = 1f;

    float maxStamina;
    float saveStamina;
    float saveWorkSpeed;

    int Attack = 0;
    int WorkMachine = 0;

    float AttackTime = 0.8f;
    float PrisonTime = 3f;

    bool Prison = false;
    bool PrisonTP = false;
    GameObject inPrison = null;

    public bool Trap = false;

    private Vector3 SaveRot;

    //
    const int State_Die = -1;
    const int State_Idle = 0;
    const int State_SlowRun = 1;
    const int State_Run = 2;
    const int State_Hit = 3;
    const int State_PickItem = 4;
    const int State_Repair = 5;
    const int State_Trap = 6;
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

        Ani = transform.Find("SurvivorModel").GetComponent<Animator>();
        trModel = transform.Find("SurvivorModel").GetComponent<Transform>();
        Item = GetComponent<SurvivorItem>();

        if (pv.isMine)
        {
            CameraCtrl.instance.transform.position = transform.position;
            CameraCtrl.instance.targetSurvivorComPivot = transform.Find("SurvivorCamPivot");

            if (LobbyCtrl.instance.SurStat == 1)
            {
                Stamina = 6f;
                pv.RPC("ChangeMaterial1", PhotonTargets.AllBuffered);
            }
            else if (LobbyCtrl.instance.SurStat == 2)
            {
                WorkSpeed = 1.1f;
                pv.RPC("ChangeMaterial2", PhotonTargets.AllBuffered);
            }
            else if (LobbyCtrl.instance.SurStat == 3)
            {
                Power = 15f;
                pv.RPC("ChangeMaterial3", PhotonTargets.AllBuffered);
            }

            maxStamina = Stamina;
            saveStamina = Stamina;
            saveWorkSpeed = WorkSpeed;

            SurvivorUICtrl.instance.DispHP(Hp);
            SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);

            SoundManager.instance.SetBGM("Ingame6-In My Nightmares");
        }
    }

    [PunRPC]
    public void ChangeMaterial1()
    {
        SkinnedMeshRenderer meshrender = transform.Find("SurvivorModel/low001").GetComponent<SkinnedMeshRenderer>();
        meshrender.material = GameCtrl.instance.Msurvivor[0];
    }

    [PunRPC]
    public void ChangeMaterial2()
    {
        SkinnedMeshRenderer meshrender = transform.Find("SurvivorModel/low001").GetComponent<SkinnedMeshRenderer>();
        meshrender.material = GameCtrl.instance.Msurvivor[1];
    }

    [PunRPC]
    public void ChangeMaterial3()
    {
        SkinnedMeshRenderer meshrender = transform.Find("SurvivorModel/low001").GetComponent<SkinnedMeshRenderer>();
        meshrender.material = GameCtrl.instance.Msurvivor[2];
    }

    void Update()
    {
        if (pv.isMine)
        {
            if (!GameCtrl.instance.isStart)
                return;

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

                if (!SurvivorUICtrl.instance.Inven.activeSelf && !SettingManager.instance.Setting.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        State = State_AttackW;
                        pv.RPC("AttackWAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        State = State_AttackL;
                        pv.RPC("AttackLAnim", PhotonTargets.All);
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
            else if (State == State_Repair || State == State_Trap)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 100 * Input.GetAxis("Mouse X"));
                trModel.rotation = Quaternion.Euler(SaveRot);
            }

            InputGet();
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f);
        }

        if (Prison)
            pv.RPC("PrisonTrue", PhotonTargets.All);
    }

    void InputGet()
    {
        // Stamina
        float FillStaminaSpeed = 0.15f;

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 5f;
            Ani.SetBool("isRun", false);

            if (State == State_Run)
                State = State_SlowRun;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (State == State_SlowRun || State == State_Run)
            {
                if (Stamina > 0)
                {
                    State = State_Run;
                    Ani.SetBool("isRun", true);

                    MoveSpeed = 7f;
                    Stamina -= Time.deltaTime;
                    GameCtrl.instance.UseFootPrint(transform.position);

                    if (Stamina < 0)
                        Stamina = 0;

                    SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);
                }
                else
                {
                    State = State_SlowRun;
                    Ani.SetBool("isRun", false);
                    MoveSpeed = 5f;
                }
            }
            else
            {
                if (Stamina < maxStamina)
                {
                    Stamina += FillStaminaSpeed * Time.deltaTime;
                    SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);
                }
            }
        }
        else
        {
            if (Stamina < maxStamina)
            {
                Stamina += FillStaminaSpeed * Time.deltaTime;
                SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);
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
                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        GameCtrl.instance.SetSurvivorScore(100);
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
                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        GameCtrl.instance.SetSurvivorScore(100);
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
        GameCtrl.instance.Murderer.GetComponent<MurdererCtrl>().DamageByPlayer(Power);
    }

    public void AttackByMurderer(int MurdererAttack)
    {
        if (!Prison)
        {
            if (State == State_AttackW || State == State_AttackL)
            {
                if (State == MurdererAttack)
                {
                    if (State == State_AttackW)
                    {
                        State = State_ParryToMurdererW;
                        pv.RPC("AttackWAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    else
                    {
                        State = State_ParryToMurdererL;
                        pv.RPC("AttackLAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    pv.RPC("DamageToMurderer", PhotonTargets.All);
                }
                else
                {
                    DamageByMurderer();
                }
            }
            else if (State == State_Trap)
            {
                DamageByMurderer();
            }
            else
            {
                Attack = MurdererAttack;
                AttackTime = 0.8f;
            }
        }
    }

    void DamageByMurderer()
    {
        Trap = false;
        pv.RPC("AttackEnd", PhotonTargets.All);
        GameCtrl.instance.SetMurdererScore(100);

        Hp -= 5f;
        if (pv.isMine)
        {
            SurvivorUICtrl.instance.DispHP(Hp);
            StartCoroutine(GameCtrl.instance.StartHit(2));
        }

        if (Hp <= 0)
        {
            State = State_Idle;
            Ani.SetBool("isSlowRun", false);
            Ani.SetBool("isRun", false);

            Prison = true;
            Life -= 1;
            Hp = 50f;
            if (pv.isMine)
            {
                SurvivorUICtrl.instance.DispLife(Life);
                SurvivorUICtrl.instance.DispHP(Hp);
            }

            GameCtrl.instance.SetMurdererScore(500);

            if (Life == 0)
            {
                Prison = false;

                SurvivorDead();
            }
        }
        else
        {
            State = State_Hit;

            float dir = transform.InverseTransformDirection(trModel.forward).z * GameCtrl.instance.Murderer.transform.forward.z;

            if (dir >= 0)
            {
                pv.RPC("DownFrontAnim", PhotonTargets.All);
                StartCoroutine(Knockback(true));
            }
            else
            {
                pv.RPC("DownAnim", PhotonTargets.All);
                StartCoroutine(Knockback(false));
            }

            Ani.SetBool("isSlowRun", false);
            Ani.SetBool("isRun", false);
        }
    }

    IEnumerator Knockback(bool front)
    {
        float AttackRunTime = 0;

        if (front)
        {
            while (AttackRunTime < 1)
            {
                AttackRunTime += Time.deltaTime;
                float newTime = Mathf.Clamp(1 - AttackRunTime, 0, 1) * 0.7f;
                transform.Translate(transform.InverseTransformDirection(trModel.forward).normalized * newTime * newTime);

                yield return null;
            }
        }
        else
        {
            while (AttackRunTime < 1)
            {
                AttackRunTime += Time.deltaTime;
                float newTime = Mathf.Clamp(1 - AttackRunTime, 0, 1) * 0.7f;
                transform.Translate(-1 * transform.InverseTransformDirection(trModel.forward).normalized * newTime * newTime);

                yield return null;
            }
        }
    }

    public void TrapOn()
    {
        Hp -= 10f;
        if (pv.isMine)
            SurvivorUICtrl.instance.DispHP(Hp);

        Trap = true;

        if (pv.isMine)
            SaveRot = trModel.eulerAngles;

        State = State_Trap;
        pv.RPC("TrapAnim", PhotonTargets.All);
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);
    }

    public void PrisonStay(GameObject prison)
    {
        if (Item.ItemGet(5) == 1)
        {
            if (Input.GetKey(KeyCode.R))
            {
                if (pv.isMine)
                {
                    if (SurvivorUICtrl.instance.Message.activeSelf)
                        SurvivorUICtrl.instance.Message.SetActive(false);
                    SurvivorUICtrl.instance.DisTime(PrisonTime, 3);
                }

                PrisonTime -= Time.deltaTime;

                if (PrisonTime < 0)
                {
                    if (pv.isMine)
                    {
                        SurvivorUICtrl.instance.Time.SetActive(false);
                    }

                    Item.ItemSet(5, 0);

                    prison.GetComponent<PrisonCtrl>().OpenDoor();
                    PrisonTime = 3f;

                    GameCtrl.instance.SetSurvivorScore(500);
                }
            }
            else
            {
                if (pv.isMine)
                {
                    if (!SurvivorUICtrl.instance.Message.activeSelf)
                        SurvivorUICtrl.instance.DisMessage(3);

                    if (SurvivorUICtrl.instance.Time.activeSelf)
                        SurvivorUICtrl.instance.Time.SetActive(false);
                }

                PrisonTime = 3f;
            }
        }
    }

    public void PrisonExit()
    {
        PrisonTime = 3f;

        if (pv.isMine)
        {
            SurvivorUICtrl.instance.Message.SetActive(false);
            SurvivorUICtrl.instance.Time.SetActive(false);
        }
    }

    [PunRPC]
    void PrisonTrue()
    {
        Hp -= 1.5f * Time.deltaTime;
        if (pv.isMine)
            SurvivorUICtrl.instance.DispHP(Hp);

        if (Hp <= 0)
        {
            Prison = false;
            inPrison.GetComponent<PrisonCtrl>().SurvivorExit(this.gameObject);

            SurvivorDead();
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
                Prison = false;
                SurvivorDead();
            }
        }
    }

    public void PrisonFalse()
    {
        Prison = false;
        PrisonTP = false;

        Hp = 100;
        if (pv.isMine)
            SurvivorUICtrl.instance.DispHP(Hp);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Machine")
        {
            if (pv.isMine)
            {
                if (State == 0 || State == 1 || State == 2 || State == 5)
                {
                    MachineRangeCtrl machineRangeCtrl = other.gameObject.GetComponent<MachineRangeCtrl>();
                    MachineCtrl machineCtrl = machineRangeCtrl.Machine.GetComponent<MachineCtrl>();

                    if (machineCtrl.GetComplete())
                        return;

                    machineCtrl.DisHUD(transform.position);

                    if (machineRangeCtrl.GetMachineUse())
                    {
                        if (WorkMachine == machineCtrl.MachineNum)
                        {
                            if (Input.GetKey(KeyCode.R))
                            {
                                if (State != State_Repair)
                                {
                                    State = State_Repair;
                                    Ani.SetBool("isRepair", true);
                                    Ani.SetBool("isSlowRun", false);
                                    Ani.SetBool("isRun", false);

                                    SaveRot = other.transform.eulerAngles;
                                }

                                bool complete = machineCtrl.Install(Time.deltaTime * WorkSpeed);

                                if (complete)
                                {
                                    WorkMachine = 0;

                                    State = State_Idle;
                                    Ani.SetBool("isRepair", false);
                                    
                                    machineRangeCtrl.SetMachineUse(false);

                                    GameCtrl.instance.SetSurvivorScore(200);
                                }
                            }
                            else
                            {
                                WorkMachine = 0;

                                State = State_Idle;
                                Ani.SetBool("isRepair", false);

                                machineCtrl.MachineStop();
                                machineRangeCtrl.SetMachineUse(false);
                            }
                        }
                    }
                    else
                    {
                        if (machineCtrl.GetGadgetUse())
                        {
                            if (pv.isMine)
                            {
                                if (!SurvivorUICtrl.instance.Message.activeSelf)
                                    SurvivorUICtrl.instance.DisMessage(2);
                            }

                            if (Input.GetKey(KeyCode.R))
                            {
                                if (pv.isMine)
                                {
                                    SurvivorUICtrl.instance.Message.SetActive(false);
                                }
                                machineRangeCtrl.SetMachineUse(true);
                                WorkMachine = machineCtrl.MachineNum;
                            }
                        }
                        else
                        {
                            int GadgetNum = Item.ItemGet(4);

                            if (GadgetNum > 0)
                            {
                                if (pv.isMine)
                                {
                                    if (!SurvivorUICtrl.instance.Message.activeSelf)
                                        SurvivorUICtrl.instance.DisMessage(1);
                                }

                                if (Input.GetKeyDown(KeyCode.T))
                                {
                                    if (pv.isMine)
                                    {
                                        SurvivorUICtrl.instance.Message.SetActive(false);
                                    }
                                    machineCtrl.SetGadgetUse(true);
                                    Item.ItemSet(4, GadgetNum - 1);
                                }
                            }
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
            if (pv.isMine)
            {
                if (SurvivorUICtrl.instance.Message.activeSelf)
                    SurvivorUICtrl.instance.Message.SetActive(false);

                other.gameObject.GetComponent<MachineRangeCtrl>().Machine.GetComponent<MachineCtrl>().SetHUD(false);
            }
        }
    }

    public void SetState(int s)
    {
        if (s == State_PickItem)
        {
            if (State == State_PickItem)
                return;

            State = State_PickItem;
            pv.RPC("PickItemAnim", PhotonTargets.All);
            Ani.SetBool("isSlowRun", false);
            Ani.SetBool("isRun", false);
        }
        else if (s == State_Die)
        {
            State = State_Die;
            //gameObject.SetActive(false);
        }
        else
            State = State_Idle;
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

            if (pv.isMine)
            {
                SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);
            }
        }
    }

    public void SurvivorDead()
    {
        State = State_Die;
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);
        pv.RPC("DieAnim", PhotonTargets.All);

        GameCtrl.instance.SetMurdererScore(2000);

        SurvivorDie();
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
    public void AttackWAnim()
    {
        Ani.SetTrigger("trAttackW");
    }

    [PunRPC]
    public void AttackLAnim()
    {
        Ani.SetTrigger("trAttackL");
    }

    [PunRPC]
    public void DownFrontAnim()
    {
        Ani.SetTrigger("trDownFront");  // 같은 방향
    }

    [PunRPC]
    public void DownAnim()
    {
        Ani.SetTrigger("trDown");   // 서로 반대 방향
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
    public void PickItemAnim()
    {
        Ani.SetTrigger("trPickItem");
    }

    [PunRPC]
    public void DieAnim()
    {
        Ani.SetTrigger("trDie");
    }

    [PunRPC]
    public void TrapAnim()
    {
        Ani.SetTrigger("trTrap");
    }

    void SurvivorDie()
    {
        PlayerPrefs.SetInt("Result", 2);
        StartCoroutine(GameCtrl.instance.StartFade(false));
    }
}