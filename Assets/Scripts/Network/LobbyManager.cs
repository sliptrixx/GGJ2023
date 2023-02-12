using Coherence.Toolkit;
using Hibzz.ActionList;
using Hibzz.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
	[Header("Lobby Properties")]
	[SerializeField] int numberOfPlayersToStartGame = 2;
	[SerializeField] float waitTime = 15.0f;
	[SerializeField] float gameTime = 120.0f;

	[Header("Decryptable Objective Properties")]
	[SerializeField] GameObject DecryptablePrefab;
	[SerializeField] List<Transform> DecryptableSpawnPoints;

	public CoherenceMonoBridge MonoBridge { get; protected set; }

	[OnValueSynced(nameof(OnGameReadyToStartUpdate))]
	public bool isGameReadyToStart = false;

	[OnValueSynced(nameof(OnGameStartedUpdate))]
	public bool hasGameStarted = false;

	[OnValueSynced(nameof(OnWaitTimerChanged))]
	public float waitTimer = 0;

	[OnValueSynced(nameof(OnGameTimerChanged))]
	public float gameTimer = 0;

	[OnValueSynced(nameof(OnWinnerChanged))]
	public string winner = "";

	public CoherenceSync CurrentObjectiveSync;
	public GameObject CurrentObjective { get; protected set; }

	public void OnStateAuthority()
	{
		GetMonobridge();

		MonoBridge.ClientConnections.OnCreated += ClientConnectionCreated;
		MonoBridge.ClientConnections.OnDestroyed += ClientConnectionsDestroyed;

		if(CurrentObjectiveSync)
		{
			CurrentObjective = CurrentObjectiveSync.gameObject;
		}
	}

	public void OnStateRemote()
	{
		GetMonobridge();

		MonoBridge.ClientConnections.OnCreated -= ClientConnectionCreated;
		MonoBridge.ClientConnections.OnDestroyed -= ClientConnectionsDestroyed;
	}

	void ClientConnectionCreated(CoherenceClientConnection connection)
	{
		// when there are the minimum nuimber of players and the game hasn't started, then get the lobby started
		if (MonoBridge.ClientConnections.ClientConnectionCount >= numberOfPlayersToStartGame && !hasGameStarted)
		{
			float prev = waitTimer;
			waitTimer = waitTime;
			OnWaitTimerChanged(prev, waitTimer);

			isGameReadyToStart = true;
			OnGameReadyToStartUpdate(false, isGameReadyToStart);
		}

		// first player to join the room reset's the lobby
		else if(MonoBridge.ClientConnections.ClientConnectionCount == 1)
		{
			ResetLobby();
		}
	}

	void ClientConnectionsDestroyed(CoherenceClientConnection connection)
	{
		// as long as the game hasn't started, we can attempt to interrupt
		if (MonoBridge.ClientConnections.ClientConnectionCount < numberOfPlayersToStartGame && !hasGameStarted)
		{
			// Add a pause message
			isGameReadyToStart = false;
			OnGameReadyToStartUpdate(false, isGameReadyToStart);
		}
	}

	void Update()
	{
		if(isGameReadyToStart) 
		{ 
			WaitUpdate();
			return;
		}
		
		if(hasGameStarted)
		{
			GameUpdate();
			return;
		}
	}

	void WaitUpdate() 
	{
		float prev = waitTimer;
		waitTimer -= Time.deltaTime;
		OnWaitTimerChanged(prev, waitTimer);

		if (waitTimer <= 0)
		{
			isGameReadyToStart = false;
			OnGameReadyToStartUpdate(true, isGameReadyToStart);

			hasGameStarted = true;
			OnGameStartedUpdate(false, hasGameStarted);

			prev = gameTimer;
			gameTimer = gameTime;
			OnGameTimerChanged(prev, gameTimer);
		}
	}

	void GameUpdate() 
	{
		if (!CurrentObjective)
		{
			// pick a random spawn point
			var spawnPoint = DecryptableSpawnPoints[Random.Range(0, DecryptableSpawnPoints.Count)];

			// spawn a new objective for the player
			CurrentObjective = Instantiate(DecryptablePrefab, spawnPoint.position, Quaternion.identity);
			CurrentObjectiveSync = CurrentObjective.GetComponent<CoherenceSync>();
		}

		if(gameTimer > 0)
		{
			float prev = gameTimer;
			gameTimer -= Time.deltaTime;
			gameTimer = Mathf.Clamp(gameTimer, 0, gameTime);
			OnGameTimerChanged(prev, gameTimer);
		}

		if(gameTimer <= 0)
		{
			// find winner print and restart lobby
			var players = FindObjectsOfType<PlayerName>();

			int highscore = 0;
			winner = "";
			
			foreach(var player in players)
			{
				if(player.CurrentScore > highscore)
				{
					highscore = player.CurrentScore;
					winner = player.playerName;
				}
			}

			// THIS WHOLE THING CAN BE SUPER OPTIMIZED BUT I GOT NO TIME
			OnWinnerChanged(null, winner);

			hasGameStarted = false;
			var wait = new WaitAction(5.0f);
			var restart = new LambdaAction(() => ResetLobby());
			var sequence = new SequenceAction();
			sequence.AddAction(wait);
			sequence.AddAction(restart);
			sequence.MarkReady();
		}
	}

	void GetMonobridge()
	{
		// if the monobridge is null, then look for it
		if (MonoBridge is null)
		{
			CoherenceMonoBridge monobridge;
			if (!MonoBridgeStore.TryGetBridge(gameObject.scene, out monobridge))
			{
				Debug.LogWarning("No monobridge available");
			}

			MonoBridge = monobridge;
		}
	}

	public void ResetLobby()
	{
		// adjust if the game is ready to start
		GetMonobridge();
		bool prev_b = isGameReadyToStart;
		isGameReadyToStart = MonoBridge.ClientConnections.ClientConnectionCount >= numberOfPlayersToStartGame;
		OnGameReadyToStartUpdate(prev_b, isGameReadyToStart);

		// adjust other values as well
		prev_b = hasGameStarted;
		hasGameStarted = false;
		OnGameStartedUpdate(prev_b, hasGameStarted);

		float prev = waitTimer;
		waitTimer = isGameReadyToStart ? 15 : 0;
		OnWaitTimerChanged(prev, waitTimer);

		prev = gameTimer;
		gameTimer = 0;
		OnWaitTimerChanged(prev, gameTimer);

		winner = "";
		OnWinnerChanged(null, winner);

		// destroy current objectives
		if(CurrentObjectiveSync)
		{
			Destroy(CurrentObjectiveSync);
			CurrentObjectiveSync = null;
		}

		// reset player scores
		var players = FindObjectsOfType<PlayerName>();
		foreach(var player in players)
		{
			player.CurrentScore = 0;
		}
	}

	public void OnGameReadyToStartUpdate(bool old, bool curr)
	{
		if(old == curr) { return; }
		if(!curr) 
		{
			UIManager.Instance.ShowPauseMessage();
		}
	}

	public void OnWaitTimerChanged(float old, float curr)
	{
		curr = Mathf.Clamp(curr, 0, waitTime);
		UIManager.Instance.UpdateWaitTimer(curr);
	}

	public void OnGameStartedUpdate(bool old, bool curr)
	{
		UIManager.Instance.ShowWaitTimer(!curr);
	}

	public void OnGameTimerChanged(float old, float curr)
	{
		UIManager.Instance.UpdateGameTimer(curr);
	}

	public void OnWinnerChanged(string old, string curr)
	{
		UIManager.Instance.UpdateWinner(curr);
	}
}
