using System.Collections;
using UnityEngine;

public class Clot : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float moveDuration = 1.5f;
    public float waitDuration = 1f;
    public float movementRadius = 5f; // �ൿ �ݰ� ����

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public Transform mouthPosition; // ������ �߻�� �� �κ� ��ġ

    private Vector2 startingPosition; // �ൿ �ݰ��� �߽���
    private Rigidbody2D rb;
    private Animator animator;
    private bool isMoving = true; // ���� �̵� ������ ����
    private bool isShooting = false; // ���� ������ ����
    private bool canShoot = true; // �� ���� ���ݸ� ����ǵ��� ����

    private SpriteRenderer spriteRenderer; // SpriteRenderer �߰�

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // SpriteRenderer �ʱ�ȭ

        // Clot�� �ʱ� ��ġ�� �߽������� ����
        startingPosition = transform.position;

        // �ʱ� �̵� �ڷ�ƾ ����
        StartCoroutine(MoveShootCycle());
    }

    // �̵��� ������ ������ ����
    private IEnumerator MoveShootCycle()
    {
        while (true)
        {
            if (isMoving)
            {
                // �̵� ����
                animator.SetBool("isMoving", true);
                yield return MoveWithinRadius();
                animator.SetBool("isMoving", false);

                isMoving = false; // ������ ���� ���·� ��ȯ
                canShoot = true; // ������ �� �� �ֵ��� ����
            }
            else
            {
                // ���� ����
                if (canShoot && !isShooting)
                {
                    isShooting = true; // ���� ����
                    animator.SetTrigger("clot-shoot");

                    // ������ ���� ������ ���
                    yield return new WaitForSeconds(waitDuration);

                    isShooting = false; // ���� ����
                    canShoot = false; // ���� �̵� ������ �߰� ���� ����
                }

                isMoving = true; // ������ �̵� ���·� ��ȯ
            }
        }
    }

    // �ݰ� ������ ���� �̵�
    private IEnumerator MoveWithinRadius()
    {
        Vector2 targetPosition;
        float timer = 0f;

        // �ݰ� �� ������ ��ġ ����
        do
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            targetPosition = startingPosition + randomDirection * Random.Range(0f, movementRadius);

        } while (Vector2.Distance(startingPosition, targetPosition) > movementRadius);

        // ��ǥ �������� �̵�
        while (timer < moveDuration)
        {
            Vector2 moveDirection = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;

            // �̵� ���⿡ ���� ��������Ʈ ȸ��
            FlipSprite(moveDirection.x);

            timer += Time.deltaTime;

            // ��ǥ ������ ��������� �̵� ����
            if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
                break;

            yield return null;
        }

        rb.velocity = Vector2.zero;
    }

    // ��������Ʈ ���� ��ȯ
    private void FlipSprite(float moveDirectionX)
    {
        if (moveDirectionX < 0)
        {
            spriteRenderer.flipX = true; // �������� �̵��� �� ��������Ʈ �¿� ����
        }
        else if (moveDirectionX > 0)
        {
            spriteRenderer.flipX = false; // ���������� �̵��� �� ���� ��������
        }
    }

    // �ִϸ��̼� �̺�Ʈ���� ȣ��Ǵ� Shoot �޼���
    public void Shoot()
    {
        if (isShooting)
        {
            ShootFromMouth();
        }
    }

    // �Կ��� X�� �������� 4���� �߻�
    private void ShootFromMouth()
    {
        Vector2[] directions = new Vector2[]
        {
            new Vector2(1, 1),   // ������ ��
            new Vector2(-1, 1),  // ���� ��
            new Vector2(1, -1),  // ������ �Ʒ�
            new Vector2(-1, -1)  // ���� �Ʒ�
        };

        foreach (Vector2 dir in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, mouthPosition.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = dir.normalized * bulletSpeed;

            // 3�� �� źȯ �ڵ� �ı�
            Destroy(bullet, 3f);
        }
    }
}




