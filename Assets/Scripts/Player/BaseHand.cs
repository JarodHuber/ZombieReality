using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseHand : MonoBehaviour
{
    public InputActionMap input = new InputActionMap();
    [SerializeField] Animator controllerAnim = null;

    private void OnEnable()
    {

        input["GripPress"].performed += AttemptGripPress; 
        input["GripPress"].canceled += AttemptGripRelease;

        input["TriggerPress"].performed += AttemptTriggerPress;
        input["TriggerPress"].canceled += AttemptTriggerRelease;

        input["ThumbHover"].performed += AttemptThumbHover;
        input["ThumbHover"].canceled += AttemptThumbRelease;

        input.Enable();
    }

    private void OnDisable()
    {
        input["GripPress"].performed -= AttemptGripPress; 
        input["GripPress"].canceled -= AttemptGripRelease;

        input["TriggerPress"].performed -= AttemptTriggerPress;
        input["TriggerPress"].canceled -= AttemptTriggerRelease;

        input["ThumbHover"].performed -= AttemptThumbHover;
        input["ThumbHover"].canceled -= AttemptThumbRelease;

        input.Disable();
    }

    public virtual void AttemptTriggerPress(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("TriggerPressed", true);
    }
    public virtual void AttemptTriggerRelease(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("TriggerPressed", false);
    }
    public virtual void AttemptThumbHover(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("ThumbDown", true);
    }
    public virtual void AttemptThumbRelease(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("ThumbDown", false);
    }
    public virtual void AttemptGripPress(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("GripPressed", true);
    }
    public virtual void AttemptGripRelease(InputAction.CallbackContext context)
    {
        controllerAnim.SetBool("GripPressed", false);
    }
}
