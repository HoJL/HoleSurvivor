using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] float _power;
    List<Rigidbody> _insideRb = new List<Rigidbody>();
    List<Enemy> _enemy = new List<Enemy>();
    List<Rigidbody> _removeRb = new List<Rigidbody>();
    List<Enemy> _removeEnemy = new List<Enemy>();
    bool _isMagnetAble = false;
    public bool IsMagnetAble {get => _isMagnetAble; set => _isMagnetAble = value;}
    Action<bool> _doShowParticle;
    public event Action<bool> DoShowParticle
    {
        add => _doShowParticle += value;
        remove => _doShowParticle -= value;
    }
    HoleManager _player;
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GetComponent<HoleManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.IsGameEnd) return;
        if (GameManager.Instance.IsPause) return;
        if (Input.GetKeyDown(KeyCode.M))
        {
            _isMagnetAble = true;
            _doShowParticle?.Invoke(true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            _isMagnetAble = false;
            _doShowParticle?.Invoke(false);
        }
        if (!_isMagnetAble) return;
        if (_insideRb.Count <= 0) return;
        for (int i = 0; i < _insideRb.Count; i++)
        {
            if (_insideRb[i] == null) continue;
            var dir = transform.position - _insideRb[i].position;
            
            if (_enemy[i].IsDead || dir.magnitude < 0.1f)
            {
                _removeRb.Add(_insideRb[i]);
                _removeEnemy.Add(_enemy[i]);
                _insideRb[i].velocity = Vector3.zero;
                continue;
            }
            _insideRb[i].AddForce(dir * _power * Time.deltaTime);
        }
        
        _insideRb.RemoveAll(_removeRb.Contains);
        _enemy.RemoveAll(_removeEnemy.Contains);
        _removeRb.Clear();
        _removeEnemy.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;
        var enemy = other.GetComponent<Enemy>();
        if (enemy.Level > _player.CurrentLevel) return;
        if (enemy.IsDead) return;
        _insideRb.Add(other.attachedRigidbody);
        _enemy.Add(enemy);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;
        _insideRb.Remove(other.attachedRigidbody);
        _enemy.Remove(other.GetComponent<Enemy>());
    }

    public void EnableMagnet(bool enable)
    {
        _isMagnetAble = enable;
        _doShowParticle?.Invoke(enable);
        if (enable) return;
        _insideRb.Clear();
        _enemy.Clear();
    }
}
