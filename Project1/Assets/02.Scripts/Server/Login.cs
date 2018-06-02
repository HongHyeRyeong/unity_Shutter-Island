using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour {

    public InputField IDInputField;
    public InputField PWInputField;

    public InputField NewIDInputField;
    public InputField NewPWInputField;

    public string LoginURL;

    void Start()
    {
        LoginURL = "wjddus0424.dothome.co.kr/Login.php";
    }

    public void LoginButton()
    {
        StartCoroutine(LoginGo());
    }

    IEnumerator LoginGo()
    {
        print(IDInputField.text);
        print(PWInputField.text);

        WWWForm form = new WWWForm();
        form.AddField("Input_id", IDInputField.text);
        form.AddField("Input_pw", PWInputField.text);

        WWW webRequest = new WWW(LoginURL, form);
        yield return webRequest;

        print(webRequest.text);

        yield return null;
    }

    public void CreateAccountButton()
    {
        //StartCoroutine(LoginGo());
    }
}
