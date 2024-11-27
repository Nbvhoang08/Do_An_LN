using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject BombPrefab;
    [SerializeField] private bool CanSet;
    [SerializeField] private int maxRadius;
    [SerializeField] private float _coolDown;
    [SerializeField] private bool _isInvinCable;
    [SerializeField] private GameObject shield;
    public float fadeDuration = 1.0f;
    private SpriteRenderer spriteRenderer;
    private NetworkVariable<float> fadeTimer = new NetworkVariable<float>(0.0f);
    public NetworkVariable<bool> IsDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> IsWin = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private PlayerMovement playerMove;
    private bool hasShownWinUI = false;
    private bool _gameStarted = false; // Biến theo dõi trạng thái game

    public TextMeshPro PlayerName;

    [SerializeField] private NetworkVariable<FixedString128Bytes> NetWorkPlayerName = new NetworkVariable<FixedString128Bytes>(default,  NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);
    
    public void Awake()
    {
        if (playerMove == null)
        {
            playerMove = GetComponent<PlayerMovement>();
        }
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
        // Chỉ server thay đổi tên khi spawn

        NetWorkPlayerName.OnValueChanged += OnPlayerNameChanged;
        SetPlayerNameServerRpc();
    


    }

    private void OnPlayerNameChanged(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        // Update the UI for all clients when the network variable changes
        PlayerName.text = newValue.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc()
    {
        SetPlayerName();
    }

    private void SetPlayerName()
    {
        // Set the player name with the client ID
        string newPlayerName = $"Player: {OwnerClientId}";
        NetWorkPlayerName.Value = new FixedString128Bytes(newPlayerName);
    }



    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallback;

        NetWorkPlayerName.OnValueChanged -= OnPlayerNameChanged;
    }
  
    private void OnClientConnectedCallback(ulong clientId)
    {
        if (GameStarted())
        {
            Debug.Log("Game has started due to enough players joining.");
        }
    }

    private void OnClientDisconnectedCallback(ulong clientId)
    {
        Debug.Log($"Client {clientId} has disconnected.");
    }




    public bool GameStarted()
    {
        // Kiểm tra xem số lượng client kết nối trong server có từ 2 đến 3
        if (!_gameStarted && NetworkManager.Singleton.ConnectedClientsIds.Count == 3)
        {
            _gameStarted = true; // Cập nhật trạng thái game chỉ lần đầu tiên
            Debug.Log("Game has started!");
        }

        return _gameStarted;
    }

    void Start()
    {
        if (IsOwner)
        {
            IsDead.Value = false;
            IsWin.Value = false;
        }
        CanSet = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
    }

    void Update()
    {
        if (!IsOwner) return;

        
        if (!IsDead.Value)
        {
            SetBomb();
            if (playerMove.gameObject.activeSelf == false)
            {
                playerMove.enabled = true;
            }

            if (_isInvinCable)
            {
                if (!shield.activeSelf)
                {
                    shield.SetActive(true);
                }
            }
            else
            {
                if (shield.activeSelf)
                {
                    shield.SetActive(false);
                }
            }
        }
        else
        {
            UpdateFadeEffectServerRpc();  // Chuyển fade về client
        }

        if (IsWin.Value && !hasShownWinUI)
        {
            UIManager.Instance.OpenUI<VitoryCanvas>();
            hasShownWinUI = true;
        }

        if (GameStarted() && IsOwner)
        {
            CheckWinCondition();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateFadeEffectServerRpc()
    {
        // Tăng thời gian fade trên server
        fadeTimer.Value += Time.deltaTime;

        // Tính toán giá trị alpha
        float alpha = Mathf.Lerp(1.0f, 0.0f, fadeTimer.Value / fadeDuration);

        // Gửi alpha xuống client để cập nhật hiệu ứng
        UpdateFadeEffectClientRpc(alpha);

        // Nếu fade đã hoàn tất, vô hiệu hóa chuyển động
        if (fadeTimer.Value >= fadeDuration)
        {
            playerMove.enabled = false;
        }
    }

    [ClientRpc]
    private void UpdateFadeEffectClientRpc(float alpha)
    {
        // Cập nhật alpha trên client
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
        }
    }

    private void CheckWinCondition()
    {
        int alivePlayers = 0;
        Player lastAlivePlayer = null;

        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (!player.IsDead.Value)
            {
                alivePlayers++;
                lastAlivePlayer = player;
            }
        }

        if (alivePlayers == 1 && lastAlivePlayer != null && !lastAlivePlayer.IsWin.Value)
        {
            // Chỉ cho phép client thay đổi IsWin của chính mình
            if (IsOwner && IsOwner == lastAlivePlayer.IsOwner)
            {
                lastAlivePlayer.IsWin.Value = true;
            }
        }
    }

    void SetBomb()
    {
        if (BombPrefab != null && Input.GetKeyDown(KeyCode.Space) && CanSet)
        {
            RequestBombSpawnServerRpc();
            CanSet = false;
            StartCoroutine(Cooldown(_coolDown));
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestBombSpawnServerRpc()
    {
        Vector3 normalizedPosition = new Vector3(
            Mathf.Round(transform.position.x / 3) * 3,
            Mathf.Round(transform.position.y / 3) * 3,
            transform.position.z
        );

        GameObject bomb = Instantiate(BombPrefab, normalizedPosition, Quaternion.identity);
       
        bomb.GetComponent<NetworkObject>().Spawn();
        bomb.GetComponent<Bomb>().maxExplosionRadius = maxRadius;
    }

    public void ReduceCoolDown()
    {
        if (_coolDown <= 0.5f) return;
        _coolDown -= _coolDown * 0.2f;
    }

    public void InvinCible()
    {
        if (_isInvinCable) return;
        _isInvinCable = true;
        StartCoroutine(InvincibleDuration());
    }

    public void IncreaseMaxRad()
    {
        maxRadius += 3;
        return;
    }



    IEnumerator Cooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        CanSet = true;
    }

    IEnumerator InvincibleDuration()
    {
        yield return new WaitForSeconds(5);
        _isInvinCable = false;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Explosion"))
        {
            if (!_isInvinCable)
            {
                UpdateDead(true);
                StartCoroutine(Die());
            }
            
        }
    }

    
    private void UpdateDead(bool isDead)
    {
        if(IsOwner)
        {
            IsDead.Value = isDead;
        }   
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.5f);
        if (IsOwner && !IsWin.Value)
        {
            UIManager.Instance.OpenUI<DefeatCanvas>();
        }
    }

}
