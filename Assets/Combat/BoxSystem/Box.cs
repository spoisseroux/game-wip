using UnityEngine;

[System.Serializable]
public struct Box
{
    [field: SerializeField] public Vector3 pos; // can be interpretted as many things, offset if a hitbox, origin if a hurtbox, etc.
    [field: SerializeField] public float length; // corresponds to Z
    [field: SerializeField] public float width; // corresponds to X
    [field: SerializeField] public float height; // corresponds to Y

    public Box(Vector3 p, float l, float w, float h)
    {
        this.pos = p;
        this.length = l;
        this.width = w;
        this.height = h;
    }

    public void SetOrigin(Vector3 origin)
    {
        pos = origin;
    }

    public Vector3 GizmoXYZ()
    {
        return new Vector3(width, height, length);
    }
}