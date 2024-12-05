using System.Collections.Generic;
using UnityEngine;

namespace AttackFlyStates
{
    public abstract class AttackFlyState : BaseState<AttackFly>
    {
        protected Rigidbody2D rigid;
        protected Animator animator;

        public AttackFlyState(AttackFly _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            rigid = monster.GetComponent<Rigidbody2D>();
            animator = monster.GetComponent<Animator>();
        }

        public override void OnStateUpdate()
        {
            if (!rigid)
            {
                rigid = monster.GetComponent<Rigidbody2D>();
            }
        }

        public override void OnStateExit() { }
    }

    public class IdleState : AttackFlyState
    {
        public IdleState(AttackFly _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            rigid.velocity = Vector2.zero; // ���� ����
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
        }
    }

    public class MoveState : AttackFlyState
    {
        private Transform player;
        private float moveForce;
        private float maxVelocity;
        private float collisionRadius = 0.5f; // �浹 ���� �ݰ�
        private float moveSpeedModifier = 1.0f; // �̵� �ӵ� ���� ���
        private bool isPlayerHit = false; // �÷��̾� �ǰ� ����
        private float speedVariationTime = 0.35f; // �ӵ� ��ȭ ���� (��)
        private float speedVariationTimer;

        public MoveState(AttackFly _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            moveForce = monster.stat.moveForce;
            maxVelocity = monster.stat.maxVelocity;
            speedVariationTimer = speedVariationTime;

            // �÷��̾� ã��
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();

            if (player != null)
            {
                // �ӵ� ��ȭ ����
                UpdateSpeedVariation();

                // �÷��̾� ����
                Vector2 direction = ((Vector2)player.position - (Vector2)monster.transform.position).normalized;
                rigid.AddForce(direction * moveForce * moveSpeedModifier, ForceMode2D.Force);

                if (rigid.velocity.magnitude > maxVelocity)
                {
                    rigid.velocity = rigid.velocity.normalized * maxVelocity;
                }

                // �浹 ����
                CheckCollisionWithPlayer();
            }
            else
            {
                rigid.velocity = Vector2.zero;
            }
        }

        private void UpdateSpeedVariation()
        {
            speedVariationTimer -= Time.deltaTime;
            if (speedVariationTimer <= 0)
            {
                // �ӵ� ���� ���� ũ�� ����
                moveSpeedModifier = UnityEngine.Random.Range(0.7f, 5.0f); // �ӵ� ��ȭ ����
                speedVariationTimer = speedVariationTime; // Ÿ�̸� �ʱ�ȭ
            }
        }

        private void CheckCollisionWithPlayer()
        {
            if (isPlayerHit) return; // �̹� ó�� ���̸� �ߺ� ó�� ����

            Collider2D hit = Physics2D.OverlapCircle(monster.transform.position, collisionRadius, LayerMask.GetMask("Player"));
            if (hit != null)
            {
                IsaacBody playerBody = hit.GetComponent<IsaacBody>();
                if (playerBody != null && !playerBody.IsHurt) // �ǰ� ���� Ȯ��
                {
                    isPlayerHit = true; // �ǰ� ó�� ����
                    playerBody.IsHurt = true; // �÷��̾� ���� ���� Ȱ��ȭ
                    playerBody.health -= monster.stat.attackDamage; // �÷��̾� ü�� ����

                    Debug.Log($"AttackFly hit the player! Remaining health: {playerBody.health}");

                    // ���� ���� ���� �ڷ�ƾ ȣ��
                    monster.StartCoroutine(ResetPlayerHitCooldown(playerBody));
                }
            }
        }

        private System.Collections.IEnumerator ResetPlayerHitCooldown(IsaacBody playerBody)
        {
            yield return new WaitForSeconds(1.0f); // 1�� ���� ����
            playerBody.IsHurt = false; // ���� ����
            isPlayerHit = false; // �ǰ� ���� ����
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            rigid.velocity = Vector2.zero;
            isPlayerHit = false; // ���� �ʱ�ȭ
        }
    }




    public class DeadState : AttackFlyState
    {
        public DeadState(AttackFly _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            // ��� ó�� ���� �߰�
            monster.gameObject.SetActive(false); // �ӽ÷� ��Ȱ��ȭ
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
        }
    }
}
