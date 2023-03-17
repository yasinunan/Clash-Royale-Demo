using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UOP1.Helper
{
    public class UILookAt : MonoBehaviour
    {

        private void LateUpdate()
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}
