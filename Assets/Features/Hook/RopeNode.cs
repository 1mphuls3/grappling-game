using UnityEngine;

public class RopeNode : MonoBehaviour
{
    [SerializeField] public HingeJoint2D joint;
    public RopeNode previousNode;
    public RopeNode nextNode;
}
