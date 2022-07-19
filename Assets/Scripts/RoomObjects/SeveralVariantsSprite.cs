using UnityEngine;
using Random = System.Random;

namespace RoomObjects
{
    public class SeveralVariantsSprite : MonoBehaviour
    {
        public Sprite[] sprites;
        public GameObject[] decorations;
        public bool flipToX;
        public bool flipToY;

        private SpriteRenderer _spriteRenderer;
        private GameObject _decoration;
        
        // Start is called before the first frame update
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            var randomNumber = new Random();
            _spriteRenderer.sprite = sprites[randomNumber.Next(0, sprites.Length)];
            if (flipToX && randomNumber.Next(0, 2) == 0)
            {
                _spriteRenderer.flipX = true;
            }
            if (flipToY && randomNumber.Next(0, 2) == 0)
            {
                _spriteRenderer.flipY = true;
            }
            if (randomNumber.Next(0, 7) >= 5 && decorations.Length != 0)
            {
                _decoration = Instantiate(decorations[randomNumber.Next(0, decorations.Length)], transform.position, Quaternion.identity);
            }
        }

        private void OnDestroy()
        {
            if (_decoration == null) return;
            Destroy(_decoration);
        }
    }
}
