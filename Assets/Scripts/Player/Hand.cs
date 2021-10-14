using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BaseHand))]
public class Hand : MonoBehaviour
{
    [SerializeField] GameObject controller = null;

    BaseHand coreHand = null;

    bool isGrabbing = false;

    public bool isPaused = false;

    GrabbableObj selectedObj = null;
    Rigidbody rb = null;

    public Rigidbody Rigidbody { get => rb; }
    Transform controllerOffset = null;

    public InputActionMap InputMap { get => coreHand.input; }

    private void Start()
    {
        coreHand = GetComponent<BaseHand>();

        coreHand.input["GripPress"].performed += AttemptGrab; // Swapped from started to performed w/o tests
        coreHand.input["GripPress"].canceled += AttemptDrop;

        rb = GetComponent<Rigidbody>();
        controllerOffset = controller.transform.GetChild(0);
    }

    private void OnDestroy()
    {
        coreHand.input["GripPress"].performed -= AttemptGrab; // Swapped from started to performed w/o tests
        coreHand.input["GripPress"].canceled -= AttemptDrop;
    }

    public void AttemptGrab(InputAction.CallbackContext context)
    {
        if (isGrabbing)
            return;

        if (selectedObj)
        {
            if (selectedObj.IsGrabbed)
                selectedObj.SwapHands(this);
            else
                Grab();
        }
    }
    public void AttemptDrop(InputAction.CallbackContext context)
    {
        if (!isGrabbing)
            return;

        UnGrab();
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
