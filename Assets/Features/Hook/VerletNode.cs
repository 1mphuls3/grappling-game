using UnityEngine;

public class VerletNode
{
    public Vector2 position;
    public Vector2 oldPosition;

    public VerletNode(Vector2 pos)
    {
        position = pos;
        oldPosition = pos;
    }
}
