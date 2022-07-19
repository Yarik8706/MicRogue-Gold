using UnityEngine;
using UnityEngine.UI;

namespace Canvas
{
    public class GameRullsActiveButton : MonoBehaviour
    {
        [Header("Components")]
        public GameObject blackout;
        public Sprite[] gameRulls;
        public GameObject gameRull;

        private bool _isShowingGameRulls;
        private int _passedGameRulls;

        private void Start()
        {
            _passedGameRulls = 0;
            _isShowingGameRulls = false;
        }

        private void Update()
        {
            if (!_isShowingGameRulls) return;
            if (!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1) && !Input.GetMouseButtonDown(2)) return;
            if (Input.touchCount > 0)
            {
                if (!Input.touches[0].phase.Equals(TouchPhase.Began)) return;
            }
            if (_passedGameRulls >= gameRulls.Length)
            {
                gameRull.SetActive(false);
                blackout.SetActive(false);
                _isShowingGameRulls = false;
                return;
            }
            gameRull.GetComponent<Image>().sprite = gameRulls[_passedGameRulls];
            _passedGameRulls++;
        }

        public void StartShowGameRulls()
        {
            if (_isShowingGameRulls)
            {
                return;
            }
            gameRull.SetActive(true);
            blackout.SetActive(true);
            _isShowingGameRulls = true;
            _passedGameRulls = 0;
        }
    }
}