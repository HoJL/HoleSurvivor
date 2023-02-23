using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HoleParticle : Pooling
{
    [SerializeField] GameObject _sizeUpText;
    [SerializeField] GameObject _expText;
    [SerializeField] Vector3 _sizeUpOffset;
    [SerializeField] Vector3 _expOffset;
    [SerializeField] ParticleSystem _magnetParticle;
    [SerializeField] ParticleSystem _dieParticle;
    IObjectPool<PoolObject> _sizeUpPool;
    IObjectPool<PoolObject> _expPool;
    
    void Start()
    {
        _sizeUpPool = new ObjectPool<PoolObject>(CreateSizeUp, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:10);
        _expPool = new ObjectPool<PoolObject>(CreateExp, OnGetObj, OnReleaseObj, OnDestroyObj, maxSize:10);
    }

    public void ShowSizeUpText(Transform pos, Transform parent = null)
    {
        var pool = _sizeUpPool.Get();
        var p = pos.position;
        p += _sizeUpOffset;
        pool.transform.position = p;
        pool.transform.parent = parent;
        var scale = Vector3.one;
        scale.y = pos.transform.localScale.z;
        pool.transform.localScale = scale;
        pool.DestroyPoolObject(1.5f);
    }

    public void ShowExpText(string txt, Transform pos, Transform parent = null)
    {
        var pool = _expPool.Get();
        var p = pos.position;
        p += _expOffset;
        pool.transform.position = p;
        pool.transform.parent = parent;
        var scale = Vector3.one;
        scale.y = pos.transform.localScale.z;
        pool.transform.localScale = scale;
        pool.GetComponent<ExpText>().SetText(txt);
        pool.DestroyPoolObject(1.5f);
    }

    public void ShowMargnetParticle(bool enable)
    {
        _magnetParticle.gameObject.SetActive(enable);
    }

    public void ShowDieParticle()
    {
        _dieParticle.gameObject.SetActive(true);
    }

    PoolObject CreateSizeUp()
    {
        PoolObject obj = Instantiate(_sizeUpText).GetComponent<PoolObject>();
        obj.SetPool(_sizeUpPool);
        return obj;
    }

    PoolObject CreateExp()
    {
        PoolObject obj = Instantiate(_expText).GetComponent<PoolObject>();
        obj.SetPool(_expPool);
        return obj; 
    }

}
