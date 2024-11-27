using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Door_controller : NetworkBehaviour
{
    public Transform leftDoor, rightDoor;
    public Transform checkPoint;  // Vị trí trung tâm hình chữ nhật kiểm tra
    public Vector2 detectionSize = new Vector2(2f, 1f);  // Kích thước hình chữ nhật
    public LayerMask playerLayer;  // Lớp của Player để kiểm tra

     public float openDistance = 1f;
    public float doorSpeed = 2f;

    private Vector3 leftDoorStartPos, rightDoorStartPos;


    private bool isOpening, isClosing;

    void Start()
    {
        if (leftDoor) leftDoorStartPos = leftDoor.transform.position;
        if (rightDoor) rightDoorStartPos = rightDoor.transform.position;

    }

    void Update()
    {
        bool playerDetected = CheckForPlayer();

        if (playerDetected && !isOpening && !isClosing)
        {
            OpenDoors();
            
        }
        else if (!playerDetected && !isClosing && !isOpening)
        {
            CloseDoors();
        }

        if (isOpening) AnimateOpenDoors();
        else if (isClosing) AnimateCloseDoors();
    }

    bool CheckForPlayer()
    {
        // Kiểm tra player bằng OverlapBox (raycast hình chữ nhật)
        Collider2D hit = Physics2D.OverlapBox(
            checkPoint.position, detectionSize, 0f, playerLayer
        );
        return hit != null;
    }

    public void OpenDoors()
    {
        isOpening = true;
        isClosing = false;
    }

    public void CloseDoors()
    {
        isClosing = true;
        isOpening = false;
    }

    void AnimateOpenDoors()
    {
        bool leftDoorFinished = true;
        bool rightDoorFinished = true;

        if (leftDoor)
        {
            Vector3 targetPos = leftDoorStartPos + Vector3.left * openDistance;
            leftDoor.transform.position = Vector3.MoveTowards(
                leftDoor.transform.position, targetPos, doorSpeed * Time.deltaTime
            );
            leftDoorFinished = Vector3.Distance(leftDoor.transform.position, targetPos) < 0.01f;
        }

        if (rightDoor)
        {
            Vector3 targetPos = rightDoorStartPos + Vector3.right * openDistance;
            rightDoor.transform.position = Vector3.MoveTowards(
                rightDoor.transform.position, targetPos, doorSpeed * Time.deltaTime
            );
            rightDoorFinished = Vector3.Distance(rightDoor.transform.position, targetPos) < 0.01f;
        }

        if (leftDoorFinished && rightDoorFinished) isOpening = false;
    }

    void AnimateCloseDoors()
    {
        bool leftDoorFinished = true;
        bool rightDoorFinished = true;

        if (leftDoor)
        {
            leftDoor.transform.position = Vector3.MoveTowards(
                leftDoor.transform.position, leftDoorStartPos, doorSpeed * Time.deltaTime
            );
            leftDoorFinished = Vector3.Distance(leftDoor.transform.position, leftDoorStartPos) < 0.01f;
        }

        if (rightDoor)
        {
            rightDoor.transform.position = Vector3.MoveTowards(
                rightDoor.transform.position, rightDoorStartPos, doorSpeed * Time.deltaTime
            );
            rightDoorFinished = Vector3.Distance(rightDoor.transform.position, rightDoorStartPos) < 0.01f;
        }

        if (leftDoorFinished && rightDoorFinished) isClosing = false;
    }

    void OnDrawGizmos()
    {
        if (checkPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(checkPoint.position, detectionSize);
        }
    }
}
