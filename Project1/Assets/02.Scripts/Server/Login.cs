using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField IDInputField;
    public InputField PWInputField;

    public InputField NewIDInputField;
    public InputField NewPWInputField;

    public GameObject LoginPanelObj;
    public GameObject CreateAccountPanelObj;

    public GameObject Fade;

    //
    public string LoginURL;
    public string CreateURL;

    //
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
        Screen.SetResolution(1920, 1080, false);

        LoginURL = "wjddus0424.dothome.co.kr/Login.php";
        CreateURL = "wjddus0424.dothome.co.kr/CreateAccount.php";

        MaskArray[0] = "";
        string mask = "";

        for (int cnt = 0; cnt <= MaxPW - 1; cnt++)
            if (cnt != 0)
            {
                MaskArray[cnt] = mask + "*";
                mask = mask + "*";
            }

        StartCoroutine(StartFade(true));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Screen.fullScreen = !Screen.fullScreen;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    Selectable selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                    if (selectable != null)
                        selectable.Select();
                }
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    Selectable selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                    if (selectable != null)
                        selectable.Select();
                }
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

        if (webRequest.text == "PE")
        {
            print("Password is Error");
            NowLoginState.text = "패스워드를 잘못 입력하셨습니다. 패스워드를 다시 입력해주세요.";

            IDInputField.text = "";
            PWInputField.text = "";

            yield return new WaitForSeconds(1);

            NowLoginState.text = "";
        }
        else if (webRequest.text == "NF")
        {
            print("User Not Found");
            NowLoginState.text = "사용자를 찾을 수 없습니다. ID를 다시 입력해주세요.";

            IDInputField.text = "";
            PWInputField.text = "";

            yield return new WaitForSeconds(1);

            NowLoginState.text = "";
        }
        else
        {
            string[] stringSeparators = new string[] { "\n" };
            string[] lines = webRequest.text.Split(stringSeparators, System.StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; ++i)
            {
                string[] _parts = lines[i].Split(',');
                SurRank = _parts[0];
                MurRank = _parts[1];
            }

            user_id = IDInputField.text;

            StartCoroutine(StartFade(false));
        }

        StopCoroutine(LoginGo());
    }

    public static string GetID() { return user_id; }
    public static string GetSurRank() { return SurRank; }
    public static string GetMurRank() { return MurRank; }
    public static void SetSurRank(string rank) { SurRank = rank; }
    public static void SetMurRank(string rank) { MurRank = rank; }

    public void OpenCreateAccountButton()
    {
        IDInputField.text = "";
        PWInputField.text = "";

        LoginPanelObj.SetActive(false);
        CreateAccountPanelObj.SetActive(true);
    }

    public void CreateAccountButton()
    {
        StartCoroutine(CreateGo());

        NewIDInputField.text = "";
        NewPWInputField.text = "";

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

        //NowLoginState.text = "존재하는 아이디입니다.";
        //yield return new WaitForSeconds(1);
        //NowLoginState.text = "";
    }

    IEnumerator StartFade(bool start)
    {
        Fade.SetActive(true);
        Image imgFade = Fade.GetComponent<Image>();

        Color color = imgFade.color;
        float time = 0;

        if (start)
        {
            color.a = 1;
            while (color.a > 0)
            {
                time += Time.deltaTime * 0.5f;
                color.a = Mathf.Lerp(1, 0, time);
                imgFade.color = color;

                yield return null;
            }
            Fade.SetActive(false);
        }
        else
        {
            color.a = 0;
            while (color.a < 1)
            {
                time += Time.deltaTime;
                color.a = Mathf.Lerp(0, 1, time);
                imgFade.color = color;

                yield return null;
            }

            SceneManager.LoadScene("1. Lobby");
        }
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}