using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactCanvas : UICanvas
{
    // Start is called before the first frame update
    public void BackBtn()
    {
        UIManager.Instance.CloseUIDirectly<ContactCanvas>();
    }
}
