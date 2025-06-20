using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data Base", menuName = "Create Data Base")]
public class DataBase : ScriptableObject
{
    public string Name;
    public TeamName LocalTeam;

    public List<Weapon> weapons;

    public GameObject BulletHole;

    public List<TeamPlayerPref> playerPrefs;
}

[System.Serializable]
public class Weapon
{
    public WeaponName name;
    public WeaponType type;
    public GameObject go;

    public float fireRate;
}

public enum WeaponName
{
    None,
    Pistol
}

public enum WeaponType
{
    None = 0,
    Pistol = 1,
    Rifle = 2
}

public enum TeamName
{
    A,
    B
}

[System.Serializable]
public class TeamPlayerPref
{
    public TeamName team;
    public GameObject pref;
}