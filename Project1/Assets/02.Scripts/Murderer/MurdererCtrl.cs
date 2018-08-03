using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurdererCtrl : MonoBehaviour
{
    private PhotonView pv = null;

    private Vector3 currPos = Vector3.zero;
    private Quaternion currRot = Quaternion.identity;
    GameObject curSurvivor;

    //
    private Animator Ani;
    private AudioSource Audio;
    public GameObject ParticleTrail;
    
    private GameObject[] TrapItems;
    private int TrapSetNum = 0;

    private int State = 0;
    private float Hp = 200f;
    private float MoveSpeed = 6f;
    private bool isAttack = false;

    private float MouseX;

    private float AttackRunTime = 0;

    private bool isMurDie = false;

    //
    const int State_Die = -1;
    const int State_Idle = 0;
    const int State_Run = 1;
    const int State_Parry = 2;
    const int State_Trap = 3;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        pv.synchronization = ViewSynchronization.UnreliableOnChange;
        pv.ObservedComponents[1] = this;

        currPos = transform.position;
        currRot = transform.rotation;

        Ani = GetComponent<Animator>();

        Audio = GetComponent<AudioSource>();
        Audio.Stop();
        SoundManager.instance.SetEffect(false, Audio, "MFootStep");

        TrapItems = new GameObject[GameCtrl.instance.MurdererTrap.transform.childCount];
        for (int i = 0; i < TrapItems.Length; ++i)
        {
            TrapItems[i] = GameCtrl.instance.MurdererTrap.transform.GetChild(i).gameObject;
            TrapItems[i].SetActive(false);
        }

        if (pv.isMine)
        {
            CameraCtrl.instance.transform.position = transform.position;
            CameraCtrl.instance.targetMurderer = this.gameObject.transform;
            CameraCtrl.instance.targetMurdererCamPivot = this.gameObject.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 Head/MurdererCamPivot").transform;

            MouseX = 180;

            SoundManager.instance.SetBGM("Ingame6-In My Nightmares");
        }
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
            if (State == State_Idle || State == State_Run)
            {
                if (v != 0 || h != 0)
                {
                    State = State_Run;

                    if (v < 0)
                    {
                        MoveSpeed = 4f;
                        Ani.SetBool("isBackRun", true);
                        Ani.SetBool("isRun", false);
                    }
                    else
                    {
                        MoveSpeed = 6f;
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

                if (!SettingManager.instance.Setting.activeSelf)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (State == State_Run && Ani.GetBool("isRun"))
                            pv.RPC("AttackWRunAnim", PhotonTargets.All);
                        else
                            pv.RPC("AttackWAnim", PhotonTargets.All);

                        Ani.SetBool("isRun", false);
                        Ani.SetBool("isBackRun", false);

                        pv.RPC("AttackWTrue", PhotonTargets.All);
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        if (State == State_Run && Ani.GetBool("isRun"))
                            pv.RPC("AttackLRunAnim", PhotonTargets.All);
                        else
                            pv.RPC("AttackLAnim", PhotonTargets.All);

                        Ani.SetBool("isRun", false);
                        Ani.SetBool("isBackRun", false);

                        pv.RPC("AttackLTrue", PhotonTargets.All);
                    }
                    else if (Input.GetKeyDown(KeyCode.E))
                    {
                        State = State_Trap;
                        pv.RPC("InstallAnim", PhotonTargets.All);
                    }
                }
            }
            else if (State == State_AttackW || State == State_AttackL)
            {
                if (Ani.GetCurrentAnimatorStateInfo(0).IsName("AttackWRun") || Ani.GetCurrentAnimatorStateInfo(0).IsName("AttackLRun"))
                {
                    AttackRunTime += Time.deltaTime;
                    float newTime = Mathf.Clamp(1 - AttackRunTime, 0, 1) * 0.4f;
                    transform.Translate(new Vector3(0, 0, 1) * newTime * newTime);
                }
            }

            // Movement
            if (State == State_Run)
            {
                Vector3 Pos = new Vector3(h, 0, v);
                transform.Translate(Pos.normalized * MoveSpeed * Time.deltaTime);

                if (!Audio.isPlaying)
                    pv.RPC("EffectPlayLoop", PhotonTargets.All);
            }
            else
            {
                if (Audio.isPlaying && Audio.clip.name == "MFootStep")
                    pv.RPC("EffectStop", PhotonTargets.All);
            }

            if (State != State_Parry && State != State_Trap)
            {
                MouseX += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
                transform.rotation = Quaternion.Euler(0, MouseX, 0);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f);
        }

        if (Hp <= 0 && !isMurDie)
        {
            State = State_Die;
            pv.RPC("DieAnim", PhotonTargets.All);

            if(pv.isMine)
            {
                PlayerPrefs.SetInt("Result", 4);
            }
            else
            {
                GameCtrl.instance.SurvivorScore[3] += 2000;
                GameCtrl.instance.SetSurvivorScore(2000);
                PlayerPrefs.SetInt("Result", 1);
            }

            pv.RPC("MurdererDie", PhotonTargets.AllBuffered);

            isMurDie = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            other.transform.parent.GetComponent<SurvivorCtrl>().AttackByMurderer(State);
        }
    }

    public void PlaceTrap()
    {
        int index = -1;
        bool use = false;

        for (int i = 0; i < TrapItems.Length; ++i)
            if (!TrapItems[i].activeSelf)
            {
                use = true;
                index = i;
                TrapItems[i].SetActive(true);

                GameCtrl.instance.DisTrap(-1);
                break;
            }

        if (!use)
        {
            int Num = 0;
            int MinNum = TrapItems[0].GetComponent<MurdererTrapCtrl>().SetNum;

            for (int i = 1; i < TrapItems.Length; ++i)
                if (MinNum > TrapItems[i].GetComponent<MurdererTrapCtrl>().SetNum)
                {
                    Num = i;
                    MinNum = TrapItems[i].GetComponent<MurdererTrapCtrl>().SetNum;
                }
            index = Num;
        }

        TrapItems[index].transform.position = transform.position + transform.forward;
        TrapItems[index].GetComponent<MurdererTrapCtrl>().SetNum = TrapSetNum;
        TrapSetNum++;
    }

    public void DamageByPlayer(float Power)
    {
        Hp -= Power;

        if (pv.isMine)
        {
            State = State_Parry;

            pv.RPC("ParryAnim", PhotonTargets.All);
            Ani.SetBool("isRun", false);
            Ani.SetBool("isBackRun", false);

            MurdererUICtrl.instance.DispHP(Hp);
            StartCoroutine(GameCtrl.instance.StartHit(2));
        }
        GameCtrl.instance.DisMurHP(Hp);
    }

    public void DamageByMachine(float Power)
    {
        Hp -= Power;

        if (pv.isMine)
        {
            StartCoroutine(CameraCtrl.instance.Attack(4));
            MurdererUICtrl.instance.DispHP(Hp);
        }
        GameCtrl.instance.DisMurHP(Hp);
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

        AttackRunTime = 0;
        ParticleTrail.SetActive(false);

        if (State == State_Die)
            gameObject.SetActive(false);
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
    public void EffectPlayLoop()
    {
        Audio.loop = true;
        SoundManager.instance.SetEffect(true, Audio, "MFootStep");
        Audio.Play();
    }

    [PunRPC]
    public void EffectStop()
    {
        Audio.Stop();
    }

    [PunRPC]
    public void AttackWAnim()
    {
        Ani.SetTrigger("trAttackW");

        Audio.loop = false;
        SoundManager.instance.SetEffect(true, Audio, "Slash");
        Audio.Play();
    }

    [PunRPC]
    public void AttackLAnim()
    {
        Ani.SetTrigger("trAttackL");

        Audio.loop = false;
        SoundManager.instance.SetEffect(true, Audio, "Slash");
        Audio.Play();
    }

    [PunRPC]
    public void AttackWRunAnim()
    {
        Ani.SetTrigger("trAttackWRun");

        Audio.loop = false;
        SoundManager.instance.SetEffect(true, Audio, "Slash");
        Audio.Play();
    }

    [PunRPC]
    public void AttackLRunAnim()
    {
        Ani.SetTrigger("trAttackLRun");

        Audio.loop = false;
        SoundManager.instance.SetEffect(true, Audio, "Slash");
        Audio.Play();
    }

    [PunRPC]
    public void ParryAnim()
    {
        Ani.SetTrigger("trParry");

        Audio.loop = false;
        SoundManager.instance.SetEffect(true, Audio, "Counterhit");
        Audio.Play();
    }

    [PunRPC]
    public void InstallAnim()
    {
        Ani.SetTrigger("trInstall");
    }

    [PunRPC]
    public void DieAnim()
    {
        Ani.SetTrigger("trDie");
    }

    [PunRPC]
    public void AttackWTrue()
    {
        State = State_AttackW;
        isAttack = true;

        ParticleTrail.SetActive(true);
    }

    [PunRPC]
    public void AttackLTrue()
    {
        State = State_AttackL;
        isAttack = true;

        ParticleTrail.SetActive(true);
    }

    [PunRPC]
    public void MurdererDie()
    {
        StartCoroutine(GameCtrl.instance.StartFade(false));
    }

    public void MurdererWin()
    {
        PlayerPrefs.SetInt("Result", 3);
        StartCoroutine(GameCtrl.instance.StartFade(false));
    }
}
