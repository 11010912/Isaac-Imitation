using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class IsaacTear : Tear
{
      private void OnTriggerEnter2D(Collider2D collision)
      {
            if (collision.CompareTag("Wall")) {
                  DisableTear();
            }
            else if (collision.CompareTag("Monster")) {
                  DisableTear();

                  if (TryGetMonsterFields(collision, out MonoBehaviour script, out FieldInfo statField,
                      out PropertyInfo isHurtProperty, out FieldInfo monsterTypeField)) {
                        if (statField.GetValue(script) is MonsterStat monsterStat &&
                            monsterTypeField.GetValue(script) is MonsterType monsterType) {
                              monsterStat.health -= tearDamage;
                              isHurtProperty.SetValue(script, true);
                              ApplyKnockToMonster(monsterType, script.GetComponent<Rigidbody2D>());
                        }
                  }
                  else {
                        Debug.LogWarning("stat 필드 또는 IsHurt 프로퍼티를 찾을 수 없습니다.");
                  }
            }
      }

      private bool TryGetMonsterFields(Collider2D collision, out MonoBehaviour script, out FieldInfo statField,
          out PropertyInfo isHurtProperty, out FieldInfo monsterTypeField)
      {
            // 초기화
            script = null;
            statField = null;
            isHurtProperty = null;
            monsterTypeField = null;

            MonoBehaviour[] scripts = collision.GetComponents<MonoBehaviour>();
            if (scripts.Length <= 1) scripts = collision.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour s in scripts) {
                  Type baseType = s.GetType()?.BaseType; // 부모: Monster
                  if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(Monster<>)) {
                        // stat 필드와 IsHurt 프로퍼티 찾기
                        statField = baseType.GetField("stat", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                        isHurtProperty = baseType.GetProperty("IsHurt", BindingFlags.Instance | BindingFlags.Public);
                        // MonsterType 열거형 필드 찾기
                        monsterTypeField = baseType.GetField("monsterType", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

                        if (statField != null && isHurtProperty != null && monsterTypeField != null) {
                              script = s;
                              return true;
                        }
                        break;
                  }
            }
            return false;
      }

      private void ApplyKnockToMonster(MonsterType monsterType, Rigidbody2D monsterRigid)
      {
            float adjustedKnockPower = knockPower;
            switch (monsterType) {
                  case MonsterType.Gaper:
                        break;
                  case MonsterType.Pooter:
                        adjustedKnockPower *= 2;
                        break;
                  default:
                        // Knockback not applied
                        return;
            }

            monsterRigid.velocity = Vector2.zero;
            monsterRigid.AddForce((monsterRigid.position - rigid.position).normalized * adjustedKnockPower, ForceMode2D.Impulse);
      }
}
