using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlacktongueRadius : MonoBehaviour
{
    bool _isSearch = false;
    public bool IsSearch {get => _isSearch; set => _isSearch = value;}
    Enemy _target = null;
    public Enemy Target {get => _target; set => _target = value;}
    private SphereCollider m_SphereCollider; 

    void Start() 
    {
        m_SphereCollider = GetComponent<SphereCollider>(); 
    }

    void OnTriggerStay(Collider other)
    {
        if (!_isSearch) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Block")) return;
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy.Grade == EnemyManager.EnemyGrade.Normal) return;
        if (enemy.IsTargeted) return;
        Debug.Log("find");
        enemy.IsTargeted = true;
        _target = enemy;
        _isSearch = false;
    }
    
    void OnDrawGizmos() 
    {
        if (m_SphereCollider == null) return;
        float radius = m_SphereCollider.radius; 

        Vector3 position = m_SphereCollider.center - transform.position; 

        Gizmos.matrix 
            = Matrix4x4.TRS(this.transform.TransformPoint(transform.position), 
            this.transform.rotation, this.transform.lossyScale); 

        Gizmos.color = Color.red; 
        Gizmos.DrawWireSphere(position, radius); 
    } 
}
