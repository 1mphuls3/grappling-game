using System.Collections.Generic;
using UnityEngine;

public class CheckpointSave : MonoBehaviour
{
    [SerializeField] private CheckpointSaveManager manager;
    public int checkpointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("CheckpointID", checkpointID);
            PlayerPrefs.Save();
        }
    }
}
