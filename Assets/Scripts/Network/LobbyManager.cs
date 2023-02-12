using Coherence.Toolkit;
using Hibzz.Singletons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
	[Header("Lobby Properties")]
	[SerializeField] int numberOfPlayersToStartGame = 2;
	[SerializeField] float waitTime = 15.0f;

	[Header("Decryptable Objective Properties")]
	[SerializeField] GameObject objectToSpawn;

	public CoherenceMonoBridge MonoBridge { get; protected set; }

	[OnValueSynced(nameof(OnGameReadyToStartUpdate))]
	public bool isGameReadyToStart = false;

	[OnValueSynced(nameof(OnGameStartedUpdate))]
	public bool hasGameStarted = false;

	[OnValueSynced(nameof(OnWaitTimerChanged))]
	public float waitTimer = 0;

	public void OnStateAuthority()
	{
		GetMonobridge();

		MonoBridge.ClientConnections.OnCreated += ClientConnectionCreated;
		MonoBridge.ClientConnections.OnDestroyed += ClientConnectionsDestroyed;
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
		}
	}

	void GameUpdate() 
	{
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
}
