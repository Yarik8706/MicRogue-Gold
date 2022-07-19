using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Canvas
{
    public class CanvasLightingStrikes : MonoBehaviour
    {
        [Header("Components")]
        public Sprite[] lightningSprites;
        public GameObject lightingSprite;
        public GameObject lightingStrikeBlackout;
        public AudioSource lightingMusic;
        
        [Header("Stats")]
        public float timeStrike;
        
        private Image _lightingSpriteImage;
        private float _timeStrikeNow;

        private void Start()
        {
            _lightingSpriteImage = lightingSprite.GetComponent<Image>();
            _timeStrikeNow = timeStrike;
        }

        private void Update()
        {
            _timeStrikeNow -= Time.deltaTime;
            if (!(_timeStrikeNow <= 0)) return;
            StartCoroutine(LightingStrike());
            _timeStrikeNow = timeStrike;
        }
        
        private IEnumerator LightingStrike()
        {
            lightingSprite.SetActive(true);
            _lightingSpriteImage.sprite = lightningSprites[Random.Range(0, lightningSprites.Length)];
            lightingMusic.Play();
            yield return new WaitForSeconds(0.1f);
            lightingStrikeBlackout.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            lightingStrikeBlackout.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            lightingStrikeBlackout.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            lightingStrikeBlackout.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            var value = 1f;
            while (value > 0)
            {
                value -= Time.deltaTime;
                _lightingSpriteImage.color = new Color(255, 255, 255, value);
                yield return null;
            }
            _lightingSpriteImage.color = new Color(255, 255, 255, 1);
            lightingSprite.SetActive(false);
        }
    }
}