using UnityEngine;

public class MonstroTear : Tear
{
      private void OnTriggerEnter2D(Collider2D collision)
      {
            if (collision.CompareTag("Wall") || collision.CompareTag("Obstacle")) {
                  DisableTear();
            }
            else if (collision.CompareTag("Player")) {
                  DisableTear();

                  if (collision.TryGetComponent<IsaacBody>(out var player)) {
                        if (player.IsHurt) return;
                        player.Health -= tearDamage;
                        player.IsHurt = true;
                        GameManager.Instance.uiManager.setKilledPlayer = "Monstro";
                  }
                  else {
                        Debug.LogWarning("Player(IsaacBody)�� ã�� �� �����ϴ�.");
                  }
            }
      }


      private float tearActiveTimeDefault;

      private void Start()
      {
            tearActiveTimeDefault = tearActiveTime;
            RandomizeTearSet();
      }

      protected override void OnEnable()
      {
            RandomizeTearSet();
            base.OnEnable();
      }

      private void RandomizeTearSet()
      {
            // ���� ũ��
            this.transform.localScale = Vector2.one * UnityEngine.Random.Range(0.5f, 1.25f);

            // ���� Ȱ�� �ð�
            tearActiveTime = UnityEngine.Random.Range(tearActiveTimeDefault, tearActiveTimeDefault + 3f);

            // �߷� ũ�� �� Ȱ�� �ð�
            gravityScale = 0.25f;
      }
}
