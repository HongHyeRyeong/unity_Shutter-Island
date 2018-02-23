using UnityEngine;
using System.Collections;

[System.Serializable]
public class AnimPlayer
{
    public AnimationClip idle;
    public AnimationClip runFront;
    public AnimationClip runBack;
}

public class PlayerCtrl : MonoBehaviour
{
    public AnimPlayer anim;
    public Animation _animation;
    
    private int Character;
    private int State;

    private int Life = 2;
    private float Hp = 100f;
    private int Power = 10;
    private float Stamina = 4f;
    private float maxStamina;
    public float MoveSpeed = 4f;
    private int WorkSpeed = 10;

    public bool Prison = false;
    private bool PrisonTP = false;

    //
    const int Cha_Default = 0;
    const int Cha_Stamina = 1;
    const int Cha_WorkSpeed = 2;
    const int Cha_Damage = 3;

    void Start()
    {
        Character = Cha_Default;

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

        _animation = GetComponentInChildren<Animation>();

        _animation.clip = anim.idle;
        _animation.Play();
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float rotationSpeed = 100f;
        transform.Rotate(0, h * rotationSpeed * Time.deltaTime, 0);

        float v = Input.GetAxis("Vertical");
        transform.Translate(0, 0, v * MoveSpeed * Time.deltaTime);

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

        if (Prison)
            PrisonTrue();

        Key();

        // End
        if (Life == 0 || Hp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void Key()
    {
        // Stamina
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Stamina -= Time.deltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MoveSpeed = 6f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            MoveSpeed = 4f;
        }
        else
        {
            if (Stamina < maxStamina)
                Stamina += Time.deltaTime;
        }

        //print(Stamina);
    }

    public void DamageByBoss()
    {
        if (Life != 0 && Prison == false)  // 1이나 2
        {
            if (Hp == 100f)
            {
                Hp = 50f;
            }
            else if (Hp == 50f)
            {
                Life -= 1;
                Prison = true;
            }
        }

        //print("HP: " + Hp);
        //print("Life: " + Life);
    }

    void PrisonTrue()
    {
        Hp -= 0.5f * Time.deltaTime;
        print("HP: " + Hp);

        if (PrisonTP == false)
        {
            PrisonTP = true;
            _animation.CrossFade(anim.idle.name, 0.3f);

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