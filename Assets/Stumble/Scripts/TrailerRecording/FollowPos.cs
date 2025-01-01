using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPos : MonoBehaviour
{
    Vector3 offset;
    bool hasSavedOffset = false;
    public Transform follow;

    private void OnEnable()
    {
        offset = transform.position - follow.position;
    }

    private void LateUpdate()
    {
        transform.position = follow.position + offset;
    }
}
