using UnityEngine;

public class AnimationRelay : MonoBehaviour
{

    private AnimationReceiver receiver; 

    private void Awake()
    {
        receiver = GetComponentInParent<AnimationReceiver>();

        if(receiver == null)
        {
            Debug.LogError($"AnimationRelay on {gameObject.name} could not find an AnimationReceiver script on its parent", this);
        }
    }

    public void TriggerEvent(string eventName)
    {
        receiver?.OnAnimationEvent(eventName);
    }
}
