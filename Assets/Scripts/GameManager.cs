using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public readonly SyncList<PlayerInfo> players = new SyncList<PlayerInfo>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [Server]
    public void RegisterPlayer(NetworkConnectionToClient conn, string playerName)
    {
        TeamName team = (players.Count % 2 == 0) ? TeamName.A : TeamName.B;

        players.Add(new PlayerInfo
        {
            connectionId = conn.connectionId,
            playerName = playerName,
            team = team
        });

        ScoreboardManager.Instance.SetEntry(playerName);
        ScoreboardManager.Instance.UpdateScoreboard();

        SpawnPlayer(conn, playerName, team);
    }

    [Server]
    private void SpawnPlayer(NetworkConnectionToClient conn, string playerName, TeamName team)
    {
        GameObject prefab = DataBaseManager.Instance.GetPlayerPref(team);
        Vector3 spawnPos = GetSpawnPoint(team);

        GameObject playerObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        NetworkServer.ReplacePlayerForConnection(conn, playerObj, ReplacePlayerOptions.Destroy);

        int gg = players.FindIndex(gg => gg.connectionId == conn.connectionId);

        players[gg].player = playerObj.GetComponent<Player>();
        players[gg].player.Name = playerName;
        players[gg].player.TeamName = team;
    }

    private Vector3 GetSpawnPoint(TeamName team)
    {
        return team == TeamName.A ? new Vector3(-5, 0, 0) : new Vector3(5, 0, 0);
    }

    [Server]
    private void ChangeTeam(NetworkConnectionToClient conn)
    {
        int index = players.FindIndex(gg => gg.connectionId == conn.connectionId);

        players[index].team = players[index].team == TeamName.A ? TeamName.B : TeamName.A;

        SpawnPlayer(conn, players[index].playerName, players[index].team);
    }
}

public class PlayerInfo
{
    public int connectionId;
    public string playerName;
    public Player player;
    public TeamName team;

    public float Score => player?.Score ?? 0f;
}
