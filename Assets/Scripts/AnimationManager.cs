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


    // NEW
    const string YAK_STOPPED_WITH_SMOKE = "sprites_yak_stopped_with_smoke";
    const string YAK_STOPPED_JERAI_APPEARS = "sprites_yak_stopped_jerai_appears";
    const string YAK_STOPPED_JERAI_BEACON = "sprites_jerai_alerts_beacon_start";
    const string YAK_REPAIRED_GOBACK = "sprites_jerai_goback_start";
    const string NOAH_ARRIVES_REPAIR = "sprites_yak_stopped_noah_appears_start";

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
                    ChangeAnimationState(YAK_STOPPED_JERAI_APPEARS);
                    break;
                case 32: //Le vaisseau repart. Jerai monte dans le vaisseau. Before 27
                    ChangeAnimationState(YAK_REPAIRED_GOBACK);
                    break;
                case 7: //Allumer un beacon. Jerai allume un beacon
                    ChangeAnimationState(YAK_STOPPED_JERAI_BEACON);
                    break;
                case 10: // Noah arrive après appel à l'aide
                    ChangeAnimationState(NOAH_ARRIVES_REPAIR);
                    break;

                case 18: // Moteur est réparé avec Noah
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
