using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ObjectOT : NetworkBehaviour
{
    // Start is called before the first frame update
    public bool notEvent;
    void Awake()
    {
        if (notEvent)
        {
            StartCoroutine(DesSpawn());
        }
    }

    // Update is called once per frame
  
    [ServerRpc(RequireOwnership = false)]
    public void RequestDestroyServerRpc()
    {
        NetworkObject.Despawn(true); // Huỷ đối tượng bom
    }

    private IEnumerator DesSpawn()
    {
        
        yield return new WaitForSeconds(3f);
        RequestDestroyServerRpc();


    }

}
