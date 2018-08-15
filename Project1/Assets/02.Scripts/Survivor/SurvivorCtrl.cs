using UnityEngine;
using System.Collections;

public class SurvivorCtrl : MonoBehaviour
{
    public PhotonView pv = null;   // PhotonView 컴포넌트를 할당할 변수

    // 위치 정보를 송수신할 때 사용할 변수 선언 및 초기값 설정
    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;

    //
    [SerializeField]
    private Animator Ani;
    [SerializeField]
    private Transform trModel;
    [SerializeField]
    private AudioSource Audio;
    [SerializeField]
    private SurvivorItem Item;
    [SerializeField]
    private Transform SurvivorComPivot;
    [SerializeField]
    private SkinnedMeshRenderer MeshRen;

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

    [HideInInspector]
    public bool Trap = false;

    private Vector3 SaveRot;
    private Vector3 SavePos;

    private bool teleport;

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
        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        // PhotonView Observed Components 속성에 Ctrl 스크립트 연결
        pv.ObservedComponents[0] = this;

        // 원격 플레이어의 위치 및 회전 값을 처리할 변수의 초기값 설정
        currPos = transform.position;
        currRot = transform.rotation;

        Audio.Stop();
        SoundManager.instance.SetEffect(false, Audio, "SFootStep");

        if (pv.isMine)
        {
            CameraCtrl.instance.transform.position = transform.position;
            CameraCtrl.instance.targetSurvivorComPivot = SurvivorComPivot;

            if (LobbyCtrl.instance.SurStat == 1)
                Stamina = 6f;
            else if (LobbyCtrl.instance.SurStat == 2)
                WorkSpeed = 1.1f;
            else if (LobbyCtrl.instance.SurStat == 3)
                Power = 15f;

            pv.RPC("ChangeMaterial", PhotonTargets.AllBuffered, LobbyCtrl.instance.SurStat - 1);

            maxStamina = Stamina;
            saveStamina = Stamina;
            saveWorkSpeed = WorkSpeed;

            SurvivorUICtrl.instance.DispHP(Hp);
            SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);

            SoundManager.instance.SetBGM("Ingame");
        }
    }

    [PunRPC]
    public void ChangeMaterial(int num)
    {
        MeshRen.material = GameCtrl.instance.Msurvivor[num];
    }

    void Update()
    {
        if (pv.isMine)
        {
            if (!GameCtrl.instance.isStart)  // 게임 시작 전
                return;

            float h = 0;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                h = Input.GetAxis("Horizontal");

            float v = 0;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
                v = Input.GetAxis("Vertical");

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

                if (!SurvivorUICtrl.instance.Inven.activeSelf && !SettingManager.instance.Setting.activeSelf)  // 인벤, 설정 창이 켜져있을때
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        State = State_AttackW;
                        pv.RPC("AttackWAnim", PhotonTargets.AllBuffered);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        State = State_AttackL;
                        pv.RPC("AttackLAnim", PhotonTargets.AllBuffered);
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

                if (!Audio.isPlaying)
                    pv.RPC("EffectPlayLoop", PhotonTargets.AllBuffered);
            }
            else
            {
                if (Audio.isPlaying && Audio.clip.name == "SFootStep")
                    pv.RPC("EffectStop", PhotonTargets.All);
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
            if (!teleport)
            {
                transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f);
            }
            else
            {
                transform.position = SavePos;
                pv.RPC("teleportFalse", PhotonTargets.AllBuffered);
            }
        }

        if (Prison)
            PrisonTrue();
    }

    void InputGet()
    {
        // Stamina
        float FillStaminaSpeed = 0.15f;

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (State == State_Run)
                State = State_SlowRun;
            Ani.SetBool("isRun", false);

            MoveSpeed = 5f;

            pv.RPC("EffectPitch", PhotonTargets.AllBuffered, 2f);
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
                    pv.RPC("RunFoot", PhotonTargets.AllBuffered);

                    if (Stamina < 0)
                        Stamina = 0;

                    SurvivorUICtrl.instance.DispStamina(Stamina, maxStamina);
                    pv.RPC("EffectPitch", PhotonTargets.AllBuffered, 3f);
                }
                else
                {
                    State = State_SlowRun;
                    Ani.SetBool("isRun", false);

                    MoveSpeed = 5f;

                    pv.RPC("EffectPitch", PhotonTargets.AllBuffered, 2f);
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
            if (pv.isMine)
                SurvivorUICtrl.instance.DisTime(AttackTime, 0.8f);

            if (AttackTime > 0)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Attack == State_AttackL)
                        DamageByMurderer();
                    else
                    {
                        State = State_ParryToMurdererW;
                        pv.RPC("ParryWAnim", PhotonTargets.All);
                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        if (pv.isMine)
                        {
                            StartCoroutine(GameCtrl.instance.StartAttack(0.2f));
                            SurvivorUICtrl.instance.Time.SetActive(false);
                        }

                        GameCtrl.instance.SurvivorScore[0] += 100;
                        GameCtrl.instance.SetSurvivorScore(100);
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (Attack == State_AttackW)
                        DamageByMurderer();
                    else
                    {
                        State = State_ParryToMurdererL;
                        pv.RPC("ParryLAnim", PhotonTargets.All);
                        pv.RPC("AttackEnd", PhotonTargets.All);
                        pv.RPC("DamageToMurderer", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);

                        if (pv.isMine)
                        {
                            StartCoroutine(GameCtrl.instance.StartAttack(0.2f));
                            SurvivorUICtrl.instance.Time.SetActive(false);
                        }

                        GameCtrl.instance.SurvivorScore[0] += 100;
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
    void RunFoot()
    {
        GameCtrl.instance.UseFootPrint(transform.position);
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
                        pv.RPC("ParryWAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    else
                    {
                        State = State_ParryToMurdererL;
                        pv.RPC("ParryLAnim", PhotonTargets.All);
                        Ani.SetBool("isSlowRun", false);
                        Ani.SetBool("isRun", false);
                    }
                    pv.RPC("DamageToMurderer", PhotonTargets.All);

                    if (pv.isMine)
                        SurvivorUICtrl.instance.Time.SetActive(false);
                }
                else
                    DamageByMurderer();
            }
            else if (State == State_Trap)
                DamageByMurderer();
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

        pv.RPC("SetMurdererScore", PhotonTargets.AllBuffered, 0, 100);

        Hp -= 50f;
        if (pv.isMine)
        {
            SurvivorUICtrl.instance.DispHP(Hp);
            SurvivorUICtrl.instance.Time.SetActive(false);
            GameCtrl.instance.HitEffect();
        }

        if (Hp <= 0)
        {
            State = State_Idle;
            Ani.SetBool("isSlowRun", false);
            Ani.SetBool("isRun", false);
            Ani.SetBool("isRepair", false);

            Prison = true;
            Life -= 1;
            Hp = 50f;
            if (pv.isMine)
            {
                SurvivorUICtrl.instance.DispLife(Life);
                SurvivorUICtrl.instance.DispHP(Hp);
            }

            pv.RPC("SetMurdererScore", PhotonTargets.AllBuffered, 1, 500);

            if (Life == -1)
            {
                Prison = false;
                SurvivorDead();
            }
        }
        else
        {
            State = State_Hit;

            float dir = trModel.forward.z * GameCtrl.instance.Murderer.transform.forward.z;

            if (dir >= 0)
            {
                if (pv.isMine)
                    pv.RPC("DownFrontAnim", PhotonTargets.AllBuffered);
                StartCoroutine(Knockback(true));
            }
            else
            {
                if (pv.isMine)
                    pv.RPC("DownAnim", PhotonTargets.AllBuffered);
                StartCoroutine(Knockback(false));
            }

            Ani.SetBool("isSlowRun", false);
            Ani.SetBool("isRun", false);
        }
    }

    IEnumerator Knockback(bool front)
    {
        transform.rotation = Quaternion.identity;
        Quaternion rot = GameCtrl.instance.Murderer.transform.rotation;
        if (!front)
            rot = Quaternion.Euler(rot.eulerAngles.x, rot.eulerAngles.y + 180, rot.eulerAngles.z);

        Vector3 forword = GameCtrl.instance.Murderer.transform.forward;

        float AttackRunTime = 0;
        
        while (AttackRunTime < 1)
        {
            trModel.rotation = Quaternion.Slerp(trModel.rotation, rot, Time.deltaTime * 50f);

            AttackRunTime += Time.deltaTime;
            float newTime = Mathf.Clamp(1 - AttackRunTime, 0, 1) * 0.7f;
            transform.Translate(forword * newTime * newTime);

            yield return null;
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
        Ani.SetTrigger("trTrap");
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);
    }

    public void PrisonStay(GameObject prison)
    {
        if (Item.ItemGet(5) == 1)
        {
            if (Input.GetKey(KeyCode.R))
            {
                PrisonTime -= Time.deltaTime;

                if (pv.isMine)
                {
                    if (SurvivorUICtrl.instance.Message.activeSelf)
                        SurvivorUICtrl.instance.Message.SetActive(false);
                    SurvivorUICtrl.instance.DisTime(PrisonTime, 3);
                }

                if (PrisonTime < 0) // 감옥 문 오픈
                {
                    Item.ItemSet(5, 0); // 열쇠 사용

                    prison.GetComponent<PrisonCtrl>().OpenDoor();

                    PrisonTime = 3f;
                    if (pv.isMine)
                        SurvivorUICtrl.instance.Time.SetActive(false);

                    GameCtrl.instance.SurvivorScore[2] += 500;
                    GameCtrl.instance.SetSurvivorScore(500);
                }
            }
            else
            {
                PrisonTime = 3f;

                if (pv.isMine)
                {
                    if (!SurvivorUICtrl.instance.Message.activeSelf)
                        SurvivorUICtrl.instance.DisMessage(3);
                    if (SurvivorUICtrl.instance.Time.activeSelf)
                        SurvivorUICtrl.instance.Time.SetActive(false);
                }
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

    void PrisonTrue()
    {
        float temp = 1;
        if (Life == 0)  // 두번째 감옥일때 더 빠르게
            temp = 2f;

        Hp -= Time.deltaTime * temp;
        if (pv.isMine)
            SurvivorUICtrl.instance.DispHP(Hp);

        if (Hp <= 0)
        {
            Prison = false;
            inPrison.GetComponent<PrisonCtrl>().SurvivorExit(transform.gameObject);

            SurvivorDead();
        }

        if (PrisonTP == false)
        {
            PrisonTP = true;

            GameObject minObject = null;
            float minDist = 10000f;

            for (int i = 0; i < GameCtrl.instance.Prisons.Length; ++i)
            {
                if (GameCtrl.instance.Prisons[i].GetComponent<PrisonCtrl>().GetOpen() == false)
                {
                    float dist = Vector3.Distance(transform.position, GameCtrl.instance.Prisons[i].transform.position);

                    if (dist < minDist)
                    {
                        minDist = dist;
                        minObject = GameCtrl.instance.Prisons[i];
                    }
                }
            }

            inPrison = minObject;
            minObject.GetComponent<PrisonCtrl>().SurvivorEnter(transform.gameObject);
            transform.position = minObject.transform.position;

            pv.RPC("teleportTrue", PhotonTargets.AllBuffered, minObject.transform.position);
        }
    }

    [PunRPC]
    public void teleportFalse()
    {
        teleport = false;
        print("false");
    }

    [PunRPC]
    public void teleportTrue(Vector3 pos)
    {
        SavePos = pos;

        teleport = true;
    }

    public void SetprisonPos(Vector3 pos)
    {
        pv.RPC("teleportTrue", PhotonTargets.AllBuffered, pos);
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
        if (other.CompareTag("Machine"))
        {
            if (pv.isMine)
            {
                if (State == 0 || State == 1 || State == 2 || State == 5)
                {
                    MachineRangeCtrl machineRangeCtrl = other.gameObject.GetComponent<MachineRangeCtrl>();
                    MachineCtrl machineCtrl = machineRangeCtrl.Machine;

                    if (machineCtrl.GetComplete())
                    {
                        State = State_Idle;
                        Ani.SetBool("isRepair", false);

                        WorkMachine = 0;

                        if (SurvivorUICtrl.instance.Message.activeSelf)
                            SurvivorUICtrl.instance.Message.SetActive(false);

                        return;
                    }

                    machineCtrl.DisHUD(transform.position);

                    if (machineRangeCtrl.GetMachineUse())
                    {
                        if (WorkMachine != machineRangeCtrl.MachineNum)  // 현재 돌리고 있는 머신인지
                            return;

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

                            if (complete)   // 머신을 다 돌렸다면
                            {
                                WorkMachine = 0;

                                State = State_Idle;
                                Ani.SetBool("isRepair", false);

                                machineRangeCtrl.SetMachineUse(false);

                                GameCtrl.instance.SurvivorScore[1] += 200;
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
                    else
                    {
                        if (machineCtrl.GetGadgetUse()) // 부품이 설치돼있다면
                        {
                            if (pv.isMine)
                                SurvivorUICtrl.instance.DisMessage(2);

                            if (Input.GetKey(KeyCode.R))
                            {
                                WorkMachine = machineRangeCtrl.MachineNum;
                                machineRangeCtrl.SetMachineUse(true);

                                if (pv.isMine)
                                    SurvivorUICtrl.instance.Message.SetActive(false);
                            }
                        }
                        else
                        {
                            int GadgetNum = Item.ItemGet(4);

                            if (GadgetNum > 0)
                            {
                                SurvivorUICtrl.instance.DisMessage(1);

                                if (Input.GetKeyDown(KeyCode.T))
                                {
                                    machineCtrl.SetGadgetUse(true);
                                    Item.ItemSet(4, GadgetNum - 1);

                                    if (pv.isMine)
                                        SurvivorUICtrl.instance.Message.SetActive(false);
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
        if (other.CompareTag("Machine"))
        {
            if (pv.isMine)
            {
                if (SurvivorUICtrl.instance.Message.activeSelf)
                    SurvivorUICtrl.instance.Message.SetActive(false);

                other.gameObject.GetComponent<MachineRangeCtrl>().Machine.SetHUD(false);
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
            if (pv.isMine)
            {
                PlayerPrefs.SetInt("Result", 2);
                StartCoroutine(GameCtrl.instance.StartFade(false));
            }
        }
        else
            State = s;
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

    void SurvivorDead()
    {
        State = State_Die;
        pv.RPC("DieAnim", PhotonTargets.All);
        Ani.SetBool("isSlowRun", false);
        Ani.SetBool("isRun", false);

        pv.RPC("SetMurdererScore", PhotonTargets.AllBuffered, 3, 1500);
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
    void EffectPlayLoop()
    {
        Audio.loop = true;
        SoundManager.instance.SetEffect(true, Audio, "SFootStep");
        Audio.Play();
    }
    
    [PunRPC]
    void EffectPitch(float pitch)
    {
        Audio.pitch = pitch;
    }

    [PunRPC]
    void EffectStop()
    {
        Audio.Stop();
    }

    [PunRPC]
    void AttackWAnim()
    {
        Ani.SetTrigger("trAttackW");
    }

    [PunRPC]
    void AttackLAnim()
    {
        Ani.SetTrigger("trAttackL");
    }

    [PunRPC]
    void DownFrontAnim()
    {
        Ani.SetTrigger("trDownFront");  // 같은 방향
    }

    [PunRPC]
    void DownAnim()
    {
        Ani.SetTrigger("trDown");   // 서로 반대 방향
    }

    [PunRPC]
    void ParryWAnim()
    {
        Ani.SetTrigger("trParryW");
    }

    [PunRPC]
    void ParryLAnim()
    {
        Ani.SetTrigger("trParryL");
    }

    [PunRPC]
    void PickItemAnim()
    {
        Ani.SetTrigger("trPickItem");
    }

    [PunRPC]
    void DieAnim()
    {
        Ani.SetTrigger("trDie");
    }

    [PunRPC]
    void SetMurdererScore(int idx, int score)
    {
        GameCtrl.instance.MurdererScore[idx] += score;
        GameCtrl.instance.SetMurdererScore(score);
    }
}