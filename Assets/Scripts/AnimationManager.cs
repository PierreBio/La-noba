using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    ClickableText clickableText;

    [SerializeField] Animator yakAnimator;
    [SerializeField] Animator jeraiAnimator;
    [SerializeField] Animator beaconAnimator;
    [SerializeField] Animator noahAnimator;
    [SerializeField] Animator gliderAnimator;
    [SerializeField] Animator bgAnimator;

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
        clickableText = FindObjectOfType<ClickableText>();
    }

    void Start()
    {
        bgAnimator.SetBool(BG_START_MOVING, true);
    }

    private void Update()
    {
        updateAnimations();
    }

    private void updateAnimations()
    {
        if (clickableText != null && clickableText.currentNode != null)
        {
            switch (clickableText.currentNode.pid)
            {
                case 3: // Sortir du vaisseau. Jerai repare le vaisseau
                    jeraiAnimator.SetBool(IS_REPAIRING_YAK, true);
                    jeraiAnimator.SetBool(JERAI_IS_IN_YAK, false);
                    break;
                case 27: //Le vaisseau repart. Jerai monte dans le vaisseau
                    jeraiAnimator.SetBool(IS_REPAIRING_YAK, false);
                    jeraiAnimator.SetBool(JERAI_IS_IN_YAK, true);
                    gliderAnimator.SetBool(NOAH_ARRIVED, false);
                    noahAnimator.SetBool(NOAH_ARRIVED, false);
                    beaconAnimator.SetBool(IS_BEACON_TRIGGERED, false);
                    yakAnimator.SetBool(YAK_IS_MOVING, true);
                    yakAnimator.SetBool(YAK_IS_REPAIRED, false);
                    break;
                case 7: //Allumer un beacon. Jerai allume un beacon
                    if (!beaconAnimator.GetBool(IS_BEACON_TRIGGERED))
                        beaconAnimator.SetBool(IS_BEACON_TRIGGERED, true);
                    break;
                case 10: // Noah arrive
                    gliderAnimator.SetBool(NOAH_ARRIVED, true);
                    noahAnimator.SetBool(NOAH_ARRIVED, true);
                break;
                case 100: // @TODO Jerai discute avec Noah. Jerai en face de Noah
                    jeraiAnimator.SetBool(IS_REPAIRING_YAK, false);
                    jeraiAnimator.SetBool(JERAI_IS_FACING_NOAH, true);
                break;
                case 34: // le vaisseau est réparer. On enlève la fumée
                case 17:
                    yakAnimator.SetBool(YAK_IS_REPAIRED, true);
                break;
                case 99: // @TODO Noah disparait de l'écran
                    gliderAnimator.SetBool(NOAH_ARRIVED, false);
                    noahAnimator.SetBool(NOAH_ARRIVED, false);
                break;
            }
        }
    }
}
