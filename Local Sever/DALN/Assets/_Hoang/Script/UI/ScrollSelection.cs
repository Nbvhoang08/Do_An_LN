using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class ScrollSelection : NetworkBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Image characterDisplayImage;

    private static int selectedCharacterIndex = 0;
    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(0, 0, 0),
        new Vector3(-39, -6, 0),
        new Vector3(-3, -15, 0)
    };

    private void Update()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            UpdateCharacterSelection();
        }
    }

    private void Start()
    {
        // Giới hạn số lượng client tối đa là 3
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // Kiểm tra số lượng client hiện tại
        if (NetworkManager.Singleton.ConnectedClients.Count >= 3)
        {
            response.Approved = false;
            response.Reason = "Server is full!";
            return;
        }

        response.Approved = true;
        response.Position = Vector3.zero;
        response.Rotation = Quaternion.identity;
    }

    private void UpdateCharacterSelection()
    {
        selectedCharacterIndex = Mathf.Clamp(
            Mathf.RoundToInt(scrollRect.horizontalNormalizedPosition * (characterPrefabs.Length - 1)),
            0,
            characterPrefabs.Length - 1
        );

        if (selectedCharacterIndex >= 0 && selectedCharacterIndex < characterPrefabs.Length)
        {
            characterDisplayImage.sprite = characterPrefabs[selectedCharacterIndex].GetComponent<SpriteRenderer>().sprite;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient && !IsHost)
        {
            RequestSpawnServerRpc(selectedCharacterIndex);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnServerRpc(int characterIndex, ServerRpcParams serverRpcParams = default)
    {
        ulong clientId = serverRpcParams.Receive.SenderClientId;

        // Sử dụng số lượng client đã kết nối để xác định vị trí spawn
        int spawnIndex = NetworkManager.Singleton.ConnectedClients.Count - 1;
        if (spawnIndex >= 0 && spawnIndex < spawnPositions.Length)
        {
            Vector3 spawnPosition = spawnPositions[spawnIndex];
            GameObject playerInstance = Instantiate(characterPrefabs[characterIndex], spawnPosition, Quaternion.identity);
            NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
            networkObject.SpawnWithOwnership(clientId);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy(); // Gọi OnDestroy của lớp base
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }
}
