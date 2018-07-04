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
    MurdererUICtrl MurdererUI;

    private Animator Ani;
    public GameObject ParticleTrail;

    private int State = 0;
    private float Hp = 200f;
    private float MoveSpeed = 6f;
    private bool isAttack = false;

    private float MouseX;

    float AttackRunTime = 0;

    //
    const int State_Die = -1;
    const int State_Idle = 0;
    const int State_Run = 1;
    const int State_Parry = 2;
    const int State_AttackW = 10;
    const int State_AttackL = 11;

    void Start()
    {
        pv = GetComponent<PhotonView>();
        Ani = GetComponent<Animator>();

        // 데이터 전송 타입 설정
        pv.synchronization = ViewSynchronization.UnreliableOnChange;

        // PhotonView Observed Components 속성에 Ctrl 스크립트 연결
        pv.ObservedComponents[1] = this;

        currPos = transform.position;
        currRot = transform.rotation;

        if (pv.isMine)
        {
            MurdererUI = GameCtrl.instance.MurGameController.GetComponent<MurdererUICtrl>();

            GameCtrl.instance.Camera.GetComponent<CameraCtrl>().targetMurderer = this.gameObject.transform;
            GameCtrl.instance.Camera.GetComponent<CameraCtrl>().targetMurdererCamPivot =
                this.gameObject.transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/MurdererCamPivot").transform;
        }
    }

    void Update()
    {
        if (pv.isMine)
        {
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
                        MoveSpeed = 3f;
                        pv.RPC("BackRunAnim", PhotonTargets.All);
                        //Ani.SetBool("isBackRun", true);
                        pv.RPC("RunFAnim", PhotonTargets.All);
                        //Ani.SetBool("isRun", false);
                    }
                    else
                    {
                        MoveSpeed = 4.5f;
                        pv.RPC("RunAnim", PhotonTargets.All);
                        //Ani.SetBool("isRun", true);
                        pv.RPC("BackRunFAnim", PhotonTargets.All);
                    }
                }
                else
                {
                    State = State_Idle;
                    pv.RPC("RunFAnim", PhotonTargets.All);
                    pv.RPC("BackRunFAnim", PhotonTargets.All);
                }

                if (Input.GetMouseButtonDown(0))
                {
                    if (State == State_Run && Ani.GetBool("isRun"))
                        pv.RPC("AttackWRunAnim", PhotonTargets.All);
                    else
                        pv.RPC("AttackWAnim", PhotonTargets.All);

                    pv.RPC("RunFAnim", PhotonTargets.All);
                    pv.RPC("BackRunFAnim", PhotonTargets.All);

                    pv.RPC("AttackWTrue", PhotonTargets.All);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (State == State_Run && Ani.GetBool("isRun"))
                        pv.RPC("AttackLRunAnim", PhotonTargets.All);
                    else
                        pv.RPC("AttackLAnim", PhotonTargets.All);

                    pv.RPC("RunFAnim", PhotonTargets.All);
                    pv.RPC("BackRunFAnim", PhotonTargets.All);

                    pv.RPC("AttackLTrue", PhotonTargets.All);
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
                transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);

            if (State != State_Parry)
            {
                MouseX += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
                transform.rotation = Quaternion.Euler(0, MouseX, 0);
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 3.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 3.0f);
        }

        if (Hp < 0)
        {
            pv.RPC("DieAnim", PhotonTargets.All);

            GameCtrl.instance.GameController.GetComponent<GameCtrl>().SetSurvivorScore(2000);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Survivor" && isAttack)
        {
            isAttack = false;
            other.GetComponent<SurvivorCtrl>().AttackByMurderer(this.gameObject, State);
        }
    }

    public void DamageByPlayer(float Power)
    {
        Hp -= Power;

        if (pv.isMine)
        {
            State = State_Parry;

            pv.RPC("ParryAnim", PhotonTargets.All);
            pv.RPC("RunFAnim", PhotonTargets.All);
            pv.RPC("BackRunFAnim", PhotonTargets.All);

            MurdererUI.DispHP(Hp);
        }

        GameCtrl.instance.GameController.GetComponent<GameCtrl>().DisMurHP(Hp);
        print("murdererctrl " + Hp);
    }

    public void DamageByMachine(float Power)
    {
        Hp -= Power;

        MurdererUI.DispHP(Hp);
        GameCtrl.instance.GameController.GetComponent<GameCtrl>().DisMurHP(Hp);
    }

    public int GetState()
    {
        return State;
    }

    public void SetState(int s)
    {
        State = s;
        AttackRunTime = 0;

        if (State == State_Die)
            gameObject.SetActive(false);

        pv.RPC("RunFAnim", PhotonTargets.All);
        pv.RPC("BackRunFAnim", PhotonTargets.All);

        ParticleTrail.SetActive(false);
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
    public void RunAnim()
    {
        Ani.SetBool("isRun", true);
    }

    [PunRPC]
    public void BackRunAnim()
    {
        Ani.SetBool("isBackRun", true);
    }

    [PunRPC]
    public void RunFAnim()
    {
        Ani.SetBool("isRun", false);
    }

    [PunRPC]
    public void BackRunFAnim()
    {
        Ani.SetBool("isBackRun", false);
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
    public void AttackWRunAnim()
    {
        Ani.SetTrigger("trAttackWRun");
    }

    [PunRPC]
    public void AttackLRunAnim()
    {
        Ani.SetTrigger("trAttackLRun");
    }

    [PunRPC]
    public void ParryAnim()
    {
        Ani.SetTrigger("trParry");
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
}
