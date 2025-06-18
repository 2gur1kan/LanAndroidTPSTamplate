using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    public static ScoreboardManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    [SerializeField] private GameObject playerEntryPrefab;

    private class PlayerInfo
    {
        public string playerName;
        public Transform playerTransform;

        public float Height => playerTransform.position.y;
    }

    private readonly List<PlayerInfo> players = new List<PlayerInfo>();
    private readonly List<TextMeshProUGUI> boards = new List<TextMeshProUGUI>();

    public void RegisterPlayer(string name, Transform tr)
    {
        PlayerInfo pInfo = new PlayerInfo
        {
            playerName = name,
            playerTransform = tr,
        };

        players.Add(pInfo);

        TextMeshProUGUI newEntry = Instantiate(playerEntryPrefab, transform).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        newEntry.text = name;
        boards.Add(newEntry);
        newEntry.transform.parent.gameObject.SetActive(true);
    }

    private void Update()
    {
        UpdateScoreboard();
    }

    private void UpdateScoreboard()
    {
        players.Sort((a, b) => b.Height.CompareTo(a.Height));

        for (int i = 0; i < boards.Count; i++)
        {
            boards[i].text = players[i].playerName;
        }
    }

    public void RemovePlayer(Transform tr)
    {
        PlayerInfo p = players.Find(gg => gg.playerTransform == tr);

        if (p != null)
        {
            int index = players.IndexOf(p);

            if (index >= 0 && index < boards.Count)
            {
                Destroy(boards[index].transform.parent.gameObject);
                boards.RemoveAt(index);
                players.RemoveAt(index);
            }
        }
    }
}
