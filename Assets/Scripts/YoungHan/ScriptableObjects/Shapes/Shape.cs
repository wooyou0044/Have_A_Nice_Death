using UnityEngine;

public abstract class Shape : ScriptableObject
{
    public abstract Strike.PolygonArea GetPolygonArea(Transform transform, string[] tags);
}