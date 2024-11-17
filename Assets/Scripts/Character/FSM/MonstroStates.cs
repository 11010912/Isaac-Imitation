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
            protected Collider2D collider;
            protected Animator animator;
            protected SpriteRenderer spriteRenderer;

            protected Transform shadow;

            public MonstroState(Monstro _monster) : base(_monster) { }

            public override void OnStateEnter()
            {
                  rigid = monster.GetComponent<Rigidbody2D>();
                  collider = monster.GetComponent<Collider2D>();
                  animator = monster.GetComponent<Animator>();
                  spriteRenderer = monster.GetComponent<SpriteRenderer>();

                  shadow = monster.transform.GetChild(0);
            }
            
            public override void OnStateUpdate()
            {
                  if (!rigid || !animator || !spriteRenderer) {
                        rigid = monster.GetComponent<Rigidbody2D>();
                        animator = monster.GetComponent<Animator>();
                        spriteRenderer = monster.GetComponent<SpriteRenderer>();
                  }
            }

            protected virtual void OnCollisionEnter2D()
            {
                  if (monster.player.IsHurt) return;

                  if (monster.collisionRectangle == default) monster.collisionRectangle = new Vector2(1.75f, 1.25f);
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

            private SpriteRenderer playerRenderer;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  monster.player = GetPlayerObject();
                  playerRenderer = monster.player.GetComponent<SpriteRenderer>();
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

            private SpriteRenderer playerRenderer;

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

                  shadowOffset = rigid.position - (Vector2)shadow.position;
                  shadow.parent = null;

                  // �ִϸ��̼��� ���� ��������
                  animationLength = animator.runtimeAnimatorController.animationClips
                      .FirstOrDefault(clip => clip.name == "AM_MonstroSmallJump")?.length ?? 0f;
                  // animator.SetTrigger("SmallJump");

                  playerRenderer = monster.player.GetComponent<SpriteRenderer>();
            }

            public override void OnStateUpdate()
            {
                  SetBeforeNextJump();
                  MoveShadow();
                  JumpToShadow();

                  if (monster.isOnLand) {
                        OnCollisionEnter2D();
                        if (IsLayerExcluded(LayerMask.NameToLayer("Player"))) {
                              RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Player"));
                              RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));
                        }
                  }

                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);
            }

            public override void OnStateExit()
            {
                  shadow.parent = monster.transform;
                  shadow.localPosition = shadowOffset;
                  monster.isOnLand = false;
                  RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Player"));
                  RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));

                  monster.isSmallJump = false;
            }

            private void SetBeforeNextJump()
            {
                  elapsedAnimationTime += Time.deltaTime;
                  if (curjumpCount > 0) {
                        // �ִϸ��̼��� ������ �ٽ� ���
                        if (elapsedAnimationTime >= animationLength || curjumpCount == maxJumpCount) {
                              if (curjumpCount == maxJumpCount) animator.SetTrigger("SmallJump");
                              else animator.Play("AM_MonstroSmallJump", 0, 0f); // 0�����Ӻ��� ���
                              elapsedAnimationTime = 0f;

                              // ���� ��ġ�� �̵��ϱ� ���� ����
                              nextPosition = GetNextPosition(2f);

                              curjumpCount--;

                              monster.isOnLand = false;
                              AddExcludeLayerToCollider(LayerMask.NameToLayer("Player"));
                              AddExcludeLayerToCollider(LayerMask.NameToLayer("Tear"));
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
                        rigid.position = Vector2.Lerp(rigid.position, nextPosition + Vector2.up * 0.44f, 
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

            private SpriteRenderer playerRenderer;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();

                  jumpUpPosition = rigid.position + Vector2.up * 15;
                  shadowOffset = rigid.position - (Vector2)shadow.position;
                  shadow.parent = null;

                  animator.SetTrigger("BigJumpUp");

                  playerRenderer = monster.player.GetComponent<SpriteRenderer>();
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
                              if (IsLayerExcluded(LayerMask.NameToLayer("Player"))) {
                                    RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Player"));
                                    RemoveExcludeLayerFromCollider(LayerMask.NameToLayer("Tear"));
                              }
                        }
                  }

                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);
            }

            public override void OnStateExit()
            {
                  shadow.parent = monster.transform;
                  shadow.localPosition = shadowOffset;
                  monster.isOnLand = false;
                  AddExcludeLayerToCollider(LayerMask.NameToLayer("Player"));
                  AddExcludeLayerToCollider(LayerMask.NameToLayer("Tear"));

                  monster.isBigJump = false;
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
                  rigid.position = Vector2.MoveTowards(rigid.position, (Vector2)shadow.position + shadowOffset,
                        jumpDownSpeed * Time.deltaTime);
            }

            // ���� �л� �߰�
      }

      public class TearSprayState : MonstroState
      {
            public TearSprayState(Monstro _monster) : base(_monster) { }

            private SpriteRenderer playerRenderer;

            public override void OnStateEnter()
            {
                  base.OnStateEnter();
                  animator.SetTrigger("TearSpray");

                  playerRenderer = monster.player.GetComponent<SpriteRenderer>();
            }

            public override void OnStateUpdate()
            {
                  monster.sortRendererBy.SortBy(spriteRenderer, playerRenderer, false);

                  OnCollisionEnter2D();
            }

            public override void OnStateExit()
            {
                  monster.isTearSpray = false;
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
