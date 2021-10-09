using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hand : MonoBehaviour
{
    public InputActionMap input = new InputActionMap();
    [SerializeField] GameObject controller = null;
    [SerializeField] Animator controllerAnim = null;

    bool isGrabbing = false;

    public bool isPaused = false;

    GrabbableObj selectedObj = null;
    Rigidbody rb = null;

    public Rigidbody Rigidbody { get => rb; }
    Transform controllerOffset = null;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        controllerOffset = controller.transform.GetChild(0);

        input["Grab"].performed += AttemptGrab; // Swapped from started to performed w/o tests
        input["Grab"].canceled += AttemptRelease;

        input["Fire"].performed += AttemptTriggerPress;
        input["Fire"].canceled += AttemptTriggerRelease;

        input["ThumbHover"].performed += AttemptThumbHover;
        input["ThumbHover"].canceled += AttemptThumbRelease;

        input.Enable();
    }

    private void OnDisable()
    {
        input["Grab"].performed -= AttemptGrab; // Swapped from started to performed w/o tests
        input["Grab"].canceled -= AttemptRelease;

        input["Fire"].performed -= AttemptTriggerPress;
        input["Fire"].canceled -= AttemptTriggerRelease;

        input["ThumbHover"].performed -= AttemptThumbHover;
        input["ThumbHover"].canceled -= AttemptThumbRelease;

        input.Disable();
    }

    public void AttemptTriggerPress(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        controllerAnim.SetBool("TriggerPressed", true);
    }
    public void AttemptTriggerRelease(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        controllerAnim.SetBool("TriggerPressed", false);
    }
    public void AttemptThumbHover(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        controllerAnim.SetBool("ThumbDown", true);
    }
    public void AttemptThumbRelease(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        controllerAnim.SetBool("ThumbDown", false);
    }

    public void AttemptGrab(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        controllerAnim.SetBool("GripPressed", true);

        if (selectedObj)
        {
            if (selectedObj.IsGrabbed)
                selectedObj.SwapHands(this);
            else
                Grab();
        }
    }
    public void AttemptRelease(InputAction.CallbackContext context)
    {
        if (isGrabbing)
        {
            UnGrab();
            return;
        }

        controllerAnim.SetBool("GripPressed", false);
    }

    public void Grab()
    {
        controller.SetActive(false);
        isGrabbing = true;
        selectedObj.Grab(this);
    }

    public void UnGrab()
    {
        selectedObj.Release();
        selectedObj = null;

        //controller.transform.rotation = (transform.rotation * Quaternion.Inverse(controllerOffset.rotation)) * transform.rotation;
        //controller.transform.position += transform.position - controllerOffset.position;
        controller.SetActive(true);
        isGrabbing = false;
    }

    public void SwapHands()
    {
        controller.SetActive(!controller.activeInHierarchy);
        isGrabbing = !isGrabbing;
    }

    private void OnTriggerStay(Collider other)
    {
        if(!isGrabbing && other.CompareTag("GrabbableObject"))
            selectedObj = other.GetComponent<GrabbableObj>();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isGrabbing && selectedObj && other.gameObject == selectedObj.gameObject)
            selectedObj = null;
    }
}
