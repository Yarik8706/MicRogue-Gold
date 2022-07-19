using System;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public enum FadeState
    {
        In,
        Out,
        Stop,
        InEnd,
        OutEnd
    }

    private Texture _colorTexture;
    private Color _fadeColor = Color.black;

    [HideInInspector] public float fadeBalance;

    public FadeState fadeState;
    public float fadeSpeed; // Скорость стремления баланса
    public float fromInDelay; // Мнимые задержки перед началом процесса затемнение/расцветания
    public float fromOutDelay;

    private void Awake()
    {
        var nullTexture = new Texture2D(1, 1);
        nullTexture.SetPixel(0, 0, Color.black);
        nullTexture.Apply();
        _colorTexture = nullTexture;
        fadeBalance = (1 + fromInDelay);
    }

    private void Update()
    {
        _fadeColor.a = fadeBalance;
        if (fadeBalance > (1 + fromInDelay))
        {
            fadeBalance = (1 + fromInDelay);
            fadeState = FadeState.InEnd;
        }

        if (fadeBalance < -(0 + fromOutDelay))
        {
            fadeBalance = -(0 + fromOutDelay);
            fadeState = FadeState.OutEnd;
        }

        switch (fadeState)
        {
            case FadeState.In:
                fadeBalance += Time.deltaTime * fadeSpeed;
                break;
            case FadeState.Out:
                fadeBalance -= Time.deltaTime * fadeSpeed;
                break;
            case FadeState.Stop:
                fadeBalance -= 0;
                break;
            case FadeState.InEnd:
                fadeBalance = (1 + fromInDelay);
                break;
            case FadeState.OutEnd:
                fadeBalance = -(0 + fromOutDelay);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnGUI()
    {
        GUI.depth = -2;
        GUI.color = _fadeColor;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _colorTexture, ScaleMode.StretchToFill, true);
    }
}