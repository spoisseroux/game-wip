using UnityEngine;

[System.Serializable]
public struct Box
{
    // z-component
    [field: SerializeField] public float length;
    // x-component
    [field: SerializeField] public float width;
    // y-component
    [field: SerializeField] public float height;

    public Box(float l, float w, float h)
    {
        this.length = l;
        this.width = w;
        this.height = h;
    }

    public Vector3 GizmoXYZ()
    {
        return new Vector3(width, height, length);
    }
}