using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract int Id { get; set; }
    public abstract string Name { get; set; }
    public abstract float Damage { get; set; }
    public abstract float Range { get; set; }
    public abstract int FireRate { get; set; }
    public abstract int MaxAmmo { get; set; }
    public abstract int Ammo { get; set; }
    public abstract Animator Animator { get; set; }
    public abstract float Weight { get; set; }
    private bool _isReady;
    
    public abstract void EquipWeapon();
    public abstract void HolsterWeapon();
}
