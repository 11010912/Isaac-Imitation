using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFly : MonoBehaviour
{
    // �̵� �ӵ�
    public float normalSpeed = 2f;    // ��� �ӵ�
    public float chaseSpeed = 4f;     // ���� �� �ӵ�
    public float distanceThreshold = 5f; // �÷��̾� ������ ������ �Ÿ�

    private Transform player;
    private Rigidbody2D rb;
    private float currentSpeed;

    // �ʱ� ����
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed; // �ʱ� �ӵ��� ��� �ӵ�
    }

    // �����Ӹ��� ȣ��Ǵ� �޼���
    void Update()
    {
        if (player != null)
        {
            // �÷��̾���� �Ÿ� ���
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // �÷��̾ ���� �Ÿ� ���� ������ ���� ���� ��ȯ
            if (distanceToPlayer <= distanceThreshold)
            {
                currentSpeed = chaseSpeed;
                ChasePlayer();
            }
            else
            {
                currentSpeed = normalSpeed;
                MoveRandomly();
            }
        }
    }

    // �÷��̾ �����ϴ� �޼���
    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * currentSpeed * Time.deltaTime);
    }

    // ��ҿ� õõ�� �����̴� �޼��� (���� ������)
    void MoveRandomly()
    {
        Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        rb.MovePosition(rb.position + randomDirection * currentSpeed * Time.deltaTime);
    }
}
