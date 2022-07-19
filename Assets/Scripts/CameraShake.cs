using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    private Vector3 _cameraPosition;
    private void Start()
    {
        _cameraPosition = transform.position;
    }
    
    public IEnumerator Shake(float time, float magnitube)
    {
        var elapsedTime = 0f;
        while (elapsedTime < time)
        {
            var xPos = Random.Range(-0.5f, 0.5f) * magnitube;
            var yPos = Random.Range(-0.5f, 0.5f) * magnitube;
            transform.localPosition = new Vector3(xPos, yPos, _cameraPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _cameraPosition;
    }
}
