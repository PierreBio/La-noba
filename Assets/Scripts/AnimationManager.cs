using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    ClickableText _clickableText;

    [SerializeField] Animator _animator;
    string _currentState;

    bool _yakRepairedAlone;

    // NEW
    const string YAK_STOPPED_WITH_SMOKE = "sprites_yak_stopped_with_smoke";
    const string YAK_STOPPED_JERAI_APPEARS = "sprites_yak_stopped_jerai_appears";
    const string YAK_STOPPED_JERAI_BEACON = "sprites_jerai_alerts_beacon_start";
    const string YAK_REPAIRED_GOBACK = "sprites_jerai_goback_start";
    const string NOAH_ARRIVES_REPAIR = "sprites_yak_stopped_noah_appears_start";
    const string NOAH_REPAIR_YAK = "sprites_yak_stopped_noah_repair_start";
    const string NOAH_REPAIRED_YAK_AND_LEAVE = "sprites_yak_stopped_noah_repaired_and_leave_start";
    const string NOAH_REPAIRED_TRAVEL_TO_JAHNAH = "sprites_yak_stopped_travel_to_jahnah_start";
    const string YAK_REPAIRED_NOAH_CHASE = "sprites_yak_repaired_noah_chase_yak_start";
    const string YAK_REPAIRED_JERAI_STOPS_YAK = "sprites_yak_repaired_jerai_stops_yak_start";
    const string YAK_REPAIRED_JERAI_MEET_NOAH = "sprites_yak_repaired_jerai_meet_noah_start";
    const string YAK_REPAIRED_NOAH_LEAVES = "sprites_yak_repaired_noah_leaves_start";
    const string YAK_REPAIRED_TRAVEL_TO_JAHNAH = "sprites_yak_repaired_yak_travels_to_jahnah_start";

    public void Start()
    {
        _yakRepairedAlone = false;
    }

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
                    _yakRepairedAlone = true;
                    ChangeAnimationState(YAK_REPAIRED_GOBACK);
                    break;
                case 7: //Allumer un beacon. Jerai allume un beacon
                    _yakRepairedAlone = false;
                    ChangeAnimationState(YAK_STOPPED_JERAI_BEACON);
                    break;
                case 10: // Noah arrive après appel à l'aide
                    ChangeAnimationState(NOAH_ARRIVES_REPAIR);
                    break;
                case 24: // Noah fuit après avoir donné la cassette et réparé le yak
                case 23: // Noah fuit après avoir donné la cassette et réparé le yak
                    if (_yakRepairedAlone)
                    {
                        ChangeAnimationState(YAK_REPAIRED_NOAH_LEAVES);
                    }
                    else
                    {
                        ChangeAnimationState(NOAH_REPAIRED_YAK_AND_LEAVE);
                    }
                    break;
                case 27: // Jerai retourne dans son yak après avoir reçu la cassette et Noah a réparé le yak
                    if (_yakRepairedAlone)
                    {
                        ChangeAnimationState(YAK_REPAIRED_TRAVEL_TO_JAHNAH);
                    }
                    else
                    {
                        ChangeAnimationState(NOAH_REPAIRED_TRAVEL_TO_JAHNAH);
                    }
                    break;
                case 43: // Noah approche le yak en glider
                    ChangeAnimationState(YAK_REPAIRED_NOAH_CHASE);
                    break;
                case 44: // Jerai Stop le yak avant de descendre parler à Noah
                    ChangeAnimationState(YAK_REPAIRED_JERAI_STOPS_YAK);
                    break;
                case 47: // Jerai sort du yak après que Noah l'arrete
                    ChangeAnimationState(YAK_REPAIRED_JERAI_MEET_NOAH);
                    break;
                case 14: // Noah aide Jerai à réparer le Yak
                    break;
                case 18: // Moteur en cours de réparation par Noah
                    ChangeAnimationState(NOAH_REPAIR_YAK);
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
