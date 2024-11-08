using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �⺻ �̵� �ӵ�
    [SerializeField]
    private float moveSpeed = 5f;

    // Rigidbody2D�� Animator ������Ʈ
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    // �̵� �ӵ��� �ܺο��� �����ϴ� ������Ƽ
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = Mathf.Max(0, value); } // �ӵ��� ������ ���� �ʵ��� ����
    }

    // �ʱ� ����
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // �����Ӹ��� ȣ��Ǵ� �޼���
    void Update()
    {
        // �Է��� �����Ͽ� �̵� ���� ����
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // �ִϸ��̼� ������Ʈ
        UpdateAnimation();
    }

    // ������ �̵��� ó���ϴ� FixedUpdate �޼���
    void FixedUpdate()
    {
        Move();
    }

    // �̵� ó�� �޼���
    void Move()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    // �ִϸ��̼� ó�� �޼���
    void UpdateAnimation()
    {
        if (movement != Vector2.zero)
        {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    // �̵� �ӵ��� �����ϴ� �޼���
    public void SetMoveSpeed(float newSpeed)
    {
        MoveSpeed = newSpeed;
    }
}
