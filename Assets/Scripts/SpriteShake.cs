using System.Collections;
using UnityEngine;

public class SpriteShake : MonoBehaviour
{
      public float duration = 0.25f;   // ��鸲 ���� �ð�
      public float magnitude = 0.1f; // ��鸲 ����

      private Vector3 originalPosition; // ���� ��ġ
      private float elapsed = 0.0f;     // ��� �ð�

      public void StartShake()
      {
            originalPosition = transform.localPosition;
            elapsed = 0.0f;
            StartCoroutine(Shake());
      }

      private IEnumerator Shake()
      {
            float offsetX, offsetY;
            while (elapsed < duration) {
                  elapsed += Time.deltaTime;

                  // ������ ��ġ ����
                  offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                  offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                  transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0);

                  yield return null; // ���� �����ӱ��� ���
            }

            // ��鸲 ���� �� ���� ��ġ�� ����
            transform.localPosition = originalPosition;
      }
}
