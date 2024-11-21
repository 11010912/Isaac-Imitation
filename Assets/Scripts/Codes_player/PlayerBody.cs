using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBody : MonoBehaviour
{
    public Vector2 inputvec;
    public float speed = 5f;

    private Rigidbody2D rigid;
    private Animator animator;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Vector2 nextVex = inputvec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVex);

        // �Ȱ� �ִ��� ���� ������Ʈ
        animator.SetBool("isWalking", inputvec.magnitude > 0);

        // �̵� ���⿡ ���� �ִϸ��̼� ��ȯ
        if (inputvec.x > 0)
        {
            animator.SetFloat("moveX", 1); // ������
        }
        else if (inputvec.x < 0)
        {
            animator.SetFloat("moveX", -1); // ����
        }
        else
        {
            animator.SetFloat("moveX", 0); // ����
        }
    }

    void OnMove(InputValue value)
    {
        inputvec = value.Get<Vector2>();
    }
}
