using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace MonstroStates
{
      public abstract class MonstroState : BaseState<Monstro>
      {
            protected Rigidbody2D rigid;
            protected Animator animator;
            protected SpriteRenderer spriteRenderer;
            protected SpriteRenderer playerRenderer;

            protected Transform shadow;
            protected Collider2D collider;

            public MonstroState(Monstro _monster) : base(_monster) { }

            public override void OnStateEnter()
            {
                  rigid = monster.GetComponent<Rigidbody2D>();
                  animator = monster.GetComponent<Animator>();
                  spriteRenderer = monster.GetComponent<SpriteRenderer>();
                  playerRenderer = monster.player.GetComponent<SpriteRenderer>();

                  shadow = monster.transform.GetChild(0);
                  collider = shadow.GetComponent<Collider2D>();
            }
            
            public override void OnStateUpdate()
            {
                  if (!rigid || !animator || !spriteRenderer || !shadow || !collider) {
                        rigid = monster.GetComponent<Rigidbody2D>();
                        animator = monster.GetComponent<Animator>();
                        spriteRenderer = monster.GetComponent<SpriteRenderer>();

                        shadow = monster.transform.GetChild(0);
                        collider = shadow.GetComponent<Collider2D>();
                  }
            }

            protected virtual void OnCollisionEnter2D()
            {
                  if (monster.player.IsHurt) return;

                  if (monster.collisionRectangle == default) monster.collisionRectangle = new Vector2(2.2f, 0.75f);
                  if (Physics2D.BoxCast(rigid.position, monster.collisionRectangle, 0, Vector2.zero, 0, 
                        LayerMask.GetMask("Player"))) {
                        // Debug.Log("Player is on Monster collision!");
                        monster.player.health -= monster.stat.attackDamage;
                        monster.player.IsHurt = true;
                  }
            }

            // Exclude Layers�� ���̾ �����ϴ��� Ȯ���ϴ� �Լ�
            protected virtual bool IsLayerExcluded(int layer)
            {
                  // ��Ʈ �������� ���̾ ���ԵǾ� �ִ��� Ȯ��
                  return (collider.excludeLayers & (1 << layer)) != 0;
            }

            // Exclude Layers�� ���̾ �߰��ϴ� �Լ�
            protected virtual void AddExcludeLayerToCollider(LayerMask layerToAdd)
            {
                  // ���� excludeLayers�� layerToAdd�� �߰�
                  collider.excludeLayers |= layerToAdd;
            }

            // Exclude Layers�� ���̾ �����ϴ� �Լ�
            protected virtual void RemoveExcludeLayerFromCollider(LayerMask layerToRemove)
            {
                  // ���� excludeLayers���� layerToRemove�� ����
                  collider.excludeLayers &= ~layerToRemove;
            }
      }

      public class IdleState : MonstroState
      {
            public IdleState(Monstro _monster) : base(_monster) { }

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  monster.player = GetPlayerObject();
            }

            public override void OnStateUpdate()
            {
                  base.OnStateUpdate();

                  if (monster.player == null) monster.player = GetPlayerObject();
                  else if (playerRenderer == null) playerRenderer = monster.player.GetComponent<SpriteRenderer>();
                  else monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);

                  OnCollisionEnter2D();
            }

            public override void OnStateExit()
            {
                  animator.SetTrigger("Awake");
            }

            private IsaacBody GetPlayerObject()
            {
                  if (monster.playerSearchBox == default) monster.playerSearchBox = Vector2.one * 40;
                  if (Physics2D.BoxCast(rigid.position, monster.playerSearchBox, 0, Vector2.zero, 0,
                      LayerMask.GetMask("Player")) is RaycastHit2D _player) {
                        return _player.transform.GetComponent<IsaacBody>();
                  }
                  else {
                        return null;
                  }
            }
      }

      public class SmallJumpState : MonstroState
      {
            public SmallJumpState(Monstro _monster) : base(_monster) { }

            private int curjumpCount;
            private int maxJumpCount;

            private Vector2 shadowOffset;
            private Vector2 nextPosition;

            private float animationLength;
            private float elapsedAnimationTime;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  // ���� Ƚ�� �ʱ� ����
                  switch (UnityEngine.Random.Range(0, 5)) {
                        case 0: maxJumpCount = 1; break;
                        case 1: case 2: maxJumpCount = 3; break;
                        case 3: case 4: maxJumpCount = 5; break;
                  }
                  curjumpCount = maxJumpCount;

                  shadowOffset = shadow.localPosition;
                  shadow.parent = null;

                  // �ִϸ��̼��� ���� ��������
                  animationLength = animator.runtimeAnimatorController.animationClips
                      .FirstOrDefault(clip => clip.name == "AM_MonstroSmallJump")?.length ?? 0f;

                  animator.SetBool("SmallJump", true);
            }

            public override void OnStateUpdate()
            {
                  SetBeforeNextJump();
                  MoveShadow();
                  JumpToShadow();

                  if (monster.isOnLand) {
                        OnCollisionEnter2D();
                        if (IsLayerExcluded(LayerMask.NameToLayer("Tear"))) {
                              RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));
                        }
                  }

                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);
            }

            public override void OnStateExit()
            {
                  monster.isOnLand = false;

                  shadow.parent = monster.transform;
                  shadow.localPosition = shadowOffset;
                  RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));

                  animator.SetBool("SmallJump", false);
            }

            private void SetBeforeNextJump()
            {
                  elapsedAnimationTime += Time.deltaTime;
                  // �ִϸ��̼��� ������ �ٽ� ���
                  if (elapsedAnimationTime >= animationLength || curjumpCount == maxJumpCount) {
                        if (curjumpCount > 0) {
                              animator.Play("AM_MonstroSmallJump", 0, 0f); // 0�����Ӻ��� ���
                              elapsedAnimationTime = 0f;

                              // ���� ��ġ�� �̵��ϱ� ���� ����
                              nextPosition = GetNextPosition(2f);

                              curjumpCount--;

                              monster.isOnLand = false;
                              AddExcludeLayerToCollider(LayerMask.NameToLayer("Tear"));
                        }
                        else {
                              // ���� Ƚ�� ��� �����Ǹ� ���� ����
                              monster.isSmallJump = false;
                        }
                  }
            }

            private Vector2 GetNextPosition(float distance)
            {
                  Vector3 nextDirection = monster.player.transform.position - shadow.position;
                  Vector3 nextPosition = nextDirection.normalized * distance;
                  return shadow.position + nextPosition;
            }

            private void MoveShadow()
            {
                  // �׸��� ��� �̵�
                  shadow.position = Vector2.Lerp(shadow.position, nextPosition, elapsedAnimationTime / animationLength);
            }

            private void JumpToShadow()
            {
                  // ���� ��� �̵�
                  if (elapsedAnimationTime < animationLength / 2) {
                        rigid.position = Vector2.Lerp(rigid.position, nextPosition + Vector2.up * 3f, 
                              elapsedAnimationTime / (animationLength / 2));
                  }
                  else {
                        rigid.position = Vector2.Lerp(rigid.position, nextPosition + Vector2.up * -shadowOffset, 
                              elapsedAnimationTime / animationLength);
                  }
            }
      }

      public class BigJumpState : MonstroState
      {
            public BigJumpState(Monstro _monster) : base(_monster) { }

            private Vector2 playerLatePosition = default;

            private Vector2 jumpUpPosition;
            private Vector2 shadowOffset;
            private float jumpDownSpeed = 25f;

            private float jumpDownDelay = 3f;
            private float curJumpDownDelayTime;

            private float animationLength;
            private float elapsedAnimationTime;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  jumpUpPosition = rigid.position + Vector2.up * 15;
                  shadowOffset = shadow.localPosition;
                  shadow.parent = null;

                  animationLength = animator.runtimeAnimatorController.animationClips
                      .FirstOrDefault(clip => clip.name == "AM_MonstroBigJumpDown")?.length ?? 0f;

                  animator.SetTrigger("BigJumpUp");
            }

            public override void OnStateUpdate()
            {
                  if (monster.isJumpUp) {
                        // ���� ȭ�� ������ ����
                        rigid.position = Vector2.Lerp(rigid.position, jumpUpPosition, 0.01f);

                        curJumpDownDelayTime += Time.deltaTime;

                        MoveShadow();

                        // ���� Ʈ����
                        LandTrigger();
                  }
                  else {
                        LandOnShadow();
                        if (monster.isOnLand) {
                              OnCollisionEnter2D();
                              if (IsLayerExcluded(LayerMask.NameToLayer("Tear"))) {
                                    RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));
                              }

                              elapsedAnimationTime += Time.deltaTime;
                              // AM_MonstroBigJumpDown �ִϸ��̼��� ������ ���� ����
                              if (elapsedAnimationTime >= animationLength) {
                                    monster.isSmallJump = false;
                              }
                        }
                  }

                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);
            }

            public override void OnStateExit()
            {
                  monster.isOnLand = false;

                  shadow.parent = monster.transform;
                  shadow.localPosition = shadowOffset;
                  AddExcludeLayerToCollider(LayerMask.NameToLayer("Tear"));

                  animator.SetTrigger("BigJumpUp");
            }

            private void MoveShadow()
            {
                  Vector2 nextPosition = monster.player.transform.position;

                  // �÷��̾��� ���� ��ġ ����
                  if (curJumpDownDelayTime > jumpDownDelay / 2 && playerLatePosition == default) {
                        playerLatePosition = nextPosition;
                  }

                  // �׸��� ���� �̵�
                  float t = Mathf.Clamp01(curJumpDownDelayTime / jumpDownDelay);
                  shadow.position =
                        Vector2.Lerp(shadow.position, playerLatePosition == default ? nextPosition : playerLatePosition, t);
            }

            private void LandTrigger()
            {
                  if (curJumpDownDelayTime > jumpDownDelay) {
                        rigid.position = new Vector2(shadow.position.x, rigid.position.y); // �׸��ڿ� X���� ��ġ
                        monster.isJumpUp = false;
                        animator.SetTrigger("BigJumpDown");
                  }
            }

            private void LandOnShadow()
            {
                  // ���� �׸��� ���� ����
                  rigid.position = Vector2.MoveTowards(rigid.position, (Vector2)shadow.position - shadowOffset,
                        jumpDownSpeed * Time.deltaTime);
            }

            // ���� �л� �߰�
      }

      public class TearSprayState : MonstroState
      {
            public TearSprayState(Monstro _monster) : base(_monster) { }

            private const TearFactory.Tears tearType = TearFactory.Tears.Boss;

            private Vector2 directionVec = Vector2.zero;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  animator.SetBool("TearSpray", true);
                  // Debug.Log("TearSpray");
            }

            public override void OnStateUpdate()
            {
                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);

                  // animator.Play("AM_MonstroTearSpary", 0, 0f); // 0�����Ӻ��� ���
                  // GameManager.Instance.monsterTearFactory.GetTear(tearType, true);

                  OnCollisionEnter2D();
            }

            public override void OnStateExit()
            {
                  // monster.isTearSpray = false;

                  animator.SetBool("TearSpray", false);
            }
      }

      public class DeadState : MonstroState
      {
            public DeadState(Monstro _monster) : base(_monster) { }

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
