using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using TMPro;
public class PlayFabCode : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField UserName, Email, PassWord;
    public Text ErrorMessage;
    public string SenceName  = "";



    void Start()
    {
        ErrorMessage.text = "";
    }

    // Update is called once per frame
    
    public void RegisterClick()
    {
        var register = new RegisterPlayFabUserRequest { Username = UserName.text , Email = Email.text, Password = PassWord.text};
        PlayFabClientAPI.RegisterPlayFabUser(register,OnRegisterSuccess,OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        ErrorMessage.text = "";
        SceneManager.LoadScene(SenceName);
    }
    private void OnRegisterFailure(PlayFabError error)
    {
        if(error.ErrorDetails != null && error.ErrorDetails.Count>0 )
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


    public void LoginClick()
    {
        var login = new LoginWithPlayFabRequest { Username = UserName.text, Password = PassWord.text };
        PlayFabClientAPI.LoginWithPlayFab(login, OnLoginSuccess, OnLoginFailure);

    }
    private void OnLoginSuccess(LoginResult result) 
    {
        ErrorMessage.text = "";
        SceneManager.LoadScene(SenceName);
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
}
