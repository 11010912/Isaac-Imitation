using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float moveRangeX; // ���� �̵��� �� �ִ� X ���� ����
    public float moveRangeY; // ���� �̵��� �� �ִ� Y ���� ����

    private Rigidbody2D rigid;
    private Vector2 initialPosition; // ���� ó�� ��ġ�� ���� (�̵� ���� ��꿡 ���)
    private Vector2 randomDirection; // �����ϰ� ������ ����
    private float changeDirectionTime = 2.0f; // ���� �̵� ������ ������ �ֱ�

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        initialPosition = rigid.position; // ���� ó�� ��ġ ����
        StartCoroutine(ChangeRandomDirection()); // �ֱ������� �̵� ���� ����
    }

    void FixedUpdate()
    {
        MoveRandomly(); // ������ �������� �̵�
    }

    IEnumerator ChangeRandomDirection()
    {
        while (true)
        {
            // 2�ʸ��� ������ ������ ����
            randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            yield return new WaitForSeconds(changeDirectionTime);
        }
    }

    void MoveRandomly()
    {
        Vector2 nextVec = randomDirection * speed * Time.fixedDeltaTime;
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
        if (randomDirection.x != 0)
        {
            // �¿� ���⿡ ���� ��������Ʈ�� ������
            GetComponent<SpriteRenderer>().flipX = randomDirection.x < 0;
        }
    }
}