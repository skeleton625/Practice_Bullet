using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour
{
    [Header("Bow Setting"), Space(20)]
    [SerializeField] private Bow bow;

    public void Shoot()
    {
        bow.Shoot();
    }
}
