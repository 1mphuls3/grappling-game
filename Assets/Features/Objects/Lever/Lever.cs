using System.Linq;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject toggleObject;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip sound;

    public bool isOn = false;
    
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = FindObjectsByType<AudioSource>(FindObjectsSortMode.None).First();
    }
    public void GrappleHit()
    {
        if (isOn) spriteRenderer.flipX = false;
        else spriteRenderer.flipX= true;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(sound);
        toggleObject.GetComponent<IToggleable>().Toggle();
        isOn = !isOn;
    }
}
