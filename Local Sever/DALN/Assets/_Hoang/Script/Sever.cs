using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using PlayFab;



public class Sever : MonoBehaviour
{
    // Start is called before the first frame update
    public bool RunLocal;
    public NetworkDriver networkDriver;
    private NativeList<NetworkConnection> connections;

    private Dictionary<int, PlayerInfo> players; // Lưu thông tin của mỗi player
    private int maxPlayers = 4; // Số lượng player tối đa

    [System.Serializable]
    public class PlayerInfo
    {
        public int playerId;
        public Vector3 position;
        public Quaternion rotation;
        public bool isReady;
        // Thêm các thông tin khác của player tùy vào game của bạn
        // Ví dụ: health, score, state,...
    }

    void Start()
    {
        if (RunLocal)
        {
            StartSever();
        }
        else
        {
            // TO DO : Start from  Playfab configuration
            StartPlayFabAPI();
        }
    }

    // Update is called once per frame

    void Update()
    {
        networkDriver.ScheduleUpdate().Complete();

        // Cleanup dead connections
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated)
            {
                connections.RemoveAtSwapBack(i);
                RemovePlayer(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = networkDriver.Accept()) != default(NetworkConnection))
        {
            if (connections.Length < maxPlayers)
            {
                connections.Add(c);
                int playerId = connections.Length - 1;
                AddNewPlayer(playerId);
                Debug.Log($"Player {playerId} connected. Total players: {connections.Length}");
            }
            else
            {
                // Server đầy, từ chối kết nối
                c.Disconnect(networkDriver);
                Debug.Log("Server is full, connection rejected");
            }
        }

        // Handle data from connected players
        for (int i = 0; i < connections.Length; i++)
        {
            if (!connections[i].IsCreated) continue;

            NetworkEvent.Type cmd;
            DataStreamReader stream;
            while ((cmd = networkDriver.PopEventForConnection(connections[i], out stream)) !=
                    NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    ProcessPlayerData(i, stream);
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log($"Player {i} disconnected");
                    connections[i] = default(NetworkConnection);
                    /*RemovePlayer(i);*/
                    if(connections.Length == 0)
                    {
                        OnShutDown();
                    }
                }
            }

            // Broadcast game state to this player
            if (connections[i].IsCreated)
            {
                SendGameState(i);
            }
        }
    }

    private void ProcessPlayerData(int playerId, DataStreamReader stream)
    {
        byte messageType = stream.ReadByte();

        switch (messageType)
        {
            case 0: // Player Position Update
                if (players.TryGetValue(playerId, out PlayerInfo player))
                {
                    player.position = new Vector3(
                        stream.ReadFloat(),
                        stream.ReadFloat(),
                        stream.ReadFloat()
                    );
                    player.rotation = new Quaternion(
                        stream.ReadFloat(),
                        stream.ReadFloat(),
                        stream.ReadFloat(),
                        stream.ReadFloat()
                    );
                }
                break;

            case 1: // Player Ready State
                if (players.TryGetValue(playerId, out PlayerInfo readyPlayer))
                {
                    readyPlayer.isReady = stream.ReadByte() == 1;
                }
                break;

                // Thêm các case khác tùy theo nhu cầu game
                // Ví dụ: chat messages, player actions, etc.
        }
    }

    private void SendGameState(int toPlayerId)
    {
        networkDriver.BeginSend(NetworkPipeline.Null, connections[toPlayerId], out var writer);

        // Gửi số lượng player
        writer.WriteInt(players.Count);

        // Gửi thông tin của tất cả players
        foreach (var playerInfo in players)
        {
            writer.WriteInt(playerInfo.Key); // Player ID
            writer.WriteFloat(playerInfo.Value.position.x);
            writer.WriteFloat(playerInfo.Value.position.y);
            writer.WriteFloat(playerInfo.Value.position.z);
            writer.WriteFloat(playerInfo.Value.rotation.x);
            writer.WriteFloat(playerInfo.Value.rotation.y);
            writer.WriteFloat(playerInfo.Value.rotation.z);
            writer.WriteFloat(playerInfo.Value.rotation.w);
            writer.WriteByte(playerInfo.Value.isReady ? (byte)1 : (byte)0);
        }

        networkDriver.EndSend(writer);
    }

    private void AddNewPlayer(int playerId)
    {
        PlayerInfo newPlayer = new PlayerInfo
        {
            playerId = playerId,
            position = Vector3.zero,
            rotation = Quaternion.identity,
            isReady = false
        };
        players[playerId] = newPlayer;
    }

    private void RemovePlayer(int playerId)
    {
        if (players.ContainsKey(playerId))
        {
            players.Remove(playerId);
            Debug.Log($"Player {playerId} removed from game state");
        }
    }
    void StartSever()
    {
        Debug.Log("Starting Sever");

        networkDriver = NetworkDriver.Create();
        var endpoint = NetworkEndpoint.AnyIpv4;
        endpoint.Port = 777;
        var connectionInfo = PlayFabMultiplayerAgentAPI.GetGameServerConnectionInfo();
        if(connectionInfo != null)
        {
            foreach(var port in connectionInfo.GamePortsConfiguration)
            {
                endpoint.Port = (ushort)port.ServerListeningPort;
                break;
            }
        }
        if (networkDriver.Bind(endpoint) !=0)
        {
            Debug.Log("Failed to bind to port");
        }
        else
        {
            networkDriver.Listen();
        }
        connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
    }
    void OnDestroy()
    {
        networkDriver.Dispose();
        connections.Dispose();
    }
    IEnumerator ReadyForPlayer()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }
    private void OnSeverActive()
    {
        StartSever();
    }
    private void OnAgentError(string error)
    {
        Debug.Log(error);
    }
    private void OnShutDown()
    {
        Debug.Log("Sever is Shutting down");
        networkDriver.Dispose();
        connections.Dispose();
        StartCoroutine(ShutDown());
    }

    IEnumerator ShutDown()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }
     void StartPlayFabAPI()
    {
        PlayFabMultiplayerAgentAPI.Start();
        PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenace;
        PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutDown;
        PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnSeverActive;
        PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;


        StartCoroutine(ReadyForPlayer());
    }
    private void OnMaintenace(DateTime ? NextScheduledMaintenaceUtc)
    {
        Debug.LogFormat("Maintenance scheduled for : {0}",
            NextScheduledMaintenaceUtc.Value.ToLongDateString());
    }


}
