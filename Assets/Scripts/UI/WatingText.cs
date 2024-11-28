using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class WatingText : MonoBehaviour
{
      public float spaceTime = 1;

      private RectTransform rectTransform;
      private Vector3 originalRotation;
      [Tooltip("Z-Axis")] public float rotateAmount;

      private void Awake()
      {
            rectTransform = GetComponent<RectTransform>();
            originalRotation = rectTransform.eulerAngles; // �ʱ� ȸ���� ����
      }

      private void Start()
      {
            StartCoroutine(TextAnimation());
      }

      private IEnumerator TextAnimation()
      {
            WaitForSeconds waitTime = new WaitForSeconds(spaceTime);
            while (true) {
                  if (NetworkManager.Instance.canStartGame) break;

                  yield return waitTime;
                  // Z�� ȸ��
                  rectTransform.eulerAngles = new Vector3(
                      originalRotation.x,
                      originalRotation.y,
                      originalRotation.z + rotateAmount
                  );
                  yield return waitTime;
                  // ���� ȸ�������� ����
                  rectTransform.eulerAngles = originalRotation;
            }
      }
}
