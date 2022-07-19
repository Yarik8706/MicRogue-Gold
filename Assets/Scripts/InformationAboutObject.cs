using Canvas;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class InformationAboutObject : MonoBehaviour
{
    public Sprite avatar;
    public string nameObject;
    public string description;
    private const float SpeedActive = 1f;
    private bool _isClickEvent;
    private float _clickEventTime;
    public static CanvasInformationAboutObject canvasInformationAboutObject;

    private void Update()
    {
        if(_clickEventTime >= SpeedActive)
        {
            canvasInformationAboutObject.Start(this);
            _isClickEvent = false;
        }
        
        if (_isClickEvent)
        {
            _clickEventTime += Time.deltaTime;
        }
        else
        {
            _clickEventTime = 0;
        }
    }

    public void ClickUp()
    {
        _isClickEvent = false;
    }

    public void ClickDown()
    {
        _isClickEvent = true;
    }
}
