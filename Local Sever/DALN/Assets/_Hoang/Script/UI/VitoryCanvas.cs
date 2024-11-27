using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class VitoryCanvas : UICanvas
{
    // Start is called before the first frame update

    public bool canReturn;
    private void OnEnable()
    {
        canReturn = false;
        StartCoroutine(returnHome());
    }
    IEnumerator returnHome()
    {
        yield return new WaitForSeconds(3);
        canReturn = true;
    }

    public void HomeBtn()
    {
        if (canReturn)
        {
            // Ngắt kết nối client khỏi server
            if (NetworkManager.Singleton.IsClient)
            {
                NetworkManager.Singleton.Shutdown(); // Ngắt kết nối client
            }
            UIManager.Instance.OpenUI<LoadingCanvas>();
            UIManager.Instance.OpenUI<HomeCanvas>();
            StartCoroutine(LoadHomeScene());
        }
        else
        {
            return;
        }
      
    }
    IEnumerator LoadHomeScene()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("Home");
        UIManager.Instance.CloseUIDirectly<VitoryCanvas>();
        

    }


}
