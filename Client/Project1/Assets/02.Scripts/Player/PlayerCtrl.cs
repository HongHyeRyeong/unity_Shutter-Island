using UnityEngine;
using System.Collections;

[System.Serializable]
public class Anim
{
    public AnimationClip idle;
    public AnimationClip runFront;
    public AnimationClip runBack;
}

public class PlayerCtrl : MonoBehaviour
{
    private float PlayerMoveSpeed = 0;
    float rotationSpeed = 60f;

    //
    public Anim anim;
    public Animation _animation;

    void Start()
    {
        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.idle;
        _animation.Play();
    }

    void Update()
    {
        // Init
        if (PlayerMoveSpeed == 0)
        {
            PlayerMoveSpeed = GameObject.Find("Player").GetComponent<PlayerState>().MoveSpeed;
        }

        // Movement
        float h = Input.GetAxis("Horizontal");
        transform.Rotate(0, h * rotationSpeed * Time.deltaTime, 0);

        float v = Input.GetAxis("Vertical");
        transform.Translate(0, 0, v * PlayerMoveSpeed * Time.deltaTime); ;

        // Animation
        if (v >= 0.1f)
        {
            _animation.CrossFade(anim.runFront.name, 0.3f);
        }
        else if (v <= -0.1f)
        {
            _animation.CrossFade(anim.runBack.name, 0.3f);
        }
        else if (h != 0)
        {
            _animation.CrossFade(anim.runFront.name, 0.3f);
        }
        else
        {
            _animation.CrossFade(anim.idle.name, 0.3f);
        }
    }

    //private float h = 0.0f;
    //private float v = 0.0f;

    //private Transform tr;

    //float Movespeed = 5.0f;
    //float Rotspeed = 150.0f;

    //void Start()
    //{
    //    tr = GetComponent<Transform>();
    //}

    //void Update()
    //{
    //    h = Input.GetAxis("Horizontal");
    //    v = Input.GetAxis("Vertical");

    //    Vector3 move = (Vector3.forward * v) + (Vector3.right * h);

    //    tr.Translate(move * Time.deltaTime * Movespeed, Space.Self);
    //    tr.Rotate(Vector3.up * Time.deltaTime * Rotspeed * Input.GetAxis("Mouse X"));
    //}
}