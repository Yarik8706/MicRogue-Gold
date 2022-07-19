using Enemies;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoomObjects
{
    public class BrokenWall : MonoBehaviour, IFireAttack, IClickToAvailablePosition
    {
        public Sprite brokenWallIdleSprite;
        public Sprite brokenWallCanClickSprite;
        private bool _playerCanGo;
        private bool _isActive;
        private SpriteRenderer _spriteRenderer;
        
        private void Start()
        {
            GameplayEventManager.OnNextMove.AddListener(NoClickEvent);
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void FireAttack(GameObject fire)
        {
            Instantiate(fire, transform.position, Quaternion.identity);
            _isActive = true;
            _spriteRenderer.sprite = brokenWallIdleSprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isActive || !_playerCanGo) return;
            StartCoroutine(GameManager.player.Move(transform.position));
            StartCoroutine(GameManager.gameManager.NextRoom(2));
        }

        public void ClickEvent()
        {
            _playerCanGo = true;
            if(_isActive) _spriteRenderer.sprite = brokenWallCanClickSprite;
        }

        private void NoClickEvent()
        {
            _playerCanGo = false;
            if(_isActive) _spriteRenderer.sprite = brokenWallIdleSprite;
        }
    }
}
