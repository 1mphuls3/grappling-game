using System.Collections;
using UnityEngine;

public class ToggleGate : MonoBehaviour, IToggleable
{
    [SerializeField] private Vector3 offset;
    [SerializeField] public float offsetTime;
    public bool isOpen = false;

    private Vector3 startPos;
    private Coroutine moveCoroutine;

    private void Start()
    {
        startPos = transform.position;
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
        Vector3 currentPos = transform.position;
        Vector3 endPos = open ? startPos + offset : startPos;

        while (elapsed < offsetTime)
        {
            float t = elapsed / offsetTime;
            transform.position = Vector3.Lerp(currentPos, endPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
    }
}
