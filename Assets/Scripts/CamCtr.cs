using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CamCtr : MonoBehaviour
{
   public enum UpdateTime
    {
        Manual      = 0,
        LateUpdate  = 1,
    }

    [SerializeField] UpdateTime _updateTime = UpdateTime.LateUpdate;

    [Space]
    [SerializeField] Transform                      _target         = null;
    [SerializeField] [Range(0.0f, 360.0f)] float    _azimuth        = 0.0f;
    [SerializeField] [Range(0.0f, 90.0f)] float     _elevation      = 60.0f;
    [SerializeField] float                          _radius         = 15.0f;
    [SerializeField] Vector3                        _beginOffset    = Vector3.zero;
    [SerializeField] Vector3                        _finishOffset   = Vector3.zero;

    [Space]
    [SerializeField] bool   _immediately    = false;
    [SerializeField] float  _moveSpeed      = 3.5f;



    public Transform Target => _target;
    public float Azimuth
    {
        set => _azimuth = Mathf.Repeat(value, 360.0f);
        get => _azimuth;
    }
    public float Elevation
    {
        set => _elevation = Mathf.Repeat(value, 90.0f);
        get => _elevation;
    }
    public float Radius
    {
        set => _radius = Mathf.Max(0.0f, value);
        get => _radius;
    }
    public Vector3 BeginOffset
    {
        set => _beginOffset = value;
        get => _beginOffset;
    }
    public Vector3 FinishOffset
    {
        set => _finishOffset = value;
        get => _finishOffset;
    }

    void LateUpdate()
    {
        if (_updateTime != UpdateTime.LateUpdate)
            return;

        Tracking(_immediately);
    }

    public void Tracking(bool immediately)
    {
        if (null == _target)
            return;

        transform.localRotation = Quaternion.Euler(90.0f - _elevation, -_azimuth, 0.0f);

        var trackingInfo = CalculateTrackingInfo();
        if (immediately)
        {
            var dirPosition    = trackingInfo.trackerPosition;
            dirPosition       += _finishOffset;
            transform.position = dirPosition;
        }
        else
        {
            var deltaTime       = Time.deltaTime;
            var moveSpeed       = _moveSpeed * deltaTime;
            var dirPosition     = Vector3.MoveTowards(transform.position - _finishOffset, trackingInfo.trackerPosition, moveSpeed);
            dirPosition        += _finishOffset;
            transform.position  = dirPosition;
        }
    }
    (Vector3 targetPosition, Vector3 trackerPosition) CalculateTrackingInfo()
    {
        var radianE         = (90.0f - _elevation) * Mathf.Deg2Rad;
        var radianA         = (_azimuth - 90.0f) * Mathf.Deg2Rad;
        var hypotenus       = Mathf.Cos(radianE) * _radius;
        var x               = Mathf.Cos(radianA) * hypotenus;
        var y               = Mathf.Sin(radianE) * _radius;
        var z               = Mathf.Sin(radianA) * hypotenus;
        var targetPosition  = _target.position + _beginOffset;
        var trackerPosition = targetPosition + new Vector3(x, y, z);
        
        return (targetPosition, trackerPosition);
    }
}
