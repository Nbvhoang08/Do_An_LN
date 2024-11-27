using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float moveAmount; // Kích thước ô trong lưới
    public float moveSpeed; // Tốc độ di chuyển
    public float moveSpeedOrigin;

    public float inputDelay; // Độ trễ giữa các lần di chuyển (moveAmount/moveSpeed)
    public LayerMask obstacleLayer; // Layer của vật cản

    private Vector2 targetPosition;
    private float lastMoveTime;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastMovement; // Lưu trữ hướng di chuyển cuối cùng

    public bool  isSpeedUp => SpeedUpDuration != 0;
    public float SpeedUpDuration;

    void Start()
    {
        animator = GetComponent<Animator>();

        Vector2 alignedPosition = new Vector2(
            Mathf.Round(transform.position.x / moveAmount) * moveAmount,
            Mathf.Round(transform.position.y / moveAmount) * moveAmount
        );
        moveSpeed = moveSpeedOrigin;
        transform.position = alignedPosition;
        targetPosition = alignedPosition;
        lastMoveTime = -inputDelay;
        lastMovement = Vector2.down; // Mặc định nhìn xuống
    }

    void Update()
    {
        if (!IsOwner) return;
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        inputDelay = 3 / moveSpeed;
        if (Time.time - lastMoveTime >= inputDelay)
        {
            HandleInput();
        }

        UpdateAnimation();

        moveSpeed = Math.Clamp(moveSpeed, 0, 30f);
        SpeedUpDuration = Math.Clamp(SpeedUpDuration, 0, 6f);

        if (isSpeedUp)
        {
            SpeedUpDuration -= Time.deltaTime;
        }
        else
        {
            moveSpeed = moveSpeedOrigin;
        }
    }




    public void SpeedUp(float buffAmount)
    {
        moveSpeed +=  moveSpeedOrigin *buffAmount;
        SpeedUpDuration += 2;

        return;
        
    }
    

    void HandleInput()
    {
        movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (!IsObstacleInDirection(Vector2.up))
                movement.y = 1;
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (!IsObstacleInDirection(Vector2.down))
                movement.y = -1;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (!IsObstacleInDirection(Vector2.left))
                movement.x = -1;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (!IsObstacleInDirection(Vector2.right))
                movement.x = 1;
        }

        if (movement != Vector2.zero)
        {
            targetPosition += movement * moveAmount;
            lastMoveTime = Time.time;
            lastMovement = movement; // Cập nhật hướng di chuyển cuối cùng

            targetPosition = new Vector2(
                Mathf.Round(targetPosition.x / moveAmount) * moveAmount,
                Mathf.Round(targetPosition.y / moveAmount) * moveAmount
            );
        }
    }

    bool IsObstacleInDirection(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, moveAmount, obstacleLayer);
        return hit.collider != null;
    }

    void UpdateAnimation()
    {
        if (movement != Vector2.zero)
        {
            // Đang di chuyển
            animator.SetLayerWeight(1, 1); // Layer Run
            animator.SetLayerWeight(0, 0); // Layer Idle
            animator.SetFloat("x", movement.x);
            animator.SetFloat("y", movement.y);
        }
        else
        {
            // Đang đứng yên
            animator.SetLayerWeight(1, 0); // Layer Run
            animator.SetLayerWeight(0, 1); // Layer Idle
            animator.SetFloat("x", lastMovement.x);
            animator.SetFloat("y", lastMovement.y);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector2.up * moveAmount);
        Gizmos.DrawRay(transform.position, Vector2.down * moveAmount);
        Gizmos.DrawRay(transform.position, Vector2.left * moveAmount);
        Gizmos.DrawRay(transform.position, Vector2.right * moveAmount);
    }
}
