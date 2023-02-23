using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCenter : MonoBehaviour
{
    Action<float> _doGetExp = null;
    public event Action<float> DoGetExp
    {
        add => _doGetExp += value;
        remove => _doGetExp += value;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            _doGetExp?.Invoke(enemy.Exp);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Block"))
        {
            var enemy = other.GetComponent<Enemy>();
            if (enemy == null) return;
            enemy.IsDead = true;
        }
    }
}
