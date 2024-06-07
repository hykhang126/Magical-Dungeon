using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GunBlueprint : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots, heatPerShot;
    public GameObject muzzleFlash;
}
