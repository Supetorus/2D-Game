using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
	[SerializeField] PrefabReference respawnPrefab;

    public Checkpoint CurrentCheckpoint { get; set; }

    public void Respawn()
	{
		if (CurrentCheckpoint != null)
		{
			Instantiate(respawnPrefab.prefab, CurrentCheckpoint.spawnPoint.position, CurrentCheckpoint.spawnPoint.rotation);
		}
	}
}
