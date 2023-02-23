using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParticleSystem _stompParticle;
    [SerializeField] ParticleSystem _soulParticle;
    IObjectPool<Enemy> _pool = null;
    Rigidbody _rb = null;
    Transform _target = null;
    Animator _ani = null;
    [SerializeField] EnemyManager.EnemyGrade _grade = EnemyManager.EnemyGrade.Rare;
    public EnemyManager.EnemyGrade Grade {get => _grade;}
    public float Exp {get => ((int)_grade + 1) * 1;}
    bool _isDead = false;
    int _level = 1;
    bool _isAttack = false;
    bool _isCaptured = false;
    public bool IsCaptured {get => _isCaptured; set => _isCaptured = value;}
    public bool IsTargeted{get; set;}
    public int Level {get => _level; set => _level = value;}
    public bool IsDead {get => _isDead; set => _isDead = value;}
    Action<float> _doAttack = null;
    public event Action<float> DoAttack
    {
        add
        {
            if (_doAttack == null) _doAttack += value;
        }
        remove => _doAttack -= value;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _ani = GetComponent<Animator>();
    }

    public void Init(Transform target, int level, Action<float> doAttack, EnemyManager.EnemyGrade grade)
    {
        _target = target;
        _rb = GetComponent<Rigidbody>();
        _ani = GetComponent<Animator>();
        _level = level;
        _grade = grade;
        if (_doAttack == null) _doAttack += doAttack;
        Reset();
    }

    void Reset()
    {
        _isDead = false;
        _isCaptured = false;
        _isAttack = false;
        IsTargeted = false;
        _ani.SetBool("Fall", false);
        _ani.SetBool("Att", false);
        if (_soulParticle != null)
            _soulParticle.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (GameManager.Instance.IsGameEnd) return;
        if (GameManager.Instance.IsPause) return;
        if (_target == null) return;
        if (_isDead) return;
        if (_isAttack) return;
        if (_isCaptured) 
        {
            if (_soulParticle != null)
                _soulParticle.gameObject.SetActive(true);
            return;
        }
        moveToTarget();
    }

    public void moveToTarget()
    {
        var dir = _target.position - _rb.position;
        _rb.MovePosition(transform.position + transform.forward * Time.deltaTime);
        _rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime);
    }
    public void DestroyEnemy()
    {
        _rb.velocity = Vector3.zero;
        _rb.position = Vector3.zero;
        _rb.rotation = Quaternion.identity;
        transform.rotation = Quaternion.identity;
        var pool = GetComponent<PoolObject>();
        pool.DestroyPoolObject(0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("DestroyZone"))
        {
            _ani.SetBool("Fall", false);
            DestroyEnemy();
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Hole"))
        {
            var hm = other.transform.parent.GetComponent<HoleManager>();
            if (!_isCaptured && hm.CurrentLevel < _level)
            {
                OnAttack(true);
                return;
            }
            _ani.SetBool("Fall", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hole"))
        {
            _ani.SetBool("Fall", false);
            OnAttack(false);
        }
    }

    void OnAttack(bool enable)
    {
        _isAttack = enable;
        _ani.SetBool("Att", enable);
    }

    public void ShowStompParticle()
    {
        _doAttack?.Invoke(_level);
        _stompParticle.Play();
    }
}
