using UnityEngine;
using Photon.Pun;
using System;

public class Player : MonoBehaviourPun
{
    public float moveSpeed = 5f;          // 이동 속도
    public float rotationSpeed = 720f;   // 회전 속도
    private Vector3 moveDirection;       // 이동 방향
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Rigidbody 회전 고정
    }

    private void Update()
    {
        UpdateAnimator();
        // 네트워크에서 로컬 플레이어만 입력 처리
        Move();
    }

    private void Move()
    {
        if (!photonView.IsMine) return;

        // 입력 값 가져오기
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

        // 이동 방향 계산
        moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 회전 처리
        if (moveDirection.sqrMagnitude > 0.01f) // 이동 중일 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = rb.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        GetComponent<Animator>().SetFloat("forwardSpeed", localVelocity.z);

    }

    private void FixedUpdate()
    {
        // 네트워크에서 로컬 플레이어만 이동 처리
        if (!photonView.IsMine) return;

        // Rigidbody의 속도를 직접 설정하여 부드럽게 이동
        rb.velocity = moveDirection * moveSpeed + new Vector3(0, rb.velocity.y, 0); // 중력(y축) 보존
    }
}
