using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using TMPro;

public class LoginCanvas : UICanvas
{
    public TMP_InputField UserName,PassWord;
    public Text ErrorMessage;
    public string SenceName = "";
    public Animator Animator;
    void Start()
    {
        ErrorMessage.text = "";
    }
    public override void Open()
    {
        base.Open();
        if(Animator != null)
        {
            Animator.SetTrigger("In");
        }
    }

    public void LoginClick()
    {
        var login = new LoginWithPlayFabRequest { Username = UserName.text, Password = PassWord.text };
        PlayFabClientAPI.LoginWithPlayFab(login, OnLoginSuccess, OnLoginFailure);

    }
    private void OnLoginSuccess(LoginResult result)
    {
        ErrorMessage.text = "";
        /*SceneManager.LoadScene(SenceName); */

        UIManager.Instance.CloseUI<LoginCanvas>(0.3f);
        UIManager.Instance.OpenUI<LoadingCanvas>();
        UIManager.Instance.OpenUI<HomeCanvas>();
        
    }
    private void OnLoginFailure(PlayFabError error)
    {
        if (error.ErrorDetails != null && error.ErrorDetails.Count > 0)
        {
            using (var iter = error.ErrorDetails.Keys.GetEnumerator())
            {
                iter.MoveNext();
                string key = iter.Current;
                ErrorMessage.text = error.ErrorDetails[key][0];
            }
        }
        else
        {
            ErrorMessage.text = error.ErrorMessage;
        }
    }

    public void RegisterClick()
    {
        Animator.SetTrigger("Out");
        UIManager.Instance.CloseUI<LoginCanvas>(1f);
      

    }
    public void OpenRegister(){
        UIManager.Instance.OpenUI<RegisterCanvas>();
    }
}
