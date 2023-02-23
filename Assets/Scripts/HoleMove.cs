using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleMove : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _holeColl;
    //[SerializeField] CircleCollider2D _holeColl;
    [SerializeField] PolygonCollider2D _groundColl;
    [SerializeField] MeshCollider _meshColl;
    [SerializeField] Collider _groundMeshColl;
    [SerializeField] float _holeScale = 0.5f;
    Mesh _mesh;

    void Start()
    {
        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        foreach (var go in gos)
        {
            if (go.layer != LayerMask.NameToLayer("Block")) continue;
            Physics.IgnoreCollision(go.GetComponent<Collider>(), _meshColl, true);
        }
    }

    void FixedUpdate()
    {
        if (transform.hasChanged)
        {
            transform.hasChanged = false;
            _holeColl.transform.position = new Vector2(transform.position.x, transform.position.z);
            var scale = transform.localScale;
            scale.y = transform.localScale.z;

            _holeColl.transform.localScale = scale * _holeScale;
            MakeHoleColl();
            MakeHoleMesh();
        }
    }

    void MakeHoleColl()
    {
        Vector2[] points = _holeColl.GetPath(0);
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = _holeColl.transform.TransformPoint(points[i]);
        }
        _groundColl.pathCount = 2;
        _groundColl.SetPath(1, points);
    }

    void MakeHoleMesh()
    {
        if (_mesh != null) Destroy(_mesh);
        _mesh = _groundColl.CreateMesh(true, true);
        _meshColl.sharedMesh = _mesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        Physics.IgnoreCollision(other, _groundMeshColl, true);
        Physics.IgnoreCollision(other, _meshColl, false);
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(other, _groundMeshColl, false);
        Physics.IgnoreCollision(other, _meshColl, true);
    }
}
