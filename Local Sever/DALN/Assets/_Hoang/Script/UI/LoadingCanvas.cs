using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvas : UICanvas
{

    private void OnEnable()
    {
        StartCoroutine(Load()); // goi den khi UI dc bat

    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(3f);
        UIManager.Instance.CloseUIDirectly<LoadingCanvas>();
    }
}
