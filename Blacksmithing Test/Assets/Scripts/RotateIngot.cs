using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateIngot : MonoBehaviour
{
    [SerializeField] private GameObject _ingot;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            RotateIngot90();
    }

    public void RotateIngot90()
    {
        Vector3 eulerAngles = transform.localEulerAngles;
        _ingot.transform.Rotate(90, 0, 0);
    }

}
