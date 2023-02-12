using Cinemachine;
using Coherence.Runtime;
using Coherence.Toolkit;
using Hibzz.Singletons;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
	[SerializeField] GameObject PlayerPrefab;
	[SerializeField] float SpawnRadius;
	[SerializeField] CinemachineVirtualCamera vcam;

	[Space]
	[SerializeField] List<Color> playerColors;

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

		// hook up with the monobridge so that the next available color can be picked by the system
		// all connection details are not available at this point
		var mono = ConnectionManager.Instance.MonoBridge;
		mono.ClientConnections.OnSynced += UpdatePlayerColor;

		// link the vcam to the spawned player
		vcam.gameObject.SetActive(true);
		vcam.Follow = CurrentPlayer.transform;
	}

	public void DespawnPlayer()
	{
		Destroy(CurrentPlayer);
		CurrentPlayer = null;
	}

	void UpdatePlayerColor(CoherenceClientConnectionManager connectionManager)
	{
		// a list representing the available colors that the player can pick from
		List<Color> AvailableColors = new List<Color>(playerColors);

		// get all the players in the scene and extract the color information from
		// their children's color modifier. If a match is found in the available colors
		// list, it'll be removed as an option that the player can pick from
		var players = FindObjectsOfType<PlayerName>();
		foreach(var player in players)
		{
			if(player.gameObject == CurrentPlayer) { continue; } // skip current player
			
			// verify that the modifer extracted is valid and remove it's color from the list
			var modifier = player.GetComponentInChildren<ColorModifier>();
			if(modifier) 
			{
				AvailableColors.Remove(modifier.color);
			}
		}

		// pick a random color from the available list of colors and apply it
		var randID = Random.Range(0, AvailableColors.Count);
		var colorModifer = CurrentPlayer.GetComponentInChildren<ColorModifier>();
		colorModifer.SetColor(AvailableColors[randID]);
		
		// it's a one and done function, so let's unhook it
		connectionManager.OnSynced -= UpdatePlayerColor;
	}
}
