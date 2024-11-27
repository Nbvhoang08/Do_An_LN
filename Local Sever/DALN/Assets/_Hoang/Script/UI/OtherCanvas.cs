using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherCanvas : UICanvas
{
    public void BackBtn()
    {
        UIManager.Instance.CloseUIDirectly<OtherCanvas>();
    }
    
}
