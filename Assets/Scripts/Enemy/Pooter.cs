using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooter : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float shootingInterval = 2f;
    public float attackDistance = 8f; // �����Ÿ�
    public float wanderRange = 2f; // �ֺ����� ������ ����

    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public Transform tailPosition; // ���� ��ġ�� ���� Transform

    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float nextShootTime;
    private Vector2 wanderTarget;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        nextShootTime = Time.time + shootingInterval;

        // ó�� ��ǥ ��ġ ����
        wanderTarget = transform.position;
    }

    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            // �÷��̾ �����Ÿ� �ȿ� �ִ� ��� �߻�
            if (distanceToPlayer <= attackDistance)
            {
                if (Time.time >= nextShootTime)
                {
                    animator.SetTrigger("isShooting"); // �߻� �ִϸ��̼� Ʈ����
                    nextShootTime = Time.time + shootingInterval;
                }

                // �÷��̾� �������� �ٶ󺸰� �ϱ�
                FlipSprite();
            }
            else
            {
                WanderAround(); // �÷��̾ �����Ÿ� �ۿ� ������ �ֺ� ����
            }
        }
    }

    // �ִϸ��̼� �̺�Ʈ�� ���� ȣ��Ǵ� �޼���
    void Shoot()
    {
        if (player == null || tailPosition == null) return;

        Vector2 direction = (player.position - tailPosition.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, tailPosition.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = direction * bulletSpeed;

        Destroy(bullet, 3f);
    }

    void FlipSprite()
    {
        if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void WanderAround()
    {
        // �ֺ� ���� ��ġ�� ���ݾ� �̵�
        if (Vector2.Distance((Vector2)transform.position, wanderTarget) < 0.1f)
        {
            // ���� ���� �������� ���� ��ǥ ��ġ ����
            wanderTarget = (Vector2)transform.position + new Vector2(Random.Range(-wanderRange, wanderRange), Random.Range(-wanderRange, wanderRange));
        }

        Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
    }
}

