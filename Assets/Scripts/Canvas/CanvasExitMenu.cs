using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canvas
{
    public class CanvasExitMenu : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private bool _isMove;

        private IEnumerator Move(Vector2 targetPosition)
        {
            if (_isMove)
            {
                yield break;
            }
            _isMove = true;
            transform.DOMove(targetPosition, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            _isMove = false;
        }

        private void OnEnable()
        {
            StartCoroutine(Move(new Vector3(Screen.width/2, Screen.height/2)));
        }

        public void CloseDialog()
        {
            StartCoroutine(CloseDialogCourotine());
        }

        private IEnumerator CloseDialogCourotine()
        {
            yield return Move(new Vector2(Screen.width / 2, Screen.height * 1.2f));
            gameObject.SetActive(false);
        }
        
        public void ExitOnMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
