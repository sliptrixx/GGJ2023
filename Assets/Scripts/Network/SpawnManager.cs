using Hibzz.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
	[SerializeField] GameObject PlayerPrefab;
	[SerializeField] float SpawnRadius;

	public GameObject CurrentPlayer { get; protected set; }

	public void SpawnPlayer()
	{
		// determine where to spawn
		Vector3 startpos = transform.position + Random.insideUnitSphere * SpawnRadius;
		startpos.y = transform.position.y;

		// spawn the player and update it's name
		CurrentPlayer = Instantiate(PlayerPrefab, startpos, Quaternion.identity);
		CurrentPlayer.name = $"[local] Player - {ConnectionManager.Instance.PlayerName}";

		// additionally set the name on the player's nameplate
		var playerName = CurrentPlayer.GetComponent<PlayerName>();
		playerName.SetName(ConnectionManager.Instance.PlayerName);
	}
}
