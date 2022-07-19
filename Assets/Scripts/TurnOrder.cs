using System.Collections;
using Canvas;
using UnityEngine;

public class TurnOrder : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private CanvasTurnOrder _canvasTurnOrder;
    
    // Start is called before the first frame update
    private void Start()
    {
        _canvasTurnOrder = FindObjectOfType<CanvasTurnOrder>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = new Color(255, 255, 255, 0);
        StartCoroutine(AllLifeActivities());
    }

    private IEnumerator AllLifeActivities()
    {
        var value = 0f;
        while (value < 1)
        {
            value += Time.deltaTime;
            _spriteRenderer.color = new Color(255, 255, 255, value);
            yield return null;
        }
        yield return new WaitUntil(() => _canvasTurnOrder.isEnded);
        value = 1f;
        while (value > 0)
        {
            value -= Time.deltaTime;
            _spriteRenderer.color = new Color(255, 255, 255, value);
            yield return null;
        }
        Destroy(gameObject);
    }
}
