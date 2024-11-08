using System.Collections;
using UnityEngine;

public class MonstroAI : MonoBehaviour
{
    public float jumpCooldown = 2f;         // ���� ��Ÿ�� (�� ����)
    public float vomitCooldown = 3f;        // ���� ���� ��Ÿ�� (�� ����)
    public float diveCooldown = 5f;         // ���� ���� ��Ÿ�� (�� ����)
    public float jumpForce = 2f;            // ���� �� (������)
    public float highJumpForce = 4f;        // ���� ���� �� ���� ���� �� (������)
    public GameObject projectilePrefab;     // ���� ���ݿ� ����� źȯ ������
    public int projectileCount = 5;         // ���� ���� źȯ ����
    public float projectileSpeed = 1.5f;    // źȯ �ӵ� (������)
    public float jumpRange = 1f;            // ���� ���� �Ÿ� (������)
    public float diveImpactRadius = 1f;     // ���� ���� ���� �� ����� ���� (������)
    public float diveImpactForce = 5f;      // ����� �� (������)

    private bool isJumping = false;         // ���� �� ���� üũ
    private bool isDiving = false;          // ���� ���� �� ���� üũ
    private float nextJumpTime = 0f;        // ���� ���� �ð�
    private float nextVomitTime = 0f;       // ���� ���� ���� �ð�
    private float nextDiveTime = 0f;        // ���� ���� ���� �ð�
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nextJumpTime = Time.time + jumpCooldown;
        nextVomitTime = Time.time + vomitCooldown;
        nextDiveTime = Time.time + diveCooldown;
    }

    void Update()
    {
        if (Time.time >= nextJumpTime && !isDiving)
        {
            Jump();
            nextJumpTime = Time.time + jumpCooldown;
        }

        if (Time.time >= nextVomitTime && !isDiving)
        {
            VomitAttack();
            nextVomitTime = Time.time + vomitCooldown;
        }

        if (Time.time >= nextDiveTime && !isJumping)
        {
            StartCoroutine(DiveAttack());
            nextDiveTime = Time.time + diveCooldown;
        }
    }

    void Jump()
    {
        if (isJumping) return;
        isJumping = true;

        Vector2 jumpTarget = new Vector2(
            transform.position.x + Random.Range(-jumpRange, jumpRange),
            transform.position.y
        );

        Vector2 jumpDirection = (jumpTarget - (Vector2)transform.position).normalized;
        rb.AddForce(jumpDirection * jumpForce, ForceMode2D.Impulse);

        Invoke("EndJump", 0.5f);
    }

    void EndJump()
    {
        isJumping = false;
    }

    void VomitAttack()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float angle = i * (360f / projectileCount);
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = direction * projectileSpeed;
        }
    }

    IEnumerator DiveAttack()
    {
        isDiving = true;

        // ���� ���� (������ ���� �� ���)
        rb.AddForce(Vector2.up * highJumpForce, ForceMode2D.Impulse);

        // ���� �ð� ��ٸ� �� ����
        yield return new WaitForSeconds(1.2f);

        // ������ ��������
        rb.AddForce(Vector2.down * (highJumpForce * 1.5f), ForceMode2D.Impulse);

        yield return new WaitUntil(() => Mathf.Abs(rb.velocity.y) < 0.1f); // ������ ������ ���

        // ����� �߻�
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, diveImpactRadius);
        foreach (Collider2D hit in hitColliders)
        {
            Rigidbody2D hitRb = hit.GetComponent<Rigidbody2D>();
            if (hitRb != null)
            {
                Vector2 impactDirection = (hit.transform.position - transform.position).normalized;
                hitRb.AddForce(impactDirection * diveImpactForce, ForceMode2D.Impulse);
            }
        }

        // ���� ���� ����
        isDiving = false;
    }

    void OnDrawGizmosSelected()
    {
        // ���� ���� ����� ���� ǥ��
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, diveImpactRadius);
    }
}


