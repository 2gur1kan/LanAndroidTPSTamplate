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
    private readonly List<TextMeshProUGUI> boards = new List<TextMeshProUGUI>();

    public void UpdateScoreboard()
    {
        List<PlayerInfo> gg = new(GameManager.Instance.players);

        gg.Sort((a, b) => b.Score.CompareTo(a.Score));

        for (int i = 0; i < boards.Count; i++)
        {
            boards[i].text = gg[i].playerName;
        }
    }

    public void SetEntry(string name)
    {
        TextMeshProUGUI newEntry = Instantiate(playerEntryPrefab, transform).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        newEntry.text = name;
        boards.Add(newEntry);
        newEntry.transform.parent.gameObject.SetActive(true);
    }

    public void RemovePlayer(int index)
    {
        if (index >= 0 && index < boards.Count)
        {
            Destroy(boards[index].transform.parent.gameObject);
            boards.RemoveAt(index);
        }
    }
}
