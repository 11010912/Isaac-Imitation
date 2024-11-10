using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHead_0 : MonoBehaviour
{
    public Vector2 inputvec;
    public float speed; // speed ������ �߰��մϴ�.

    Rigidbody2D rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputvec.x = Input.GetAxis("Horizontal"); // �빮�ڷ� ����
        inputvec.y = Input.GetAxis("Vertical");   // �빮�ڷ� ����
    }

    void FixedUpdate()
    {
        Vector2 nextVex = inputvec.normalized * speed * Time.fixedDeltaTime; // ��Ÿ ���� �� �ϰ��� ����
        rigid.MovePosition(rigid.position + nextVex);
    }
}
