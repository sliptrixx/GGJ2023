using Coherence.Runtime;
using Coherence.Toolkit;
using System.Threading.Tasks;
using UnityEngine;
using Hibzz.Singletons;

public class ConnectionManager : Singleton<ConnectionManager>
{
	const int k_MAX_PLAYERS = 4;

	CoherenceMonoBridge MonoBridge = null; // A reference to the monobridge

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

		// Fetch the local regions
		(bool success, string region) fetchResult = await PlayResolver.FetchLocalRegions();
		
		// check if the operation was successful
		if(!fetchResult.success)
		{
			Debug.Log("Failed to fetch local regions");
			return;
		}

		// continue by getting all room info
		var rooms = await PlayResolver.FetchRooms(fetchResult.region);
		if(rooms == null) 
		{ 
			Debug.Log("Server is down"); return;
		}

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
		await CreateAndJoin(fetchResult.region);
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
