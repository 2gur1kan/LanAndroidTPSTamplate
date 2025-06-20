using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance;

    [SerializeField] private DataBase DB;

    public string Name { get => DB.Name; set => DB.Name = value; }
    public TeamName Team { get => DB.LocalTeam; set => DB.LocalTeam = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public Weapon GetWeapon(WeaponName WN) => DB.weapons.Find(gg => gg.name == WN);
    public GameObject GetPlayerPref(TeamName TN) => DB.playerPrefs.Find(gg => gg.team == TN)?.pref;

    public GameObject bulletHole { get => DB.BulletHole; }
}
