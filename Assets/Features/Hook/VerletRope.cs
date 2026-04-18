using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class VerletRope : MonoBehaviour
{
    public Transform anchor;
    public Transform target;

    public LineRenderer lineRenderer;
    public Camera cam;
    public int numNodes = 40;
    public int iterations = 80;
    public float nodeDist = 0.1f;
    public Vector2 gravity = new Vector2(0f, -20f);
    public bool lockPos = true;
    public VerletNode[] nodes;

    public Vector2 wrapPoint;

    private void Awake()
    {
        nodes = new VerletNode[numNodes];

        Vector2 pos = transform.position;
        for (int i = 0; i < numNodes; i++)
        {
            nodes[i] = new VerletNode(pos);
            pos.y -= nodeDist;
        }
        lineRenderer.positionCount = numNodes;
    }

    void FixedUpdate()
    {
        // Node base physics
        foreach (VerletNode node in nodes)
        {
            Vector2 prevPos = node.position;

            node.position += (node.position - node.oldPosition) + gravity * Mathf.Pow(Time.fixedDeltaTime, 2);
            node.oldPosition = prevPos;
        }

        for (int i = 0; i < iterations; i++)
        {
            SimConstraints();
            if (i % 2 == 0)
            {
                DetectCollisions();
            }
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            lineRenderer.SetPosition(i, nodes[i].position);
        }
    }

    private void SimConstraints()
    {
        for (int j = 0; j < nodes.Length - 1; j++)
        {
            VerletNode node1 = nodes[j];
            VerletNode node2 = nodes[j + 1];

            if (j == 0 && !lockPos)
            {
                node1.position = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            }
            else if (j == 0 && lockPos)
            {
                node1.position = anchor.position;
            }

            float dX = node1.position.x - node2.position.x;
            float dY = node1.position.y - node2.position.y;
            float dist = Vector2.Distance(node1.position, node2.position);
            float diff = 0;
            if (dist > 0)
            {
                diff = (nodeDist - dist) / dist;
            }

            Vector2 mov = new Vector2(dX, dY) * (diff / 2f);

            node1.position += mov;
            node2.position -= mov;
        }
    }

    private void DetectCollisions()
    {
        KeyValuePair<int, VerletNode> best = new KeyValuePair<int, VerletNode>(0, nodes[0]);
        for (int i = 0; i < nodes.Length; i++)
        {
            VerletNode node = nodes[i];

            var colliders = Physics2D.OverlapCircleAll(node.position, nodeDist*2);
            
            if (colliders.Length > 0 && i > best.Key)
            {
                best = new KeyValuePair<int, VerletNode>(i, node);
            }

            foreach (var collider in colliders)
            {
                if (!collider.CompareTag("Player"))
                {
                    Vector2 closestPoint = collider.ClosestPoint(node.position);
                    float dist = Vector2.Distance(node.position, closestPoint);

                    Vector2 normal = (node.position - closestPoint).normalized;
                    float depth = dist;

                    node.position += normal * depth;
                }
            }
        }
        wrapPoint = best.Value.position;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        for (int i = 0; i < nodes.Length-1; i++)
        {
            if(i % 2 == 0)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(wrapPoint, 0.2f);
    }
}
