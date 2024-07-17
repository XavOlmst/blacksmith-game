using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : MonoBehaviour
{
    [SerializeField] private float _hammerForce = 300f;
    [SerializeField] private IngotAxis _axis;
    //[SerializeField] private float _maxDeformPercent = 0.1f;
    [SerializeField] private float _maxForce = 3000f;
    [SerializeField] private LayerMask ingotLayer;
    private float startHeight = 3;

    private void Start()
    {
        if (_hammerForce > _maxForce)
            _hammerForce = _maxForce;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0) )
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out RaycastHit hit, 1000f, ingotLayer))
            {
                transform.position = new(hit.point.x, startHeight, hit.point.z);
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (isFinished) return;

/*        IngotMolding molding = collision.gameObject.GetComponentInParent<IngotMolding>();
        if (!molding) return; //early out*/

        MeshMolding mesh = collision.gameObject.GetComponent<MeshMolding>();
        if (!mesh) return; //early out

        //Split mesh at this point
        //Deform the part after the split by some hammering force

/*        float percent = molding.GetPercentAtPointOnAxis(mesh, contactPoint.point, _axis);

        if (percent == float.PositiveInfinity) return; //early out

        molding.SplitMeshOnAxisAtPercent(mesh, percent, _axis);*/
        //StartCoroutine(AddForceNextFrame(molding, contactPoint, _hammerForce));
        foreach(ContactPoint contact in collision.contacts)
        {
            Vector3 localPoint = mesh.transform.InverseTransformPoint(contact.point);
            //StartCoroutine(mesh.ImpactForceAtVertex(mesh.GetNearestVertex(contact.point), localPoint - contact.normal, _hammerForce));
            mesh.AddDeformingForce(contact.point + contact.normal * 0.1f, _hammerForce);
        }

        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

/*    private IEnumerator AddForceNextFrame(IngotMolding mold, ContactPoint point, float force)
    {
        yield return new WaitForEndOfFrame();
        mold.AddDeformingForce(point.point + point.normal, _hammerForce);

    }*/
}
