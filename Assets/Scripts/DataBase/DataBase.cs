using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data Base", menuName = "Create Data Base")]
public class DataBase : ScriptableObject
{
    public string Name;

    public List<Weapon> weapons;
}

[System.Serializable]
public class Weapon
{
    public WeaponName name;
    public WeaponType type;
    public GameObject go;
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