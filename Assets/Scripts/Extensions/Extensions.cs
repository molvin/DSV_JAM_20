using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void Truncate(this ref Vector3 Self, float MaxValue)
    {
        if (Self.magnitude > MaxValue)
        {
            Self = Self.normalized * MaxValue;
        }
    }
}
