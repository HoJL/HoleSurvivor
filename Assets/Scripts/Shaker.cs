using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker : MonoBehaviour
{
    Coroutine _coroutineShake = null;
    public void Shake(Vector3 dir, AnimationCurve curve, float timeLength, float maximumDistance)
    {
        if (null != _coroutineShake)
            StopCoroutine(_coroutineShake);

        _coroutineShake = StartCoroutine(CoroutineShake(dir, curve, timeLength, maximumDistance));
    }

    IEnumerator CoroutineShake(Vector3 dir, AnimationCurve curve, float timeLength, float maximumDistance)
    {
        var elapsed = 0.0f;
        while (elapsed < timeLength)
        {
            yield return null;
            elapsed += Time.deltaTime;

            var factor              = curve.Evaluate(elapsed / timeLength) * maximumDistance;
            transform.localPosition = dir * factor;
        }

        transform.localPosition = Vector3.zero;
        _coroutineShake         = null;
    }
}
