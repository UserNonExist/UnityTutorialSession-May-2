using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStateBehaivour : StateMachineBehaviour
{
    private InventorySystem _inventorySystem;
    private GameObject _weapon;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _inventorySystem = animator.GetComponentInParent<InventorySystem>();
        _weapon = _inventorySystem.currentWeapon;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _inventorySystem.isExitDone = true;
    }
}
