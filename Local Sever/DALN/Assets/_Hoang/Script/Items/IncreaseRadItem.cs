using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class IncreaseRadItem : NetworkBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            other.GetComponent<Player>().IncreaseMaxRad();
            StartCoroutine(Despawn());
            SoundManger.Instance.PlayVFXSound(1);
        }
    }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(0.2f);
        RequestDestroyServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    public void RequestDestroyServerRpc()
    {
        NetworkObject.Despawn(true);
    }

}
