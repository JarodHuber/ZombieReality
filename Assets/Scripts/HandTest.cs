using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandTest : MonoBehaviour
{
    [SerializeField] Animator handR;
    [SerializeField] Animator handL;

    void Update()
    {
        handR.SetBool("GripPressed", Keyboard.current.aKey.isPressed);
        handR.SetBool("TriggerPressed", Keyboard.current.sKey.isPressed);
        handR.SetBool("ThumbDown", Keyboard.current.dKey.isPressed);

        handL.SetBool("GripPressed", Keyboard.current.zKey.isPressed);
        handL.SetBool("TriggerPressed", Keyboard.current.xKey.isPressed);
        handL.SetBool("ThumbDown", Keyboard.current.cKey.isPressed);
    }
}
