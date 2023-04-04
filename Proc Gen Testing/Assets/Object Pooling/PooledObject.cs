using UnityEngine;
using UnityEngine.Pool;

public class PooledObject : MonoBehaviour
{
    private IObjectPool<GameObject> _pool;
    public void SetPool(IObjectPool<GameObject> pool) => _pool = pool;
    public void ReleaseToPool()
    {
        if (_pool != null) _pool.Release(gameObject);
        else Destroy(gameObject);
    }
}