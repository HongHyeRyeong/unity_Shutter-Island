using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimBoss
{
    public AnimationClip idle;
    public AnimationClip run;
    public AnimationClip attack;
    public AnimationClip tired;
    public AnimationClip down;
}

public class BossCtrl : MonoBehaviour
{
    public AnimBoss anim;
    public Animation _animation;

    float MoveSpeed = 4.5f;

    // 카메라
    private float x;
    float xSpeed = 100.0f;

    void Start()
    {
        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.idle;
        _animation.Play();
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        x += Input.GetAxis("Mouse X") * xSpeed * 0.015f;

        transform.Translate(new Vector3(h, 0, v) * MoveSpeed * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(0, x, 0);
        transform.rotation = rotation;

        // Animation
        if (v != 0 || h != 0)
        {
            _animation.CrossFade(anim.run.name, 0.2f);
        }
        else
        {
            _animation.CrossFade(anim.idle.name, 0.2f);
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.transform.tag == "Player")
        {
           col.gameObject.GetComponent<PlayerCtrl>().DamageByBoss();
        }
    }
}
