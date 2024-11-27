using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.EconomyModels;
using TMPro;

public class RegisterCanvas : UICanvas
{
    // Start is called before the first frame update
    public TMP_InputField UserName, Email, PassWord;
    public Text ErrorMessage;
    public string SenceName = "";
    public Animator Animator;



    void Start()
    {
        ErrorMessage.text = "";
    }

    // Update is called once per frame
    public override void Open()
    {
        base.Open();
        if (Animator != null)
        {
            Animator.SetTrigger("In");
        }
    }
    public void RegisterClick()
    {
        var register = new RegisterPlayFabUserRequest { Username = UserName.text, Email = Email.text, Password = PassWord.text };
        PlayFabClientAPI.RegisterPlayFabUser(register, OnRegisterSuccess, OnRegisterFailure);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        ErrorMessage.text = "";
        /*SceneManager.LoadScene(SenceName);*/
        Animator.SetTrigger("Out");
        UIManager.Instance.CloseUI<RegisterCanvas>(1f);

    }
    private void OnRegisterFailure(PlayFabError error)
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

    public void ReturnLoginClick()
    {
        Animator.SetTrigger("Out");
        UIManager.Instance.CloseUI<RegisterCanvas>(1.2f);
        

    }
    public void otherBtn()
    {
       //IManager.Instance.CloseUI<RegisterCanvas>(0.2f);
        UIManager.Instance.OpenUI<OtherCanvas>();

    }


    public void OpenLogin()
    {
        UIManager.Instance.OpenUI<LoginCanvas>();
    }
}
