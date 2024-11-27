using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SmoothFollow : MonoBehaviour
{
    private Transform target; // Nhân vật hoặc đối tượng cần theo dõi
    public float smoothSpeed = 0.125f; // Tốc độ mượt
    public Vector3 offset; // Độ lệch của camera so với đối tượng

    private void Start()
    {
        StartCoroutine(FindLocalPlayer()); // Tìm kiếm Player tương ứng
    }

    private IEnumerator FindLocalPlayer()
    {
        while (target == null)
        {
            // Tìm Player có NetworkObject thuộc sở hữu của client hiện tại
            foreach (var networkObject in FindObjectsOfType<NetworkObject>())
            {
                if (networkObject.IsOwner && networkObject.CompareTag("Player"))
                {
                    target = networkObject.transform; // Gán Player làm mục tiêu
                    break;
                }
            }

            // Chờ một frame trước khi kiểm tra lại
            yield return null;
        }
    }

    private void Update()
    {
        if (target == null) return; // Đảm bảo có đối tượng để theo dõi

        // Vị trí mục tiêu + độ lệch
        Vector3 desiredPosition = target.position + offset;

        // Nội suy giữa vị trí hiện tại và vị trí mong muốn để tạo chuyển động mượt
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Đặt vị trí mới cho camera
        transform.position = smoothedPosition;
    }
}
