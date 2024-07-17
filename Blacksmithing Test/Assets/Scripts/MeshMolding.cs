using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class VertexNeighbors
{
    Vector3 vertex;
    int index;
    int[] vertexNeighbors;

    public VertexNeighbors(Vector3 vertex, int index)
    {
        this.vertex = vertex;
        this.index = index;
    }

    public int[] GetNeighbors() => vertexNeighbors;

    public void SetNeighbors(int[] meshTriangles)
    {
        int count = meshTriangles.Length / 3;

        List<int> neighbors = new();
        neighbors.Add(index);

        for (int i = 0; i < count; i++)
        {
            int v1 = meshTriangles[i];
            int v2 = meshTriangles[i+1];
            int v3 = meshTriangles[i+2];

            if(v1 == index || v2 == index || v3 == index)
            {
                if (!neighbors.Contains(v1))
                    neighbors.Add(v1);

                if (!neighbors.Contains(v2))
                    neighbors.Add(v2);

                if (!neighbors.Contains(v3))
                    neighbors.Add(v3);
            }
        }

        neighbors.Remove(index);

        vertexNeighbors = neighbors.ToArray();
    }
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class MeshMolding : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider _meshCollider;
    //[SerializeField] private float _moldResistance = 0.7f;
    [SerializeField] private float _minForce = 10f;
    [SerializeField] private float _damping = 0.7f;
    private List<VertexNeighbors> _vertexNeighbors = new();
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

        for(int i = 0; i<_originalVertices.Length;i++)
        {
            _vertexNeighbors.Add(new(_originalVertices[i], i));
            _vertexNeighbors[i].SetNeighbors(_mesh.triangles);
        }
    }

    private void Update()
    {
        for(int i = 0; i < _displacedVertices.Length; i++)
        {
            UpdateVertex(i);
        }

        _mesh.vertices = _displacedVertices;
        _mesh.RecalculateNormals();
        _meshCollider.sharedMesh = _mesh;
    }

    //Getters
    public Mesh GetMesh() => _mesh;
    public MeshFilter GetFilter() => _meshFilter;

    //Setters
    public void SetDamping(float damping) => _damping = damping;
    public void SetDisplaceVertices(Vector3[] newVertices) => _displacedVertices = newVertices;
    public void ResetDisplaceVertices() => _displacedVertices = _mesh.vertices;
    //public void SetMoldingResistance(float resistance) => _moldResistance = resistance;

    public int GetNearestVertex(Vector3 worldPos)
    {
        Vector3 localPos = transform.InverseTransformPoint(worldPos);

        int bestVertex = -1;
        float closestDistance = float.MaxValue;

        for(int i = 0; i<_displacedVertices.Length;i++)
        {
            float distance = Vector3.Distance(_displacedVertices[i], localPos);
            if (distance < closestDistance)
            {
                bestVertex = i;
                closestDistance = distance;
            }
        }

        return bestVertex;
    }

    public IEnumerator ImpactForceAtVertex(int index, Vector3 point, float forceStrength, int depth = 3)
    {
        if (depth == 0) yield return null;

        yield return new WaitForEndOfFrame();

        Vector3 newForce = point.normalized * (forceStrength - (forceStrength * _damping));
        _vertexVelocities[index] = newForce;

        int[] connectedVertices = _vertexNeighbors[index].GetNeighbors();

        foreach(int i in connectedVertices)
        {
            ImpactForceAtVertex(i, _displacedVertices[i] - _displacedVertices[index], newForce.magnitude, depth - 1);
        }
    }

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

        float attenuationForce = forceStrength / (1f + direction.sqrMagnitude);
        attenuationForce -= attenuationForce * _damping;
        //attenuationForce -= attenuationForce * _moldResistance;

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

/*    public void ChangeVertexPos(Vector3 vertex, Vector3 newPos)
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
    }*/
}
