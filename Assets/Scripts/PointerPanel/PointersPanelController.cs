using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointersPanelController : MonoBehaviour
{
    public static PointersPanelController Instance;

    [SerializeField] private GameObject EnemyPointer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    /// <summary>
    /// diðer oyuncular için pointer oluþturacak iþlev
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Pointer CreateEnemyPointer(Transform target)
    {
        Pointer BP = Instantiate(EnemyPointer).GetComponent<Pointer>();
        BP.transform.SetParent(transform, false);
        BP.Target = target;

        BP.gameObject.SetActive(true);

        return BP;
    }
}
