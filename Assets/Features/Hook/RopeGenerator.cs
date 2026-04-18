using System.Linq;
using UnityEngine;

public class RopeGenerator : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private float nodeDist = 0.5f;

    [SerializeField] public GameObject startPos;
    [SerializeField] public GameObject endPos;

    private GameObject lastNode;
    public int nodeCount = 0;
    public int maxNodeCount = 20;
    private bool generated = false;
    public void Generate()
    {
        GameObject startNode = Instantiate(nodePrefab);
        startNode.transform.position = startPos.transform.position;
        startNode.transform.parent = gameObject.transform;
        HingeJoint2D startJoint = startPos.GetComponent<HingeJoint2D>();
        startJoint.enabled = true;
        startJoint.connectedBody = startNode.GetComponent<Rigidbody2D>();

        Vector3 direction = (endPos.transform.position - startPos.transform.position).normalized;
        float dist = Vector3.Distance(startPos.transform.position, endPos.transform.position);
        nodeCount = Mathf.CeilToInt(dist / nodeDist);

        GameObject prevNode = startNode;
        for (int i = 0; i < nodeCount; i++)
        {
            HingeJoint2D prevJoint = prevNode.GetComponent<HingeJoint2D>();
            GameObject nextNode = Instantiate(nodePrefab);

            nextNode.GetComponent<RopeNode>().previousNode = prevNode.GetComponent<RopeNode>();
            prevNode.GetComponent<RopeNode>().nextNode = nextNode.GetComponent<RopeNode>();

            nextNode.transform.position = prevJoint.transform.position + direction * nodeDist;
            nextNode.transform.parent = gameObject.transform;
            prevJoint.connectedBody = nextNode.GetComponent<Rigidbody2D>();
            prevNode = nextNode;
        }

        lastNode = prevNode;
        HingeJoint2D lastJoint = lastNode.GetComponent<HingeJoint2D>();
        lastJoint.connectedBody = endPos.GetComponent<Rigidbody2D>();
        generated = true;
    }

    void Update()
    {
        if (!generated) return;

        // Generate rope visuals
        LineRenderer line = GetComponent<LineRenderer>();
        Vector3[] positions = GetComponentsInChildren<Transform>().Select(t => t.position).ToArray();
        positions[0] = startPos.transform.position;
        positions[positions.Length-1] = endPos.transform.position;

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }

    public void AddNode()
    {
        if (nodeCount >= maxNodeCount) return;

        Vector3 direction = (endPos.transform.position - lastNode.transform.position).normalized;
        float dist = Vector3.Distance(startPos.transform.position, endPos.transform.position);

        HingeJoint2D lastJoint = lastNode.GetComponent<HingeJoint2D>();

        GameObject nextNode = Instantiate(nodePrefab);

        nextNode.GetComponent<RopeNode>().previousNode = lastNode.GetComponent<RopeNode>();
        lastNode.GetComponent<RopeNode>().nextNode = nextNode.GetComponent<RopeNode>();

        nextNode.transform.position = lastNode.transform.position + direction * nodeDist;
        nextNode.transform.parent = gameObject.transform;
        nextNode.GetComponent<HingeJoint2D>().connectedBody = endPos.GetComponent<Rigidbody2D>();
        lastJoint.connectedBody = nextNode.GetComponent<Rigidbody2D>();
        lastNode = nextNode;
        nodeCount += 1;
    }

    public void RemoveNode()
    {
        RopeNode prevNode = lastNode.GetComponent<RopeNode>().previousNode;
        prevNode.joint.connectedBody = endPos.GetComponent<Rigidbody2D>();
        prevNode.transform.position = lastNode.transform.position;
        Destroy(lastNode);
        lastNode = prevNode.gameObject;

        nodeCount -= 1;
    }
}
