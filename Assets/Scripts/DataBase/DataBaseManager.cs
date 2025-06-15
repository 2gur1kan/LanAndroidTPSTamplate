using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseManager : MonoBehaviour
{
    public static DataBaseManager Instance;

    [SerializeField] private DataBase DB;

    public string Name { get => DB.Name; set => DB.Name = value; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public Weapon GetWeapon(WeaponName WN) => DB.weapons.Find(gg => gg.name == WN);
}
