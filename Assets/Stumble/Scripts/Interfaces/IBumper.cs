using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBumper
{
    public void Bump(Vector3 direction, float magnitude, IBumper source);
    public virtual BumpSource GetSourceType() { return BumpSource.StaticBumper; }
}
