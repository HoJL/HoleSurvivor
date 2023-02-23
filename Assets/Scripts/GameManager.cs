using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] UIManager _uiManager;
    [SerializeField] HoleControl _player;
    [SerializeField] EnemyManager _enemyManager;
    [SerializeField] HoleManager _hole;
    [SerializeField] CamCtr _tracker;
    [SerializeField] Shaker _shaker;
    [Space(20)]
    [SerializeField] float _magnetTime = 5.0f;

    public static GameManager Instance; 
    public bool IsGameEnd {get; set;}
    public bool IsPause {get; set;}
    public bool IsGameStart {get; set;}
    Coroutine _powerCo;
    
    private void Awake()
    {
        Instance = this;
        IsGameStart = false;
        _uiManager.DoGetPower += DoGetPower;
        _uiManager.Init(_player);
        _enemyManager.Init(_player.transform);
        _hole.Init(_shaker);
        _hole.DoSetCamRadius += DoSetCamRadius;
        _hole.DoShowSizeUpPanel += DoShowSizeUpPanel;
        _hole.DoUpdateLevel += DoUpdateLevel;
        _hole.DoEnd += DoEnd;
    }

    void DoSetCamRadius(float radiusFactor)
    {
        _tracker.Radius *= (radiusFactor);
    }

    void DoEnd(string absorb)
    {
        if (IsGameEnd) return;
        IsGameEnd = true;
        StartCoroutine(EndWait(absorb));
    }

    IEnumerator EndWait(string absorb)
    {
        var time = 0.0f;
        var waitTime = 0.8f;
        while(time < waitTime)
        {
            yield return null;
            time += Time.deltaTime;
        }
        _uiManager.ShowRewardPanel(absorb);
        Time.timeScale = 0;
    }

    void DoShowSizeUpPanel(string level)
    {
        if (IsGameEnd) return;
        _uiManager.ShowSizeUpPanel(level);
        IsPause = true;
        Time.timeScale = 0;
    }

    void DoGetPower(int id)
    {
        switch(id)
        {
            case 1:
                if (_powerCo != null)
                    StopCoroutine(_powerCo);
                _powerCo = StartCoroutine(InvokePower());
            break;

            case 2:
                _hole.UpdateFullHp();
            break;
        }
        
        StartCoroutine(InvokeChangeRadius());
    }

    IEnumerator InvokePower()
    {
        var magnet = _hole.GetComponent<Magnet>();

        var time = 0.0f;
        magnet.EnableMagnet(true);
        
        while(time < _magnetTime)
        {
            yield return null;
            _uiManager.ShowSkillPanel(time, _magnetTime);
            time += Time.deltaTime;
        }
        magnet.EnableMagnet(false);
        _uiManager.HideSkillPanel();
    }

    IEnumerator InvokeChangeRadius()
    {
        var odd = _hole.CurrentLevel % 2;
        if (odd == 0) yield break;
        var time = 0.0f;
        var changeTime = 0.8f;
        var originRad = _tracker.Radius;
        var target = originRad * 1.5f;
        while(time < changeTime)
        {
            yield return null;
            _tracker.Radius = Mathf.Lerp(originRad, target, time / changeTime);
            time += Time.deltaTime;
        }
    }

    void DoUpdateLevel()
    {
        var odd = _hole.CurrentLevel % 2;
        if (odd == 0) return;
        _enemyManager.SpawnRadius += 0.5f;
        _enemyManager.SpawnTime -= 0.375f;
    }

}
