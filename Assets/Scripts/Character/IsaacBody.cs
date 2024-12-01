using ItemSpace;
using Photon.Pun;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// Photon applied complete
public class IsaacBody : MonoBehaviour, IPunObservable
{
      private PhotonView photonView;


      private IsaacHead head;

      private Rigidbody2D rigid;
      private Animator animator;
      private SpriteRenderer spriteRenderer;

      private FlashEffect flashEffect;

      [HideInInspector] public Vector2 inputVec;
      public float moveForce = 20; // +5
      public float maxVelocity = 5; // moveForce/5 + 1
      [HideInInspector] public float curMoveForce;
      [HideInInspector] public float curMaxVelocity;

      // for head (photonview)
      [HideInInspector] public Vector2 bodyVelocity;

      #region Health
      [SerializeField] private int health;
      public int Health
      {
            get => health;
            set {
                  if (health > 0) {
                        if (value > health) {
                              if (value > maxHealth) health = maxHealth;
                              else health = value;
                        }
                        else {
                              int damage = health - value;
                              if (soulHealth > 0) soulHealth -= damage;
                              else health = value;

                              if (soulHealth < 0) {
                                    health += soulHealth;
                                    soulHealth = 0;
                              }
                        }
                  }
                  else health = value;

                  //if (value != maxHealth) GameManager.Instance.uiManager.RefreshUI();
                  photonView.RPC(nameof(RPC_SetHealth), RpcTarget.AllBuffered, health, value);
            }
      }
      [PunRPC]
      private void RPC_SetHealth(int _health, int _value)
      {
            health = _health;
            if (_value != maxHealth) GameManager.Instance.uiManager.RefreshUI();
      }

      [SerializeField] private int maxHealth = 6;
      public int MaxHealth
      {
            get => maxHealth;
            set {
                  if (value > 24) maxHealth = 24;
                  else maxHealth = value;

                  //GameManager.Instance.uiManager.RefreshUI();
                  photonView.RPC(nameof(RPC_SetMaxHealth), RpcTarget.AllBuffered, maxHealth);
            }
      }
      [PunRPC]
      private void RPC_SetMaxHealth(int _maxHealth)
      {
            maxHealth = _maxHealth;
            GameManager.Instance.uiManager.RefreshUI();
      }

      [SerializeField] private int soulHealth = 0;
      public int SoulHealth
      {
            get => soulHealth;
            set {
                  if (soulHealth > 24) soulHealth = 24;
                  else soulHealth = value;

                  //GameManager.Instance.uiManager.RefreshUI();
                  photonView.RPC(nameof(RPC_SetSoulHealth), RpcTarget.AllBuffered, soulHealth);
            }
      }
      [PunRPC]
      private void RPC_SetSoulHealth(int _soulHealth)
      {
            soulHealth = _soulHealth;
            GameManager.Instance.uiManager.RefreshUI();
      }
      #endregion

      #region Item
      [Header("Item")]
      [SerializeField] private int bombCount = 3;
      public int BombCount
      {
            get => bombCount;
            set {
                  bombCount = value;

                  //GameManager.Instance.uiManager.RefreshUI();
                  photonView.RPC(nameof(RPC_SetBombCount), RpcTarget.AllBuffered, bombCount);
            }
      }
      [PunRPC]
      private void RPC_SetBombCount(int _bombCount)
      {
            bombCount = _bombCount;
            GameManager.Instance.uiManager.RefreshUI();
      }

      public float maxBombCool = 1;
      private float curBombCool = 0;
      #endregion


      private void Awake()
      {
            photonView = GetComponent<PhotonView>();

            head = GetComponentInChildren<IsaacHead>();

            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            flashEffect = GetComponent<FlashEffect>();
      }

      private void OnEnable()
      {
            // ������ Ŭ���̾�Ʈ�� ���� ������Ʈ�� �����ϰ� ���� ������
            if (PhotonNetwork.IsMasterClient && photonView.Owner != PhotonNetwork.LocalPlayer) {
                  photonView.RequestOwnership();
            }

            Health = MaxHealth;
            curMoveForce = moveForce;
            curMaxVelocity = maxVelocity;
      }

      private void Update()
      {
            // ���� ������Ʈ�� �������� ������ return
            if (!PhotonNetwork.IsMasterClient) return;

            GetInputVec();

            SetBodyDirection();

            ControlItems();

            // test code
            //if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //      IsHurt = true;
            //}
      }

      private void FixedUpdate()
      {
            // ���� ������Ʈ�� �������� ������ return
            if (!photonView.IsMine) return;

            MoveBody();
      }

      private void GetInputVec()
      {
            inputVec.x = Input.GetAxisRaw("Horizontal WASD");
            inputVec.y = Input.GetAxisRaw("Vertical WASD");
            photonView.RPC(nameof(RPC_SetInputVec), RpcTarget.Others, inputVec);
      }
      [PunRPC]
      private void RPC_SetInputVec(Vector2 _inputVec)
      {
            inputVec = _inputVec;
      }

      private void SetBodyDirection()
      {
            if (inputVec.x > 0) {
                  photonView.RPC(nameof(RPC_SetFlipX), RpcTarget.All, false);
                  // spriteRenderer.flipX = false;
            }
            else if (inputVec.x < 0) {
                  photonView.RPC(nameof(RPC_SetFlipX), RpcTarget.All, true);
                  // spriteRenderer.flipX = true;
            }
            animator.SetInteger("XAxisRaw", (int)inputVec.x);
            animator.SetInteger("YAxisRaw", (int)inputVec.y);
      }
      [PunRPC]
      private void RPC_SetFlipX(bool flipX)
      {
            spriteRenderer.flipX = flipX;
      }

      private void MoveBody()
      {
            rigid.AddForce(inputVec.normalized * curMoveForce, ForceMode2D.Force);
            if (rigid.velocity.magnitude > curMaxVelocity) {
                  rigid.velocity = rigid.velocity.normalized * curMaxVelocity;
            }
      }

      private GameObject itemObject = default;
      private void ControlItems()
      {
            curBombCool += Time.deltaTime;

            if (Input.GetKey(KeyCode.E) && BombCount > 0 && curBombCool > maxBombCool) {
                  curBombCool = 0;
                  BombCount--;

                  //itemObject = GameManager.Instance.itemFactory.GetItem(ItemFactory.Items.Bomb, false);
                  //itemObject.transform.position = rigid.position;
                  //itemObject.SetActive(true);
                  photonView.RPC(nameof(RPC_BombControl), RpcTarget.AllBuffered);
            }
      }
      [PunRPC]
      private void RPC_BombControl()
      {
            itemObject = GameManager.Instance.itemFactory.GetItem(ItemFactory.Items.Bomb, false);
            itemObject.transform.position = rigid.position;
            itemObject.SetActive(true);
      }
      

      private bool isHurt = false;
      public bool IsHurt
      {
            get { return isHurt; }
            set {
                  if (isHurt == false) {
                        //isHurt = true;
                        photonView.RPC(nameof(RPC_SetisHurt), RpcTarget.AllBuffered, true);
                        if (Health <= 0) {
                              IsDeath = true;
                              return;
                        }

                        // Body��
                        if (photonView.IsMine) {
                              curMoveForce = moveForce + 10;
                              curMaxVelocity = maxVelocity + 2;
                        }

                        //this.animator.SetTrigger("Hit");
                        photonView.RPC(nameof(RPC_SetAnimTrigger), RpcTarget.All, "Hit");
                        //flashEffect.Flash(new Color(1, 1, 0, 1));
                        photonView.RPC(nameof(flashEffect.Flash), RpcTarget.AllBuffered, 1f, 1f, 0f, 1f);
                  }
                  else photonView.RPC(nameof(RPC_SetisHurt), RpcTarget.AllBuffered, value);
            }
      }
      [PunRPC]
      private void RPC_SetisHurt(bool value)
      {
            isHurt = value;
      }
      [PunRPC]
      private void RPC_SetAnimTrigger(string name)
      {
            this.animator.SetTrigger(name);
      }

      // For animation event
      public async void ResetIsHurtAfterAnimation()
      {
            // ������ Ŭ���̾�Ʈ(Body)�� �ƴϸ� return
            if (!PhotonNetwork.IsMasterClient) return;

            await Task.Delay(500); // 0.5 second

            IsHurt = false;
            curMoveForce = moveForce;
            curMaxVelocity = maxVelocity;
      }

      // For animation event
      public void SetHeadSpriteAlpha(float _alpha)
      {
            // ������ Ŭ���̾�Ʈ(Body)�� �ƴϸ� return
            if (!PhotonNetwork.IsMasterClient) return;

            //head.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _alpha);
            photonView.RPC(nameof(RPC_SetHeadAlpha), RpcTarget.All, _alpha);
      }
      [PunRPC]
      private void RPC_SetHeadAlpha(float _alpha)
      {
            head.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, _alpha);
      }

      private bool isDeath = false;
      public bool IsDeath
      {
            get { return isDeath; }
            set {
                  if (isDeath == false) {
                        //isDeath = true;
                        photonView.RPC(nameof(RPC_SetisDeath), RpcTarget.AllBuffered, true);
                        //GameManager.Instance.GameOver();
                        photonView.RPC(nameof(GameManager.Instance.GameOver), RpcTarget.AllBuffered);
                  }
            }
      }
      [PunRPC]
      private void RPC_SetisDeath(bool value)
      {
            isDeath = value;
      }

      private void OnDisable()
      {
            inputVec = Vector2.zero;
            rigid.velocity = Vector2.zero;

            animator.SetInteger("XAxisRaw", 0);
            animator.SetInteger("YAxisRaw", 0);
      }



      public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
      {
            if (stream.IsWriting) {
                  // ���� �÷��̾��� flipX ���� ����
                  stream.SendNext(rigid.velocity);
            }
            else {
                  // ���� �÷��̾��� flipX ���� ����
                  bodyVelocity = (Vector2)stream.ReceiveNext();
            }
      }
}
