using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    const int Cha_Default = 0;
    const int Cha_MoveSpeedUp = 1;
    const int Cha_WorkSpeedUp = 2;
    const int Cha_DamageUp = 3;

    //
    const int DEFAULT = 0;
    const int WALK = 1;
    const int RUN = 2;
    const int INJURY = 3;

    //
    private int Character;
    private int State = DEFAULT;
    private int Life = 2;
    private float Hp = 100f;
    public float MoveSpeed = 5f;
    private int Power = 10;
    private int WorkSpeed = 10;
    public bool Prison = false;

    void Start()
    {
        Character = Cha_Default;

        if (Character == Cha_MoveSpeedUp)
        {
            MoveSpeed = 7f;
        }
        else if (Character == Cha_WorkSpeedUp)
        {
            WorkSpeed = 15;
        }
        else if (Character == Cha_DamageUp)
        {
            Power = 15;
        }
    }

    void Update()
    {
        if (Prison)
        {
            Hp -= 0.5f * Time.deltaTime;
        }

        if (Life == 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void DamageByBoss()
    {
        //print("HP: " + Hp);
        //print("Life: " + Life);

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
    }

    void PrisonFalse()
    {
        Prison = false;
        Hp = 50;
    }
}
