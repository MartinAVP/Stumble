using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBumper
{
    public void Bump(Vector3 direction, float magnitude, BumpSource source);
    public void Bump(Vector3 direction, Vector3 position, float magnitude, BumpSource source);
    public Vector3 GetBumpDirection(GameObject other);
    public float GetBumpMagnitude();
    public BumpSource GetBumpSource();
}
