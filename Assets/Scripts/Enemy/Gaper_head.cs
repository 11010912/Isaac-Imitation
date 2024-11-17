using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gaper_Head : MonoBehaviour
{
    public Transform player; // �÷��̾� ��ġ�� �����ϱ� ���� �Ҵ�
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        FlipTowardsPlayer();
    }

    // �÷��̾� ��ġ�� ���� Gaper_head�� �¿� ���� ó��
    private void FlipTowardsPlayer()
    {
        if (player != null)
        {
            // �÷��̾ Gaper_head���� �����ʿ� ���� ���
            if (player.position.x > transform.position.x)
            {
                spriteRenderer.flipX = true; // �������� �ٶ󺸰� �¿� ����
            }
            else
            {
                spriteRenderer.flipX = false; // ������ �ٶ󺸰� ���� ���� ����
            }
        }
    }
}
