using UnityEngine;
using MonstroStates;
using Photon.Realtime;
using System.Threading;
using System.Collections;

public class Monstro : Monster<Monstro>
{
      private enum States { Idle, SmallJump, BigJump, TearSpray, Dead }
      private States? curState;

      [HideInInspector] public IsaacBody player;
      public Vector2 playerSearchBox;

      [HideInInspector] public bool isSmallJump = false, isBigJump = false, isTearSpray = false;

      #region Control Animation Events
      // For animation events
      [HideInInspector] public bool isJumpUp = false; // for BigJump state
      [HideInInspector] public bool isOnLand = false; // for all Jump state
      public void TriggerJumpUp(int value) => isJumpUp = value != 0;
      public void TriggerOnLand(int value) => isOnLand = value != 0;

      [HideInInspector] public bool isTearTiming; // for tear attack
      public void TriggerTearTiming(int value) => isTearTiming = value != 0;
      #endregion


      private void Start()
      {
            curState = States.Idle;
            fsm = new FSM<Monstro>(new IdleState(this));
      }

      protected override void OnEnable()
      {
            base.OnEnable();

            if (curState != null) ChangeState(States.Idle);
      }

      private void Update()
      {
            if (curState == States.Dead) {
                  return;
            }

            switch (curState) {
                  case States.Idle:
                        if (OnDead()) {
                              ChangeState(States.Dead);
                        }
                        else if (isSpawned) {
                              isSmallJump = true;
                              ChangeState(States.SmallJump);
                        }
                        break;
                  case States.SmallJump:
                        if (OnDead()) {
                              ChangeState(States.Dead);
                        }
                        else if (!isSmallJump) {
                              ChangeState(GetNextState((States)curState));
                        }
                        break;
                  case States.BigJump:
                        if (OnDead()) {
                              ChangeState(States.Dead);
                        }
                        else if (!isBigJump) {
                              ChangeState(GetNextState((States)curState));
                        }
                        break;
                  case States.TearSpray:
                        if (OnDead()) {
                              ChangeState(States.Dead);
                        }
                        else if (!isTearSpray) {
                              ChangeState(GetNextState((States)curState));
                        }
                        break;
                  case States.Dead:
                        // 
                        break;
            }

            // Debug.Log(curState.ToString());
            fsm.UpdateState();
      }

      private void ChangeState(States nextState)
      {
            curState = nextState;

            switch (curState) {
                  case States.Idle:
                        fsm.ChangeState(new IdleState(this));
                        break;
                  case States.SmallJump:
                        fsm.ChangeState(new SmallJumpState(this));
                        break;
                  case States.BigJump:
                        fsm.ChangeState(new BigJumpState(this));
                        break;
                  case States.TearSpray:
                        fsm.ChangeState(new TearSprayState(this));
                        break;
                  case States.Dead:
                        fsm.ChangeState(new DeadState(this));
                        break;
            }
      }

      private States GetNextState(States curState)
      {
            States nextState = curState;
            switch (curState) {
                  case States.SmallJump:
                        // isSmallJump = false;
                        isTearSpray = true;
                        nextState = States.TearSpray;
                        break;
                  case States.BigJump:
                        // isBigJump = false;
                        isSmallJump = true;
                        nextState = States.SmallJump;
                        break;
                  case States.TearSpray:
                        // isTearSpray = false;
                        nextState = UnityEngine.Random.Range(0, 5) == 0 ? States.SmallJump : States.BigJump;
                        if (nextState == States.SmallJump) isSmallJump = true;
                        else isBigJump = true;
                        break;
            }

            return nextState;
      }

      protected override IEnumerator ParticleSystemCoroutine(ParticleSystem _effect)
      {
            ParticleSystem effect = Instantiate(_effect,
                rigid.position + Vector2.down * 0.35f, Quaternion.identity, this.transform);
            effect.transform.localScale = _effect.transform.localScale * 1.5f;
            yield return new WaitUntil(() => !effect.isPlaying);

            isSpawned = true;
      }

      protected override void OnDisable()
      {
            base.OnDisable();
            curState = null;
      }

      //private Transform shadow;
      private void OnDrawGizmos()
      {
            Gizmos.color = Color.green;
            //shadow = transform.GetChild(0);
            //Bounds bounds = shadow.GetComponent<Collider2D>().bounds;
            //Gizmos.DrawWireCube(bounds.center, bounds.size);
      }
}
