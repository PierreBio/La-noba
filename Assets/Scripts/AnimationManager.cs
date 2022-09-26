using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    ClickableText _clickableText;

    [SerializeField] Animator _animator;
    string _currentState;

    const string IS_REPAIRING_YAK = "isRepairingYak";
    const string IS_BEACON_TRIGGERED = "isBeaconTriggered";
    const string NOAH_ARRIVED = "noahHasArrived";
    const string JERAI_IS_FACING_NOAH = "jeraiIsFacingNoah";
    const string JERAI_IS_IN_YAK = "jeraiIsInYak";

    const string BG_START_MOVING = "startMoving";

    const string YAK_IS_REPAIRED = "yakIsRepaired";

    const string YAK_IS_MOVING = "yakIsMoving";

    public void Awake()
    {
        _clickableText = FindObjectOfType<ClickableText>();
    }

    private void Update()
    {
        updateAnimations();
    }

    private void updateAnimations()
    {
        if (_clickableText != null && _clickableText.currentNode != null)
        {
            switch (_clickableText.currentNode.pid)
            {
                case 3: // Sortir du vaisseau. Jerai repare le vaisseau
                    
                    break;
                case 27: //Le vaisseau repart. Jerai monte dans le vaisseau
                    
                    break;
                case 7: //Allumer un beacon. Jerai allume un beacon
                    
                    break;
                case 10: // Noah arrive
                   
                break;
                case 100: // @TODO Jerai discute avec Noah. Jerai en face de Noah
                    
                break;
                case 34: // le vaisseau est réparer. On enlève la fumée
                case 17:
                    
                break;
                case 99: // @TODO Noah disparait de l'écran
                    
                break;
            }
        }
    }

    public void ChangeAnimationState(string newState)
    {
        if (newState == _currentState)
        {
            return;
        }

        _animator.Play(newState);
        _currentState = newState;
    }
    public float GetCurrentAnimationDuration()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).length;
    }
}
