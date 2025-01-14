using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShip : MonoBehaviour
{

    // [SerializeField] private EnemyDamageEventChannel damageEventChannel;  // Reference to the damage event channel
    [SerializeField] private EnemyShipColor shipColor;  // The color of the ship (enum)
    [SerializeField] private EnemyShipType shipType;  // The type of the ship (enum)
    [SerializeField] private EnemyCommander commander;  // The commander of the ship 

    // Start is called before the first frame update
    void Start()
    {
        if (commander == null)
        {
            commander = FindObjectOfType<EnemyCommander>();
        }
    }

}
