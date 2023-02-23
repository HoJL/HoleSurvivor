using UnityEngine;
using UnityEngine.Pool;

public class Pooling : MonoBehaviour
{
    protected void OnGetObj(PoolObject obj)
    {
        obj.gameObject.SetActive(true);
    }

    protected void OnReleaseObj(PoolObject obj)
    {
        obj.gameObject.SetActive(false);
    }

    protected void OnDestroyObj(PoolObject obj)
    {
        Destroy(obj.gameObject);
    }
}
