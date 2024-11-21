using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
      [Tooltip("The current collider is the target's script name to ignore the collision (2D)")]
      [SerializeField] protected List<string> targetNames; // �浹�� ������ ��� �̸�


      protected virtual void Start()
      {
            // ���� ������Ʈ�� �ִ� �� ã��
            if (transform.parent.parent.GetComponent<AddRoom>() is AddRoom room) {
                  // ���� �濡�� �̸��� Monsters�� ������Ʈ ã��
                  Transform monsters = null;
                  foreach (Transform child in room.transform) {
                        if (child.name == "Monsters") {
                              monsters = child;
                              break;
                        }
                  }

                  if (monsters == null) {
                        Debug.LogWarning("No object named 'Monsters' found in the room.");
                        return;
                  }

                  // Monsters ������Ʈ�� ��Ȱ�� ���� ��� �ڽ� ��
                  Collider2D thisCollider = GetComponent<Collider2D>();
                  foreach (Transform monster in monsters.GetComponentsInChildren<Transform>(true)) {
                        // targetName�� ��ġ�ϴ� ��ũ��Ʈ�� ���� ������Ʈ ã��
                        if (monster.GetComponent<MonoBehaviour>() is MonoBehaviour targetScript) {
                              if (targetNames.Contains(targetScript.GetType().Name)) {
                                    // targetNames�� ��ġ�ϴ� �̸��� ������ ���� ������Ʈ�� �浹 ���� ó��
                                    Physics2D.IgnoreCollision(thisCollider, targetScript.GetComponent<Collider2D>(), true);
                              }
                        }
                  }
            }
      }
}
