using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage;
    public int per;

    private Rigidbody2D rigid; // Rigid �ʵ带 �����մϴ�.

    void Awake()
    {
        // Rigidbody2D ������Ʈ�� �����ɴϴ�.
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;

        if (per > -1)
        {
            // �ʱ� �ӵ��� �����մϴ�.
            rigid.velocity = dir;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // "Enemy" �±װ� �ƴϰų� per ���� -1�̸� �ƹ� �۾��� ���� �ʽ��ϴ�.
        if (!collision.CompareTag("Enemy") || per == -1)
            return;

        per--;

        if (per == -1)
        {
            // �ӵ��� 0���� �����ϰ�, ��ü�� ��Ȱ��ȭ�մϴ�.
            rigid.velocity = Vector2.zero;
            gameObject.SetActive(false);
        }
    }
}
