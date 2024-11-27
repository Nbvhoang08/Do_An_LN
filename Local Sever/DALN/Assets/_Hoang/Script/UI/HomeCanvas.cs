using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class HomeCanvas : UICanvas
{
    public ScrollSelection selection;
    // Start is called before the first frame update
    public void SettingBtn()
    {   
        UIManager.Instance.OpenUI<SetingCanvas>();// Bat Setting 
    }
    public void ContactBtn()
    {
        UIManager.Instance.OpenUI<ContactCanvas>();// Bat contact
    }
    public void Logout()
    {
        UIManager.Instance.CloseUI<HomeCanvas>(0.5f);
        UIManager.Instance.OpenUI<LoginCanvas>();
    }
    /* public void findMatch()
     {
         UIManager.Instance.OpenUI<LoadingCanvas>();
         StartCoroutine(LoadPlayScene());
     }*/

    public void HostBtn()
    {
        UIManager.Instance.OpenUI<LoadingCanvas>();
        StartCoroutine(LoadHost());
    }
    public void ClientBtn()
    {
        UIManager.Instance.OpenUI<LoadingCanvas>();
        StartCoroutine(LoadClient());
    }

    public void SeverBtn()
    {
        UIManager.Instance.OpenUI<LoadingCanvas>();
        StartCoroutine(LoadSever());
    }


    IEnumerator LoadPlayScene() 
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("MainScene");
        UIManager.Instance.CloseUIDirectly<HomeCanvas>();
        
    }
    IEnumerator LoadHost()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("MainScene");
        yield return null;
        NetworkManager.Singleton.StartHost();
        UIManager.Instance.CloseUI<HomeCanvas>(0.2f);
    }
    IEnumerator LoadSever()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("MainScene");
        yield return null;
        NetworkManager.Singleton.StartServer();
        UIManager.Instance.CloseUI<HomeCanvas>(0.2f);
    }
    IEnumerator LoadClient()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("MainScene");
        yield return null;
        NetworkManager.Singleton.StartClient();
        UIManager.Instance.CloseUI<HomeCanvas>(0.5f);


    }
    
}
