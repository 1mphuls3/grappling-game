using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * All code is original work, with Unity Documentation referenced for identifying Unity
 * specific methods and their correct usage and outputs.
 */
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private PlayerController followTarget;
    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float speedMultiplier = 0.5f;
    [SerializeField] private float offsetSmoothTime = 0.2f;

    private float currentOffsetX;
    private float targetOffsetX;
    private Camera cam;

    void Start()
    {
        currentOffsetX = offset.x;
        offset.z = transform.position.z;
        cam = GetComponent<Camera>();

        transform.position = followTarget.transform.position;
    }

    void LateUpdate()
    {
        // Handle camera facing offset
        bool facingLeft = followTarget.spriteRenderer.flipX;

        // Flip target offset based on facing direction
        targetOffsetX = facingLeft ? -offset.x : offset.x;

        // Interpolate between current and target x offset
        currentOffsetX = Mathf.Lerp(currentOffsetX, targetOffsetX, Time.deltaTime / offsetSmoothTime);

        Vector3 targetPos = new Vector3(
            followTarget.transform.position.x + currentOffsetX,
            followTarget.transform.position.y + offset.y,
            offset.z);

        float distance = Vector3.Distance(transform.position, targetPos);
        float dynamicSpeed = baseSpeed + distance * speedMultiplier;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, dynamicSpeed * Time.deltaTime);

    }
}
