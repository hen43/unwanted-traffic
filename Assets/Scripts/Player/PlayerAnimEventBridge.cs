using UnityEngine;

public class PlayerAnimEventBridge : MonoBehaviour
{
    void Attack()
    {
        GetComponentInParent<PlayerCombat>().Attack();
    }
}
