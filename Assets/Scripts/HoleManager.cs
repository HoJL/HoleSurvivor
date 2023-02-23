using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HoleManager : MonoBehaviour
{

    [SerializeField] Transform _circleAni;
    [SerializeField] Image _expBar;
    [SerializeField] Image _hpBar;
    [SerializeField] HoleCenter _holeCenter;
    [SerializeField] float _sizeUpFactor = 1.2f;
    [SerializeField] float _expUpFactor = 1.5f;
    [SerializeField] AnimationCurve _shakeCurve = null;
    [SerializeField] float _shakeTime = 0.0f;
    [SerializeField] float _shakePower = 1.0f;
    [Space(20)]
    [SerializeField] float _maxHp = 100.0f;
    HoleParticle _holeParticle;
    Magnet _magnet;
    Shaker _shaker;
    int _currentLevel;
    public int CurrentLevel {get => _currentLevel;}
    float _currentMaxExp = 6;
    float _currentExp = 0;
    float _currentHp;
    int _absorbedEnemy = 0;
    Action<float> _doSetCamRadius;
    public event Action<float> DoSetCamRadius
    {
        add => _doSetCamRadius += value;
        remove => _doSetCamRadius += value;
    }
    Action<string> _doEnd;
    public event Action<string> DoEnd
    {
        add => _doEnd += value;
        remove => _doEnd -= value;
    }
    Action<string> _doShowSizeUpPanel;
    public event Action<string> DoShowSizeUpPanel
    {
        add => _doShowSizeUpPanel += value;
        remove => _doShowSizeUpPanel -= value;
    }
    Action _doUpdateLevel;
    public event Action DoUpdateLevel
    {
        add => _doUpdateLevel += value;
        remove => _doUpdateLevel -= value;
    }

    public void Init(Shaker shaker)
    {
        _circleAni.DORotate(new Vector3(90, 0, 90f), 0.5f).SetEase(Ease.Linear).From(new Vector3(90, 0 ,0)).SetLoops(-1);
        _currentLevel = 1;
        _holeCenter.DoGetExp += DoGetExp;
        _holeParticle = GetComponent<HoleParticle>();
        _magnet = GetComponent<Magnet>();
        _magnet.DoShowParticle += DoShowMagnetParticle;
        _shaker = shaker;
        _currentHp = _maxHp;
    }

    void DoGetExp(float exp)
    {
        _currentExp += exp;
        _absorbedEnemy++;
        var str = $"+{exp}";
        _holeParticle.ShowExpText(str, transform, transform);
        _expBar.fillAmount = _currentExp / _currentMaxExp;
        if (_currentExp >= _currentMaxExp)
        {
            _currentExp -= _currentMaxExp;
            _currentMaxExp *= _expUpFactor;
            _expBar.fillAmount = _currentExp / _currentMaxExp;
            LevelUp();
        }
    }

    void LevelUp()
    {
        _currentLevel += 1;
        var scale = transform.localScale;
        scale.x *= _sizeUpFactor;
        scale.z *= _sizeUpFactor;
        transform.localScale = scale;
        _holeParticle.ShowSizeUpText(transform, transform);
        _doUpdateLevel?.Invoke();
        //effect //UI
        _doShowSizeUpPanel?.Invoke(_currentLevel.ToString());
    }

    void DoShowMagnetParticle(bool enable)
    {
        _holeParticle.ShowMargnetParticle(enable);
    }

    public void OnHit(float level)
    {
        if (GameManager.Instance.IsGameEnd) return;
        _currentHp -= level * 2.5f;
        _hpBar.fillAmount = _currentHp/_maxHp;
        if (_currentHp <=0)
        {
            _holeParticle.ShowDieParticle();
            _doEnd?.Invoke(_absorbedEnemy.ToString());
            return;
        }
        
        _shaker.Shake(UnityEngine.Random.insideUnitCircle, _shakeCurve, _shakeTime, _shakePower);
    }

    public void UpdateFullHp()
    {
        _currentHp = _maxHp;
        _hpBar.fillAmount = _currentHp/_maxHp;
    }
}
