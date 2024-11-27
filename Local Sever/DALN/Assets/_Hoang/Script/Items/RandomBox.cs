using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class RandomBox : NetworkBehaviour
{
    [SerializeField] private List<GameObject> itemsList; // Danh sách prefab các item (phải có NetworkObject)

    private void RandomItems(int randIndex)
    {
        int itemIndex = -1; // Mặc định là không hợp lệ

        switch (randIndex)
        {
            case int n when n >= 0 && n <= 20:
                break;

            case int n when n > 20 && n <= 50:
                itemIndex = 0;
                break;

            case int n when n > 50 && n <= 80:
                itemIndex = 1;
                break;

            case int n when n > 80 && n <= 90:
                itemIndex = 3;
                break;

            default:
                itemIndex = 2;
                break;
        }

        // Gọi ServerRpc để spawn item trên server
        
        if (itemIndex >= 0 && itemIndex < itemsList.Count)
        {
            RequestSpawnItemServerRpc(itemIndex);
        }
        else
        {
            return;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnItemServerRpc(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < itemsList.Count)
        {
            GameObject itemPrefab = itemsList[itemIndex];

            // Kiểm tra prefab có NetworkObject không
            if (itemPrefab.TryGetComponent<NetworkObject>(out NetworkObject networkObject))
            {
                GameObject spawnedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
                spawnedItem.GetComponent<NetworkObject>().Spawn(); // Spawn qua NetworkObject
            }
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(0.5f);

        // Huỷ RandomBox qua server để đồng bộ
        if (IsServer)
        {
            NetworkObject.Despawn();
        }
        else
        {
            RequestDestroyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestDestroyServerRpc()
    {
        NetworkObject.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            int randomValue = Random.Range(0, 100);
            RandomItems(randomValue);
            StartCoroutine(Despawn());
        }
    }
}
