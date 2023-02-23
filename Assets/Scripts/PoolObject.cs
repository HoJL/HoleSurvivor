using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolObject : MonoBehaviour
{
    IObjectPool<PoolObject> _pool;

    public void SetPool(IObjectPool<PoolObject> pool)
    {
        _pool = pool;
    }

    public void DestroyPoolObject(float time)
    {
        Invoke(nameof(Release), time);
    }

    void Release()
    {
        _pool.Release(this);
    }
}
