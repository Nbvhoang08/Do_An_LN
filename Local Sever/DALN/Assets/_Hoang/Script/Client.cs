using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using System;
using Unity.Collections;

public class Client : MonoBehaviour
{
    public bool RunLocal;
    private NetworkDriver networkDriver;
    private NetworkConnection networkConnection;
    private bool isDone;
    private GameObject[] players;
    private Vector2[] playerPositions; // Trạng thái vị trí của player trong 2D
    private bool startedConnectionRequest = false;
    private bool isConnected = false;

    // Hàm kết nối tới server
    private void connectToServer(string address, ushort port)
    {
        Debug.Log("Connecting to " + address + ":" + port);
        networkDriver = NetworkDriver.Create();
        networkConnection = default(NetworkConnection);

        var endpoint = NetworkEndpoint.Parse(address, port);
        networkConnection = networkDriver.Connect(endpoint);
        startedConnectionRequest = true;

        // Tìm 2 player trong game
        players = GameObject.FindGameObjectsWithTag("Player");

        // Kiểm tra có đủ 2 player hay không
        if (players.Length != 2)
        {
            Debug.LogError("Không phát hiện đủ 2 player. Vui lòng đảm bảo có đúng 2 player trong game.");
            return;
        }

        Debug.Log("Detected " + players.Length + " players");

        // Sắp xếp theo tên để giữ tính nhất quán
        Array.Sort(players, (p1, p2) => p1.name.CompareTo(p2.name));

        // Lưu trữ vị trí ban đầu của 2 player
        playerPositions = new Vector2[players.Length];
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                playerPositions[i] = players[i].transform.position;
            }
        }
    }

    // Giải phóng NetworkDriver khi đối tượng bị hủy
    public void OnDestroy()
    {
        networkDriver.Dispose();
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting Client");
        if (RunLocal)
        {
            connectToServer("127.0.0.1", 7777);
        }
        else
        {
            // TODO: Kết nối từ cấu hình PlayFab (nếu có)
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedConnectionRequest)
        {
            return;
        }

        networkDriver.ScheduleUpdate().Complete();

        if (!networkConnection.IsCreated)
        {
            if (!isDone)
            {
                Debug.Log("Something went wrong during connect");
            }
            return;
        }

        DataStreamReader stream;
        NetworkEvent.Type cmd;

        if (!isConnected)
        {
            Debug.Log("Connecting...");
        }

        // Xử lý sự kiện mạng
        while ((cmd = networkConnection.PopEvent(networkDriver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");
                isConnected = true;
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                // Nhận dữ liệu vị trí từ server
                for (int i = 0; i < playerPositions.Length; i++)
                {
                    // Đọc vị trí 2D được gửi từ server (chỉ có x và y)
                    Vector2 newPosition = new Vector2(stream.ReadFloat(), stream.ReadFloat());
                    players[i].transform.position = new Vector3(newPosition.x, newPosition.y, players[i].transform.position.z); // Cập nhật vị trí player, giữ nguyên z
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                networkConnection = default(NetworkConnection);
                isConnected = false;
            }
        }

        // Cập nhật vị trí hiện tại của player
        for (var i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                playerPositions[i] = players[i].transform.position;
            }
        }

        // Gửi vị trí của player tới server để đồng bộ
        if (isConnected)
        {
            networkDriver.BeginSend(networkConnection, out var writer);
            for (int i = 0; i < playerPositions.Length; i++)
            {
                // Gửi vị trí 2D (chỉ gửi x và y)
                writer.WriteFloat(playerPositions[i].x);
                writer.WriteFloat(playerPositions[i].y);
            }
            networkDriver.EndSend(writer);
        }
    }
}
