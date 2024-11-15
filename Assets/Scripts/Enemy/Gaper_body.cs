using UnityEngine;

public class Gaper_body : MonoBehaviour
{
    public float moveSpeed = 2f; // �̵� �ӵ�
    public Transform player; // ������ �÷��̾��� ��ġ
    public Animator animator; // �ִϸ�����
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private Vector2 direction; // �̵� ����
    private Vector2 startPosition; // Gaper�� ���� ��ġ
    public float movementRadius = 3f; // �̵��� �� �ִ� ����

    private bool isMovingSide = false; // �¿� �̵� ���� ����

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position; // �ʱ� ��ġ ����
    }

    private void Update()
    {
        MoveTowardsPlayer();
        UpdateAnimation();
    }

    // �÷��̾ ���� �̵� (�̵� ���� ����)
    private void MoveTowardsPlayer()
    {
        Vector2 targetPosition = player.position;
        direction = (targetPosition - (Vector2)transform.position).normalized;

        Vector2 desiredPosition = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;
        Vector2 offset = desiredPosition - startPosition;

        if (offset.magnitude < movementRadius)
        {
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero; // ������ ������ �̵��� ����
        }
    }

    // �ִϸ��̼� ���� ������Ʈ
    private void UpdateAnimation()
    {
        float absX = Mathf.Abs(direction.x);
        float absY = Mathf.Abs(direction.y);

        // �ִϸ��̼��� ������ ������ ���� �ذ�
        if (absX > absY) // �¿�� �̵��� ��
        {
            if (!isMovingSide)
            {
                animator.Play("Gaper-Move-side");
                isMovingSide = true;
            }
            spriteRenderer.flipX = direction.x < 0; // �¿� ���� ��ȯ
        }
        else // ���Ϸ� �̵��� ��
        {
            if (isMovingSide)
            {
                animator.Play("Gaper-Move-idle");
                isMovingSide = false;
            }

            // ���� �̵� �� �¿� ������ idle �ִϸ��̼�
            if (direction.y > 0)
            {
                spriteRenderer.flipX = true; // ���� �̵� �� flipX ����
            }
            else
            {
                spriteRenderer.flipX = false; // �Ʒ��� �̵� �� �⺻ ����
            }
        }
    }
}

