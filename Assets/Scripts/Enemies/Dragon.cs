using System.Collections;
using RoomObjects;
using UnityEngine;

namespace Enemies
{
    public class Dragon : TheEnemy, ICold
    {
        [Header("Dragon Setting")]
        public LayerMask noFireLayer;
        public Transform[] firePosition;
        public GameObject sleepTimeObject;
        public Sprite[] sleepTimeSprites;
        public GameObject fire;
        
        private bool _isFall;
        private bool _isWait;
        private int _waitTime;
        private SpriteRenderer _sleepTimeSpriteRenderer;
        private int _sleepTime;
        private readonly Color _standart = new(255,255,255,1);
        private bool _isSleep;
        private int _sleepTimeCount;
        private int _waitTimeCount;

        private static readonly int IsSleep = Animator.StringToHash("isSleep");

        protected override void Start()
        {
            base.Start();
            animator.SetBool(IsSleep, true);
            _isSleep = true;
            turnedRight = true;
            _sleepTimeSpriteRenderer = sleepTimeObject.GetComponent<SpriteRenderer>();
            _sleepTime = sleepTimeSprites.Length;
        }

        public override void Active()
        {
            isTurnOver = false;
            switch (_isSleep)
            {
                case false when !_isWait:
                    BaseAction();
                    break;
                case true when _sleepTime == 0:
                    _isSleep = false;
                    animator.SetBool(IsSleep, false);
                    StartAnimationTrigger("StartAction");
                    Invoke(nameof(TurnOver), 2);
                    return;
                case true:
                    _sleepTime--;
                    _sleepTimeSpriteRenderer.sprite = sleepTimeSprites[_sleepTime];
                    StartCoroutine(DisappearanceAndTurnOver());
                    break;
                default:
                {
                    if (_isWait)
                    {
                        if (_waitTime == 2)
                        {
                            foreach (var exit in GameObject.FindGameObjectsWithTag("Exit"))
                            {
                                var exitClass = exit.GetComponent<Exit>();
                                if (exitClass.isStairDown) continue;
                                spriteRenderer.enabled = true;
                                boxCollider2D.enabled = true;
                                _isWait = false;
                                StartCoroutine(Fall(exitClass.GetNextPositionPlayer()));
                                return;
                            }
                        }
                        _waitTime++;
                    }

                    isTurnOver = true;
                    break;
                }
            }
        }
        
        private IEnumerator DisappearanceAndTurnOver()
        {
            sleepTimeObject.SetActive(true);
            yield return new WaitForSeconds(0.8f);
            var value = 1f;
            while (value > 0)
            {
                value -= Time.deltaTime;
                _sleepTimeSpriteRenderer.color = new Color(255, 255, 255, value);
                yield return null;
            }
            sleepTimeObject.SetActive(false);
            _sleepTimeSpriteRenderer.color = _standart;
            base.TurnOver();
        }

        protected override void TurnOver()
        {
            foreach (var spawnFireTransform in firePosition)
            {
                boxCollider2D.enabled = false;
                var hit = Physics2D.Linecast(spawnFireTransform.position, spawnFireTransform.position, noFireLayer);
                boxCollider2D.enabled = true;
                if (hit.collider == null)
                {
                    Instantiate(fire, spawnFireTransform.position, Quaternion.identity);
                }
            }
            base.TurnOver();
        }

        protected override void NextRoomEvent()
        {
            if (_isSleep)
            {
                _isSleep = false;
                animator.SetBool(IsSleep, false);
            }
            _isWait = true;
            _waitTime = 0;
            spriteRenderer.enabled = false;
            boxCollider2D.enabled = false;
        }

        private IEnumerator Fall(Vector3 position)
        {
            _isFall = true;
            if (turnedRight)
            {
                Flip();
            }
            transform.position = new Vector3(position.x, position.y, position.z + 10);
            inverseMoveTime = 1 / .05f;
            StartCoroutine(SmoothMovement(position));
            yield return new WaitUntil(() => transform.position == position);
            inverseMoveTime = 1 / moveTime;
            StartCoroutine(GameManager.cameraShake.Shake(0.5f, 1));
            _isFall = false;
            TurnOver();
        }

        protected override IEnumerator OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player") && _isFall)
            {
                other.gameObject.GetComponent<Player>().Died(causeOfDied[Random.Range(0, causeOfDied.Length)]);
                yield break;
            }
            yield return base.OnTriggerEnter2D(other);
        }

        public override void Died() {}

        private void BaseAction()
        {
            switch (transform.position.x - GameManager.player.transform.position.x)
            {
                case < 0:
                    Flip(true);
                    break;
                case > 0:
                    Flip(false);
                    break;
            }

            base.Active();
        }

        public void Cold(GameObject cold)
        {
            Destroy(cold);
        }
    }
}
