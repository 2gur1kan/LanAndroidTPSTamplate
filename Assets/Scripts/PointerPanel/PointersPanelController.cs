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
    /// Boss i�in pointer olu�turacak ve olu�turdu�u pointer � bossa g�nderecek
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

    /// <summary>
    /// Anvil i�in pointer olu�turacak ve olu�turdu�u pointer � bossa g�nderecek
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public Pointer CreateAnvilPointer(Transform target)
    {
        Pointer BP = Instantiate(EnemyPointer).GetComponent<Pointer>();
        BP.transform.parent = transform;
        BP.Target = target;

        BP.gameObject.SetActive(true);

        return BP;
    }
}
