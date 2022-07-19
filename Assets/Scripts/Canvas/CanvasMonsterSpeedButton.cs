using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class CanvasMonsterSpeedButton : MonoBehaviour
    {
        public Sprite activeSprite;
        public Sprite disableSprite;
        public CanvasMonsterSpeedButton canvasMonsterSpeedButton;
        public float monsterMoveAnimationSpeed;
        public float monsterMoveTimeSpeed;

        private bool _notActive;
        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void Click()
        {
            if (!_notActive) return;
            canvasMonsterSpeedButton.Diseble();
            PlayerPrefsSafe.SetFloat("MonsterMoveAnimationSpeed", monsterMoveAnimationSpeed);
            PlayerPrefsSafe.SetFloat("MonsterMoveTimeSpeed", monsterMoveTimeSpeed);
            Active();
        }

        public void Diseble()
        {
            _image.sprite = disableSprite;
            _notActive = true;
        }

        public void Active()
        {
            _image.sprite = activeSprite;
            _notActive = false;
        }
    }
}
