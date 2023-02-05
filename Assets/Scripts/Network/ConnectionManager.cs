using Coherence.Runtime;
using Coherence.Toolkit;
using System.Threading.Tasks;
using UnityEngine;
using Hibzz.Singletons;
using Hibzz.DevMenu;
using Coherence.Connection;
using Coherence;
using System.Net;
using System.Collections.Generic;

public class ConnectionManager : Singleton<ConnectionManager>
{
	[SerializeField] bool EditorAutoConnect = false;

	const int k_MAX_PLAYERS = 4;

	public string PlayerName { get; protected set; } = "";
	public string CurrentRegion { get; protected set; }

	public List<string> Regions { get; protected set; }

	CoherenceMonoBridge MonoBridge = null; // A reference to the monobridge

	async void Start()
	{
		await RefreshRegions();

		#if UNITY_EDITOR
		if(EditorAutoConnect) { CreateConnection(); }
		#else
		EditorAutoConnect = false;
		#endif
	}

	public async Task RefreshRegions()
	{
		// When in local development mode, check if the local server is up and update the region
		if(RuntimeSettings.instance.LocalDevelopmentMode)
		{
			// Fetch the local regions
			(bool success, string region) fetchResult = await PlayResolver.FetchLocalRegions();

			// check if the operation was successful
			if (!fetchResult.success)
			{
				Debug.Log("Failed to fetch local regions");
				return;
			}

			// store the result in current region
			CurrentRegion = fetchResult.region;
			return;
		}

		// Load the list of regions from the cloud and update the list
		var regions = await PlayResolver.FetchRegions();
		Regions = new List<string>(regions);
		
		// validate that the process was successful
		if(Regions.Count <= 0) 
		{
			Debug.Log("Failed to fetch regions from the cloud");
			return;
		}

		// set the current region as the first in the result
		CurrentRegion = Regions[0];
		UIManager.Instance.UpdateServerList();
	}

	public async void CreateConnection()
	{
		// if the monobridge is null, then look for it
		if(MonoBridge is null)
		{
			if(!MonoBridgeStore.TryGetBridge(gameObject.scene, out MonoBridge))
			{
				Debug.LogWarning("No monobridge available");
			}
		}

		// continue by getting all room info
		var rooms = await PlayResolver.FetchRooms(CurrentRegion);
		if(rooms == null) 
		{ 
			Debug.Log("Server is down"); 
			return;
		}

		// the player for sure will join a room at this point, so...now would be a good
		// time to finalize the player name for this player
		PlayerName = EditorAutoConnect ? "Editor_Client" : UIManager.Instance.GetPlayerName();

		// look for any valid room to join
		foreach (var room in rooms)
		{
			if (room.ConnectedPlayers < k_MAX_PLAYERS)
			{
				MonoBridge.JoinRoom(room);
				return;
			}
		}

		// no valid room is available, so just join one
		await CreateAndJoin(CurrentRegion);
	}

	public string CycleServer()
	{
		// for the local development mode, we can't really switch servers
		if(RuntimeSettings.instance.LocalDevelopmentMode) { return "-"; }

		// if the regions is invalid, return "-"
		if(Regions is null || Regions.Count <= 0) { return "-"; }

		// get the current index and when cycled go to the next index
		int index = Regions.IndexOf(CurrentRegion);
		index++;
		if(index >= Regions.Count) 
		{ 
			index = 0; 
		}

		// update the current region and return it to the caller
		CurrentRegion = Regions[index];
		return CurrentRegion;
	}

	async Task CreateAndJoin(string region)
	{
		// create the room
		RoomCreationOptions options = new RoomCreationOptions();
		options.MaxClients = 4;
		var room = await PlayResolver.CreateRoom(region, options);

		// join the room
		MonoBridge.JoinRoom(room);
	}	
}
