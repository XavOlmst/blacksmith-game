using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshMolding : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private float _moldResistance = 0.7f;
    [SerializeField] private float _minForce = 10f;
    [SerializeField] private float _damping = 0.7f;
    private Vector3[] _originalVertices;
    private Vector3[] _displacedVertices;
    private Vector3[] _vertexVelocities;
    private Mesh _mesh;

    private void Awake()
    {
        if (!_meshFilter)
            _meshFilter = GetComponent<MeshFilter>();

        if (!_meshCollider)
            _meshCollider = GetComponent<MeshCollider>();

        _mesh = _meshFilter.mesh;
        _originalVertices = _mesh.vertices;
        _displacedVertices = _originalVertices;
        _vertexVelocities = new Vector3[_originalVertices.Length];
    }

    private void Update()
    {
        for(int i = 0; i < _displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }

        _mesh.vertices = _displacedVertices;
        _mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh
    }

    //Getters
    public Mesh GetMesh() => _mesh;
    public MeshFilter GetFilter() => _meshFilter;
    //Setters
    public void SetDamping(float damping) => _damping = damping;
    public void SetMoldingResistance(float resistance) => _moldResistance = resistance;
    public void SetDisplaceVertices(Vector3[] newVertices) => _displacedVertices = newVertices;
    public void ResetDisplaceVertices() => _displacedVertices = _mesh.vertices;


    public void AddDeformingForce(Vector3 point, float forceStrength)
    {
        Vector3 localPoint = transform.InverseTransformPoint(point);

        for(int i = 0; i< _displacedVertices.Length; i++)
        {

            AddForceToVertex(i, localPoint, forceStrength);
        }
    }

    private void AddForceToVertex(int index, Vector3 point, float forceStrength)
    {
        Vector3 direction = _displacedVertices[index] - point;

        float attenuationForce = forceStrength / (1f + direction.sqrMagnitude * direction.sqrMagnitude);

        attenuationForce -= attenuationForce * _moldResistance;

        if (attenuationForce <= _minForce)
            return;

        float speed = attenuationForce * Time.deltaTime;

        _vertexVelocities[index] = direction.normalized * speed;
    }

    private void UpdateVertex(int index)
    {
        _displacedVertices[index] += _vertexVelocities[index] * Time.deltaTime;
        _vertexVelocities[index] *= _damping;
    }

    public void ChangeVertexPos(Vector3 vertex, Vector3 newPos)
    {
        Vector3[] vertices = _mesh.vertices;

        for (int i = 0; i < _mesh.vertices.Length; i++)
        {
            if (_mesh.vertices[i] == vertex)
            {
                vertices[i] = newPos;
            }
        }

        _mesh.vertices = vertices;
        _meshCollider.sharedMesh = _mesh;
    }
}
