using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHead : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed = 5f; // �⺻ �ӵ� ����
    public InputActionAsset inputActions; // InputActionAsset �߰�

    private Rigidbody2D rigid;
    private SpriteRenderer spriter;
    private Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // InputActionAsset ����Ͽ� �Է°� �б�
        if (inputActions != null)
        {
            inputVec = inputActions.FindAction("Move").ReadValue<Vector2>();
        }
        else
        {
            // ���� Input ���
            inputVec.x = Input.GetAxis("Horizontal");
            inputVec.y = Input.GetAxis("Vertical");
        }
    }

    void FixedUpdate()
    {
        // �Է� ���� ����ȭ �� �̵� ó��
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        anim.SetFloat("speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }
}
