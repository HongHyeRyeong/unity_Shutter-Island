using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour {

    //
    public InputField IDInputField;
    public InputField PWInputField;

    public InputField NewIDInputField;
    public InputField NewPWInputField;

    public GameObject LoginPanelObj;
    public GameObject CreateAccountPanelObj;

    //
    public string LoginURL;
    public string CreateURL;

    //
    int CountPW = 0;
    static int MaxPW = 12;
    string[] MaskArray = new string[MaxPW];
    public Text StarPW;
    public Text StarPWCAP;

    //
    public Text NowLoginState;

    //
    public static string user_id;
    public static string SurRank;
    public static string MurRank;

    void Start()
    {
        LoginURL = "wjddus0424.dothome.co.kr/Login.php";
        CreateURL = "wjddus0424.dothome.co.kr/CreateAccount.php";

        MaskArray[0] = "";
        string mask = "";

        for (int cnt = 0; cnt <= MaxPW - 1; cnt++)
        {
            if (cnt != 0)
            {
                MaskArray[cnt] = mask + "*";
                mask = mask + "*";
            }
        }
    }

    public void PWToStar()
    {
        int CntPW = PWInputField.text.Length;

        StarPW.text = MaskArray[CntPW];
    }

    public void PWToStarCAP()
    {
        int CntPW = NewPWInputField.text.Length;

        StarPWCAP.text = MaskArray[CntPW];
    }

    public void LoginButton()
    {
        StartCoroutine(LoginGo());
    }

    IEnumerator LoginGo()
    {
        WWWForm form = new WWWForm();
        form.AddField("Input_id", IDInputField.text);
        form.AddField("Input_pw", PWInputField.text);

        WWW webRequest = new WWW(LoginURL, form);
        yield return webRequest;

        if(webRequest.text == "PE")
        {
            print("Password is Error");
            NowLoginState.text = "패스워드를 잘못 입력하셨습니다. 패스워드를 다시 입력해주세요.";
        }
        else if (webRequest.text == "NF")
        {
            print("User Not Found");
            NowLoginState.text = "사용자를 찾을 수 없습니다. ID를 다시 입력해주세요.";
        }
        else
        {
            string[] stringSeparators = new string[] { "\n" };
            string[] lines = webRequest.text.Split(stringSeparators, System.StringSplitOptions.RemoveEmptyEntries);

            for(int i = 0; i < lines.Length; ++i)
            {
                string[] _parts = lines[i].Split(',');
                SurRank = _parts[0];
                MurRank = _parts[1];
            }

            print(SurRank);
            print(MurRank);

            print("Login Success ");
            NowLoginState.text = "Login Success";

            user_id = IDInputField.text;

            SceneManager.LoadScene("TEST");
        }

        StopCoroutine(LoginGo());
    }

    public static string GetID()
    {
        return user_id;
    }

    public static string GetSurRank()
    {
        return SurRank;
    }

    public static string GetMurRank()
    {
        return MurRank;
    }

    public void OpenCreateAccountButton()
    {
        LoginPanelObj.SetActive(false);
        CreateAccountPanelObj.SetActive(true);
    }

    public void CreateAccountButton()
    {
        StartCoroutine(CreateGo());

        CreateAccountPanelObj.SetActive(false);
        LoginPanelObj.SetActive(true);
    }

    IEnumerator CreateGo()
    {
        print(NewIDInputField.text);
        print(NewPWInputField.text);

        WWWForm form = new WWWForm();
        form.AddField("Input_id", NewIDInputField.text);
        form.AddField("Input_pw", NewPWInputField.text);

        WWW webRequest = new WWW(CreateURL, form);
        yield return webRequest;

        print(webRequest.text);

        yield return null;
    }
}
