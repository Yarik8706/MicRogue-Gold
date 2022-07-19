using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class CanvasInformationAboutObject : MonoBehaviour
    {
        public Image avatarImage;
        public Text textName;
        public Text textDescription;
        private Vector3 _targetPoint;
        private bool _isMove;
        private bool _isStart;
        private Vector3 _endPosition;

        private void Start()
        {
            _endPosition = transform.position;
        }

        private void Update()
        {
            if (_isMove || !_isStart) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2) &&
                Input.touchCount <= 0) return;
            _isStart = false;
            StartCoroutine(MoveTo(_endPosition));
        }

        private IEnumerator MoveTo(Vector3 to)
        {
            _isMove = true;
            transform.DOMove(to, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            _isMove = false;
        }

        public void Start(InformationAboutObject informationAboutObject)
        {
            avatarImage.sprite = informationAboutObject.avatar;
            var newSize = new Vector2
            {
                x = informationAboutObject.avatar.rect.width * 4,
                y = informationAboutObject.avatar.rect.height * 4
            };
            avatarImage.rectTransform.sizeDelta = newSize;
            textName.text = informationAboutObject.nameObject;
            textDescription.text = informationAboutObject.description;
            _isStart = true;
            StartCoroutine(MoveTo(new Vector2(Screen.width / 2, Screen.height / 2)));
        }
    }
}
