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

    const string IS_REPAIRING_YAK = "isRepairingYak";
    const string IS_BEACON_TRIGGERED = "isBeaconTriggered";

    public void Awake()
    {
        clickableText = FindObjectOfType<ClickableText>();
    }

    private void Update()
    {
        updateAnimations();
    }

    private void updateAnimations()
    {
        if (clickableText != null && clickableText.currentNode != null)
        {
            Debug.Log(clickableText.currentNode.pid);
            Debug.Log(beaconAnimator.GetBool(IS_BEACON_TRIGGERED));
            switch (clickableText.currentNode.pid)
            {
                case 3: // Sortir du vaisseau. Jerai repare le vaisseau
                    jeraiAnimator.SetBool(IS_REPAIRING_YAK, true);
                    break;
                case 27: //Le vaisseau repart. Jerai monte dans le vaisseau
                    jeraiAnimator.SetBool(IS_REPAIRING_YAK, false);
                    break;
                case 7: //Allumer un beacon. Jerai allume un beacon
                    if (!beaconAnimator.GetBool(IS_BEACON_TRIGGERED))
                        beaconAnimator.SetBool(IS_BEACON_TRIGGERED, true);
                    break;
            }
        }
    }
}
