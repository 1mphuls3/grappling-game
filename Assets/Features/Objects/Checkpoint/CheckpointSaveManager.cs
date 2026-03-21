using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Rendering;

public class CheckpointSaveManager : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("CheckpointID"))
        {
            CheckpointSave[] checkpoints = FindObjectsByType<CheckpointSave>(FindObjectsSortMode.None);

            foreach (CheckpointSave checkpoint in checkpoints)
            {
                if (checkpoint.checkpointID == PlayerPrefs.GetInt("CheckpointID"))
                {
                    GameObject.FindGameObjectWithTag("Player").transform.position = checkpoint.gameObject.transform.position;
                    break;
                }
            }
        }
    }
}
