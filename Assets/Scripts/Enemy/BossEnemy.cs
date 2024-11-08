using System.Collections;
using UnityEngine;

public class RandomBossEnemy : MonoBehaviour
{
    public float normalSpeed = 2f; // �⺻ �̵� �ӵ�
    public float fastSpeed = 5f; // ������ ������ ���� �ӵ�
    public float moveRangeX = 5f; // X�� �̵� ����
    public float moveRangeY = 5f; // Y�� �̵� ����

    private Rigidbody2D rigid;
    private Vector2 initialPosition; // ������ ó�� ��ġ
    private Vector2 randomDirection; // �����ϰ� ������ ����
    private float currentSpeed; // ���� �ӵ�
    private bool isResting = false; // ������ ���� �������� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        initialPosition = rigid.position; // ������ ó�� ��ġ ����
        currentSpeed = normalSpeed; // �ʱ� �ӵ��� �⺻ �ӵ��� ����
        StartCoroutine(ChangeRandomDirection()); // �ֱ������� �̵� ���� ����
        StartCoroutine(RandomPatternCycle()); // ������ ���� �ֱ������� ����
    }

    void FixedUpdate()
    {
        if (isResting) return; // ���� ���¶�� �������� ����
        MoveRandomly(); // �Ϲ����� ���� �̵�
    }

    // ���� �ֱ������� ���� (�ӵ� ��ȭ �Ǵ� ��� ���߱�)
    IEnumerator RandomPatternCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f)); // 3��~6�� �������� ���� ��ȭ
            int pattern = Random.Range(0, 2); // 2���� ���� �� �ϳ� ����

            if (pattern == 0)
            {
                StartCoroutine(SpeedChangePattern());
            }
            else if (pattern == 1)
            {
                StartCoroutine(RestAndMove());
            }
        }
    }

    // ���� �ð� ���� ������ �̵��ߴٰ� �ٽ� ���� �ӵ��� ���ƿ��� ����
    IEnumerator SpeedChangePattern()
    {
        currentSpeed = fastSpeed; // ������ �̵�
        yield return new WaitForSeconds(2f); // 2�� ���� ������ �̵�
        currentSpeed = normalSpeed; // �ٽ� ���� �ӵ��� ���ƿ�
    }

    // ��� ����ٰ� �ٽ� �����̴� ����
    IEnumerator RestAndMove()
    {
        isResting = true; // ���� ����
        yield return new WaitForSeconds(1.5f); // 1.5�� ���� ����
        isResting = false; // �ٽ� �̵�
    }

    // ���� �ð����� ������ �������� �̵�
    IEnumerator ChangeRandomDirection()
    {
        while (true)
        {
            if (!isResting) // ���� ���°� �ƴϸ� ������ �ٲ۴�
            {
                randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            }
            yield return new WaitForSeconds(2f); // 2�ʸ��� ���� ����
        }
    }

    // �����ϰ� ������ �������� �̵�
    void MoveRandomly()
    {
        Vector2 nextVec = randomDirection * currentSpeed * Time.fixedDeltaTime;
        Vector2 newPosition = rigid.position + nextVec;

        // �̵� ������ ����� �ʵ��� ����
        if (newPosition.x >= initialPosition.x + moveRangeX || newPosition.x <= initialPosition.x - moveRangeX)
        {
            nextVec.x = 0; // ������ ����� X�� ���� ����
        }

        if (newPosition.y >= initialPosition.y + moveRangeY || newPosition.y <= initialPosition.y - moveRangeY)
        {
            nextVec.y = 0; // ������ ����� Y�� ���� ����
        }

        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        // �ð��� ȿ��: ������ �����̴� ���⿡ ���� ��������Ʈ�� ������
        if (randomDirection.x != 0)
        {
            GetComponent<SpriteRenderer>().flipX = randomDirection.x < 0;
        }
    }
}
