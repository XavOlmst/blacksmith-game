using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public enum IngotAxis
{
    X_AXIS,
    Y_AXIS,
    Z_AXIS,
}

public class IngotMolding : MonoBehaviour
{
   /* [SerializeField] private MeshMolding _initMold;
    [SerializeField] private float _damping = 0.7f;
    [Range(0, 1)][SerializeField] private float _moldResistance = 0.7f;
    private List<MeshMolding> _meshMoldings = new List<MeshMolding>();

    private int[] _xConnectedIndexs = { 0, 1, 2, 3, 4, 5, 6, 7 };
    private int[] _yConnectedIndexs = { 0, 2, 1, 3, 4, 6, 5, 7 };
    private int[] _zConnectedIndexs = { 0, 6, 2, 4, 3, 5, 1, 7 };

    private void Awake()
    {
        _meshMoldings.Add(_initMold);

        foreach(MeshMolding mesh in _meshMoldings)
        {
            mesh.SetDamping(_damping);
            mesh.SetMoldingResistance(_moldResistance);
        }
    }

    public void AddDeformingForce(Vector3 point, float forceStrength, bool forceIsWorldSpace = true)
    {
        foreach(MeshMolding mesh in _meshMoldings)
        {
            Vector3 localPoint = point;

            if(forceIsWorldSpace)
            {
                localPoint = mesh.transform.InverseTransformPoint(point);
            }


            mesh.AddDeformingForce(localPoint, forceStrength);
        }
    }

*//*    public void ReduceMeshScaleByPercent(MeshMolding mesh, float percent, IngotAxis axis)
    {
        Vector3 localScale = mesh.transform.localScale;
        Vector3 newScale =  localScale - (localScale * percent);

        switch (axis)
        {
            case IngotAxis.X_AXIS:
                mesh.transform.localScale = new(localScale.x, newScale.y, newScale.z);
                break;
            case IngotAxis.Y_AXIS:
                mesh.transform.localScale = new(newScale.x, localScale.y, newScale.z);
                break;
            case IngotAxis.Z_AXIS:
                mesh.transform.localScale = new(newScale.x, newScale.y, localScale.z);
                break;
        }

    }*/

/*    public void IncreaseMeshScaleByPercent(MeshMolding mesh, float percent)
    {
        Vector3 localScale = mesh.transform.localScale;
        mesh.transform.localScale = localScale + (localScale * percent);
    }

    public void ChangeVertexPos(Vector3 vertex, Vector3 newPos)
    {
        foreach(var mesh in meshMoldings)
        {
            foreach(var v in mesh.GetMesh().vertices)
            {
                if (v == vertex)
                    mesh.ChangeVertexPos(v, newPos);
            }
        }
    }*/

    /*public Vector3 GetNearestVertex(Vector3 pos)
    {
        Vector3 nearestVertex = new();
        float bestDistance = float.MaxValue;

        foreach (var mesh in meshMoldings)
        {
            Vector3 localPos = mesh.transform.InverseTransformPoint(pos);
            Vector3[] vertices = mesh.GetMesh().vertices;

            foreach (var vertex in vertices)
            {
                float distance = Vector3.Distance(vertex, localPos);
                if (distance < bestDistance)
                {
                    nearestVertex = vertex;
                    bestDistance = distance;
                }
            }
        }

        return nearestVertex;
    }*/

/*    public void ReduceVertexPosOnAxis(Vector3 vertex, float percentReduction, IngotAxis axis)
    {
        foreach (var mesh in meshMoldings)
        {
            Vector3[] vertices = mesh.GetMesh().vertices;
            foreach (var v in vertices)
            {   
                Vector3 newPos = v - v * percentReduction;
                switch(axis)
                {
                    case IngotAxis.X_AXIS:
                        if (v.x == vertex.x)
                            ChangeVertexPos(v, new(v.x, newPos.y, v.z));
                        break;
                    case IngotAxis.Y_AXIS:
                        if (v.y == vertex.y)
                            ChangeVertexPos(v, new(v.x, v.y, newPos.z));
                        break;
                    case IngotAxis.Z_AXIS:
                        if (v.z == vertex.z)
                            ChangeVertexPos(v, new(newPos.x, v.y, v.z));
                        break;
                }
            }
        }
    }*/

/*    public MeshMolding GetNearestMesh(Vector3 pos)
    {
        MeshMolding nearestMesh = null;
        float nearestDistance = float.MaxValue;

        foreach(var mesh in _meshMoldings)
        {
            Vector3 localPos = mesh.transform.InverseTransformPoint(pos);

            float distance = Vector3.Distance(mesh.transform.localPosition, localPos);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestMesh = mesh;
            }
        }

        return nearestMesh;
    }*//*

    //gods this is oddly complex...
    public float GetPercentAtPointOnAxis(MeshMolding mesh, Vector3 pos, IngotAxis axis)
    {
        int[] connectedIndexs = GetConnectedIndexsOnAxis(axis);
        //MeshMolding mesh = GetNearestMesh(pos);

        Vector3 localPos = mesh.transform.InverseTransformPoint(pos);


        Vector3[] vertices = mesh.GetMesh().vertices;
        float distance = 0; //TODO: hard coded rn, needs to be fixed later
        float posDistance = 0;  //TODO: hard coded rn, needs to be fixed later

        float averagePercent = 0;

        for(int i = 0; i<connectedIndexs.Length; i+=2)
        {
            switch (axis)
            {
                case IngotAxis.X_AXIS:
                    distance = vertices[connectedIndexs[i+1]].x - vertices[connectedIndexs[i]].x;
                    posDistance = localPos.x - vertices[connectedIndexs[i]].x;
                    break;
                case IngotAxis.Y_AXIS:
                    distance = vertices[connectedIndexs[i+1]].y - vertices[connectedIndexs[i]].y;
                    posDistance = localPos.y - vertices[connectedIndexs[i]].y;
                    break;
                case IngotAxis.Z_AXIS:
                    distance = vertices[connectedIndexs[i+1]].z - vertices[connectedIndexs[i]].z;
                    posDistance = localPos.z - vertices[connectedIndexs[i]].z;
                    break;
            }

            averagePercent += posDistance / distance;
        }

        return averagePercent / (connectedIndexs.Length / 2);
    }

    public int[] GetConnectedIndexsOnAxis(IngotAxis axis)
    {
        int[] temp = { };

        switch (axis)
        {
            case IngotAxis.X_AXIS:
                return _xConnectedIndexs;
            case IngotAxis.Y_AXIS:
                return _yConnectedIndexs;
            case IngotAxis.Z_AXIS:
                return _zConnectedIndexs;
            default:
                return temp;
        }
    }

    public void SplitMeshOnAxisAtPercent(MeshMolding splittingMesh, float percent, IngotAxis axis)
    {
        if (transform.childCount > 100)
            return;

        if (percent <= 0.01f || percent >= 0.99f)
            return;

        int[] connectedIndexs = GetConnectedIndexsOnAxis(axis);

        if (connectedIndexs.Length == 0)
            return;

        MeshMolding newMoldingMesh = Instantiate(_initMold, transform);

        Mesh mesh = splittingMesh.GetMesh();
        Mesh newMesh = newMoldingMesh.GetMesh();
        Vector3[] vertices = mesh.vertices;
        Vector3[] newVertices = newMesh.vertices;

        for(int i = 0; i < connectedIndexs.Length; i+=2)
        {
            Vector3 direction = vertices[i + 1] - vertices[i];

            if (direction.magnitude < 0.05)
            {
                Destroy(newMoldingMesh.gameObject);
                return;
            }

            newMoldingMesh.ChangeVertexPos(newVertices[i], vertices[i]);


            splittingMesh.ChangeVertexPos(vertices[i],  *//*split point*//* vertices[i] + direction * percent);
            newMoldingMesh.ChangeVertexPos(newVertices[i+1], *//*split point*//* vertices[i] + direction * percent);
        }

        splittingMesh.ResetDisplaceVertices();
        newMoldingMesh.ResetDisplaceVertices();

        newMoldingMesh.SetDamping(_damping);
        newMoldingMesh.SetMoldingResistance(_moldResistance);

        _meshMoldings.Add(newMoldingMesh);
    }*/
}
