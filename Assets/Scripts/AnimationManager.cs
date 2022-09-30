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


    // ANIMATION NAME
    const string SORTIR_DU_YAK = "Sortir du vaisseau";
    const string ALLUMER_BEACON = "Allumer un beacon";
    const string AIDE_PROVIDENTIELLE = "Noah arrive en aide providentielle";
    const string NOAH_REPARE_YAK = "Noah vous aide à réparer votre vaisseau";
    const string BUT_VOYAGE = "But réel du voyage";
    const string YAK_REPART = "Le vaisseau repart";
    const string QUESTION_NOAH = "Comment cette mémoire est-elle arrivée en la possession de Noah ?";

    const string TIRER_DOUCEMENT = "Tirer doucement";
    const string NOAH_RADAR = "Observer plus précisément";
    const string NOAH_STOP = "Noah nous barre la route";
    const string JERAI_SORT_YAK = "On sort du vaisseau pour discuter avec Noah";
    const string PRENDRE_MEMOIRE = "Prendre la mémoire";
    const string MEFIANCE_NOAH = "Méfiance envers Noah";

    // ANIMATIONS
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
            switch (_clickableText.currentNode.name)
            {
                case SORTIR_DU_YAK: // Sortir du vaisseau. Jerai repare le vaisseau
                    ChangeAnimationState(YAK_STOPPED_JERAI_APPEARS);
                    break;
                case TIRER_DOUCEMENT: //Le vaisseau repart. Jerai monte dans le vaisseau. Before 27
                    bool repairedYak = _clickableText.currentNode.text.Contains("repartir");
                    if (repairedYak)
                    {
                        _yakRepairedAlone = true;
                        ChangeAnimationState(YAK_REPAIRED_GOBACK);
                    }
                    break;
                case ALLUMER_BEACON: //Allumer un beacon. Jerai allume un beacon
                    _yakRepairedAlone = false;
                    ChangeAnimationState(YAK_STOPPED_JERAI_BEACON);
                    break;
                case AIDE_PROVIDENTIELLE: // Noah arrive après appel à l'aide
                    ChangeAnimationState(NOAH_ARRIVES_REPAIR);
                    break;
                case QUESTION_NOAH:
                case PRENDRE_MEMOIRE: // Noah fuit après avoir donné la cassette et réparé le yak
                    if (_yakRepairedAlone)
                    {
                        ChangeAnimationState(YAK_REPAIRED_NOAH_LEAVES);
                    }
                    else
                    {
                        ChangeAnimationState(NOAH_REPAIRED_YAK_AND_LEAVE);
                    }
                    break;
                case YAK_REPART: // Jerai retourne dans son yak après avoir reçu la cassette et Noah a réparé le yak
                    if (_yakRepairedAlone)
                    {
                        ChangeAnimationState(YAK_REPAIRED_TRAVEL_TO_JAHNAH);
                    }
                    else
                    {
                        ChangeAnimationState(NOAH_REPAIRED_TRAVEL_TO_JAHNAH);
                    }
                    break;
                case NOAH_RADAR: // Noah approche le yak en glider
                    ChangeAnimationState(YAK_REPAIRED_NOAH_CHASE);
                    break;
                case NOAH_STOP: // Jerai Stop le yak avant de descendre parler à Noah
                    ChangeAnimationState(YAK_REPAIRED_JERAI_STOPS_YAK);
                    break;
                case JERAI_SORT_YAK: // Jerai sort du yak après que Noah l'arrete
                    ChangeAnimationState(YAK_REPAIRED_JERAI_MEET_NOAH);
                    break;
                case NOAH_REPARE_YAK: // Moteur en cours de réparation par Noah
                    ChangeAnimationState(NOAH_REPAIR_YAK);
                    break;
                case MEFIANCE_NOAH:
                    if (_yakRepairedAlone == false)
                    {
                        ChangeAnimationState(NOAH_REPAIR_YAK);
                    }
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
