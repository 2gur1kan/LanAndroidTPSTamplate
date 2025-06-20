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

    public override void OnStartLocalPlayer()
    {
        PlayerInfo pInfo = new PlayerInfo
        {
            playerName = DataBaseManager.Instance.Name,
            player = null,
        };

        players.Add(pInfo);
        int index = players.IndexOf(pInfo);

        AssignPlayerToTeam();

        DataBaseManager.Instance.Team = players[index].team;
    }

    [Server]
    public void AssignPlayerToTeam()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].team = (i % 2 == 0) ? TeamName.B : TeamName.A;
        }
    }

    [Server]
    public void StartGame()
    {
        foreach (PlayerInfo p in players)
        {
            NetworkConnectionToClient conn = p.player.connectionToClient;
            GameObject prefab = DataBaseManager.Instance.GetPlayerPref(p.team);

            Vector3 spawnPos = GetSpawnPoint(p.team);
            GameObject newPlayer = Instantiate(prefab, spawnPos, Quaternion.identity);

            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, ReplacePlayerOptions.Destroy);// karakteri siler ve yenisini oluþturur

            p.player = newPlayer.GetComponent<Player>();
            p.player.TeamName = p.team;
        }
    }

    private Vector3 GetSpawnPoint(TeamName team)
    {
        return team == TeamName.A ? new Vector3(-5, 0, 0) : new Vector3(5, 0, 0);
    }

    public void RegisterPlayer(string name, Player gg)
    {
        PlayerInfo pInfo = new PlayerInfo
        {
            playerName = name,
            player = gg,
        };

        players.Add(pInfo);

        ScoreboardManager.Instance.SetEntry(name);

        ScoreboardManager.Instance.UpdateScoreboard();
    }

    public void RemovePlayer(Player player)
    {
        PlayerInfo p = players.Find(gg => gg.player == player);

        if (p != null)
        {
            int index = players.IndexOf(p);

            ScoreboardManager.Instance.RemovePlayer(index);

            players.RemoveAt(index);
        }
    }
}

public class PlayerInfo
{
    public string playerName;
    public Player player;
    public TeamName team;

    public float Score => player?.Score ?? 0f;
}
