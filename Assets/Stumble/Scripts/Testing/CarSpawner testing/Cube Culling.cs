using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCulling : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "CullZone")
        {
            Destroy(gameObject);
        }
    }
}
