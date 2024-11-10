using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PooterStates
{
    public abstract class PooterState : BaseState<Pooter>
    {
        protected Rigidbody2D rigid;
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;

        public PooterState(Pooter _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            rigid = monster.GetComponent<Rigidbody2D>();
            animator = monster.GetComponent<Animator>();
            spriteRenderer = monster.GetComponent<SpriteRenderer>();
        }

        public override void OnStateUpdate()
        {
            if (!rigid || !animator || !spriteRenderer) {
                rigid = monster.GetComponent<Rigidbody2D>();
                animator = monster.GetComponent<Animator>();
                spriteRenderer = monster.GetComponent<SpriteRenderer>();
            }
        }
    }

    public class IdleState : PooterState
    {
        public IdleState(Pooter _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
        }

        public override void OnStateUpdate()
        {
            base.OnStateUpdate();
        }

        public override void OnStateExit()
        {
            animator.SetTrigger("Awake");
        }
    }

    public class MoveState : PooterState
    {
        public MoveState(Pooter _monster) : base(_monster) { }

        private bool isStateExit = false;

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            // (�ִϸ��̼� �̸�, ���� Ȱ��ȭ ���� ���̾�, 0~1������ 0.5�� ����)
            animator.Play("AM_PooterMove", -1, UnityEngine.Random.Range(0f, 1f));

            SetInputVec(1);
            SetInputVec();
        }

        public override void OnStateUpdate()
        {
            MoveMonster();
        }

        public override void OnStateExit()
        {
            isStateExit = true;
        }

        private async void SetInputVec(int _time = 2)
        {
            if (_time < 1) {
                Debug.LogError($"{monster.name}: SetInputVec ȣ�� �ð��� �߸��Ǿ����ϴ�. (0 ����)");
                return;
            }

            for (int i = 0; i < _time; ++i) {
                if (isStateExit) return;
                await Task.Delay(1000); // 1 second
            }

            SetInputVec();
            SetInputVec(_time);
        }
        
        private void SetInputVec()
        {
            monster.inputVec = new Vector2(UnityEngine.Random.Range(-1, 2), UnityEngine.Random.Range(-1, 2));

            SetSpriteDirection();
        }

        private void SetSpriteDirection()
        {
            if (monster.inputVec.x > 0) {
                spriteRenderer.flipX = false;
            }
            else if (monster.inputVec.x < 0) {
                spriteRenderer.flipX = true;
            }
        }

        private void MoveMonster()
        {
            rigid.AddForce(monster.inputVec.normalized * monster.stat.moveForce, ForceMode2D.Force);
            if (rigid.velocity.magnitude > monster.stat.maxVelocity) {
                rigid.velocity = rigid.velocity.normalized * monster.stat.maxVelocity;
            }
        }
    }

    public class AttackState : PooterState
    {
        public AttackState(Pooter _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            animator.SetTrigger("Attack");

            if (monster.playerHit is RaycastHit2D playerHit) {
                float direction = playerHit.point.x - rigid.position.x;
                if (Mathf.Sign(direction) > 0) spriteRenderer.flipX = false;
                else spriteRenderer.flipX = true;

                AttackPlayer();
            }
            else {
                Debug.LogWarning($"{monster.name}: AttackState���� monster.playerHit�� ã�� ���߽��ϴ�.");
                // (�ִϸ��̼� �̸�, ���� Ȱ��ȭ ���� ���̾�, 0~1������ 0.5�� ����)
                animator.Play("AM_PooterAttack", -1, 0.9f); // AM_PooterAttack ���������� ���¸� �ٲٴ� �̺�Ʈ ����
            }
        }

        public override void OnStateUpdate()
        {
            // 
        }

        public override void OnStateExit()
        {
            // 
        }

        private void AttackPlayer()
        {
            //RaycastHit2D playerHit = monster.OnSenseForward(0.45f, "Player");
            //if (playerHit && !monster.isAttack) {
            //    monster.isAttack = true;

            //    if (playerHit.transform.TryGetComponent<IsaacBody>(out var player)) {
            //        if (!player.IsHurt) {
            //            player.health -= monster.stat.attackDamage;
            //            player.IsHurt = true;
            //        }
            //    }
            //}
        }
    }

    public class DeadState : PooterState
    {
        public DeadState(Pooter _monster) : base(_monster) { }

        public override void OnStateEnter()
        {
            // 
        }

        public override void OnStateUpdate()
        {
            // 
        }

        public override void OnStateExit()
        {
            // 
        }
    }
}
