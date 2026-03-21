using System.Collections;
using UnityEngine;

public class RotateGate : MonoBehaviour, IToggleable
{
    [SerializeField] private Vector3 rotation;
    [SerializeField] public float offsetTime;
    public bool isOpen = false;

    private Quaternion startRot;
    private Coroutine moveCoroutine;

    private void Start()
    {
        startRot = transform.rotation;
    }

    public void Toggle()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        isOpen = !isOpen;

        moveCoroutine = StartCoroutine(GateMove(isOpen));
    }


    private IEnumerator GateMove(bool open)
    {
        float elapsed = 0;
        Quaternion currentRot = transform.rotation;
        Quaternion endRot = open ? startRot * Quaternion.Euler(rotation) : startRot;

        while (elapsed < offsetTime)
        {
            float t = elapsed / offsetTime;
            transform.rotation = Quaternion.Lerp(currentRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
    }
}
