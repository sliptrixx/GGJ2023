using Coherence.Runtime;
using Coherence.Toolkit;
using Hibzz.Singletons;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : Singleton<SpawnManager>
{
	[SerializeField] GameObject PlayerPrefab;
	[SerializeField] float SpawnRadius;

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
	}

	void UpdatePlayerColor(CoherenceClientConnectionManager connectionManager)
	{
		// get the number of clients and based on that assign the color
		var index = connectionManager.ClientConnectionCount - 1;

		// color modifier component is used to change the color of a material (as defined by the material
		// id in the inspector)
		var colorModifier = CurrentPlayer.GetComponentInChildren<ColorModifier>();
		colorModifier.SetColor(playerColors[index]);

		// it's a one and done function, so let's unhook it
		connectionManager.OnSynced -= UpdatePlayerColor;
	}
}
