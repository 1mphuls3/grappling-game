using System.Linq;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] private GameObject toggleObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private AudioSource audioSource;
    [SerializeField] private AudioClip pressSound;
    [SerializeField] private AudioClip releaseSound;

    private int objectsOnButton = 0;
    private bool isPressed = false;

    private void Start()
    {
        audioSource = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).First();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsValidPresser(collision))
        {
            objectsOnButton++;

            if (!isPressed)
            {
                Press();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsValidPresser(collision))
        {
            objectsOnButton--;

            if (objectsOnButton <= 0)
            {
                Release();
            }
        }
    }

    private bool IsValidPresser(Collider2D collider)
    {
        return collider.CompareTag("Player") || collider.CompareTag("Box");
    }

    private void Press()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(pressSound);

        isPressed = true;
        toggleObject.GetComponent<IToggleable>().Toggle();
        spriteRenderer.sprite = onSprite;
    }

    private void Release()
    {
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(releaseSound);

        isPressed = false;
        toggleObject.GetComponent<IToggleable>().Toggle();
        spriteRenderer.sprite = offSprite;
    }
}
