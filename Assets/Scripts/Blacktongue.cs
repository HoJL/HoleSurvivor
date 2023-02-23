using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blacktongue : MonoBehaviour
{
    bool _isCapture = false;
    bool _isRelease = false;
    [SerializeField] BlacktongueRadius _radius;
    [SerializeField] float _tongueSpeed = 15.0f;
    LineRenderer _line;
    BoxCollider _lineColl;
    Vector3[] _linePoints = new Vector3[2];

    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _lineColl = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (_isCapture) return;
            Debug.Log("key");
            _radius.IsSearch = true;
            _isRelease = false;
        }
        if (_radius.Target == null) return;
        
        if (!_isCapture)
        {
            var dir = _radius.Target.transform.position - transform.parent.position;
            _linePoints[1] = Vector3.Lerp(_linePoints[1], dir + Vector3.up, Time.deltaTime * _tongueSpeed);
            _lineColl.center = _linePoints[1];
            _line.SetPositions(_linePoints);
        }
        else
        {
            if (_isRelease) return;
            _linePoints[1] = Vector3.Lerp(_linePoints[1], Vector3.zero, Time.deltaTime * _tongueSpeed);
            _lineColl.center = _linePoints[1];
            _line.SetPositions(_linePoints);
              var scale = _radius.Target.transform.localScale;
            _radius.Target.transform.localScale = Vector3.Lerp(scale, new Vector3(0.2f, 0.2f, 0.2f), Time.deltaTime * _tongueSpeed);
            _radius.Target.transform.position = _linePoints[1] + transform.parent.position;
            if (_linePoints[1].magnitude < 0.1f)
            {
                _isRelease = true;
                _radius.Target.GetComponent<Rigidbody>().isKinematic = false;
                _isCapture = false;
                _radius.Target = null;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (_radius.Target == null) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != _radius.Target) return;
        if (enemy.IsCaptured) return;
        enemy.IsCaptured = true;
        enemy.GetComponent<Rigidbody>().isKinematic = true;
        enemy.GetComponent<BoxCollider>().isTrigger = true;
        enemy.GetComponent<Animator>().SetBool("Att", false);
        _isCapture = true;
    }
}
