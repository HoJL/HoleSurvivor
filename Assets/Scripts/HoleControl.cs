using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleControl : MonoBehaviour, IPlayable
{
    [SerializeField] float _speed;
    
    public void UpdateMove(float radian, Vector3 dir)
    {
        var angle = radian * Mathf.Rad2Deg;
        //transform.rotation  = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, angle, 0.0f), 10 * Time.deltaTime);
        var forward = dir.normalized;
        transform.Translate(forward * _speed * Time.deltaTime, Space.World);
    }

    public void FinishMove()
    {

    }
}
