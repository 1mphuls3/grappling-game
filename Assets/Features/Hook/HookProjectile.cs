using UnityEngine;

public class HookProjectile : MonoBehaviour
{
    [SerializeField] private HingeJoint2D joint;
    [SerializeField] private GameObject ropePrefab;

    public bool isHooked = false;

    private void Start()
    {
        gameObject.GetComponent<HingeJoint2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHooked || collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D collisionTarget = collision.gameObject.GetComponent<Rigidbody2D>();

        if (collisionTarget != null)
        {
            joint.connectedBody = collisionTarget;
            gameObject.GetComponent<HingeJoint2D>().enabled = true;

            RopeGenerator rope = Instantiate(ropePrefab).GetComponent<RopeGenerator>();

            rope.transform.parent = transform;
            rope.startPos = GameObject.FindGameObjectWithTag("Player");
            rope.endPos = gameObject;
            rope.Generate();

            isHooked = true;

            var interactable = collision.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.GrappleHit();
            }
        }
    }
}
