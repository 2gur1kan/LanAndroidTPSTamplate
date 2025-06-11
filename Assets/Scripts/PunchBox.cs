using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().PunchMe(transform.position);

            Debug.Log("Vallaha vurdum");
        }
    }
}
