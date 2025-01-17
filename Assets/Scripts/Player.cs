using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviourPun
{
    public float moveSpeed = 5f;          // 이동 속도
    public float rotationSpeed = 720f;   // 회전 속도

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Rigidbody 회전 고정
    }

    private void FixedUpdate()
    {
        // 네트워크에서 로컬 플레이어만 이동 처리
        if (!photonView.IsMine) return;

        // 입력 값 가져오기
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

        // 이동 방향 계산
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 이동 처리
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        // 회전 처리
        if (moveDirection.sqrMagnitude > 0.01f) // 이동 중일 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
