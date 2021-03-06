using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holster : MonoBehaviour
{
    [System.Flags]
    public enum ObjectsToHolster 
    { 
        None = 0, 
        Default = 1,
        Gun = 1 << 1, 
        Grenade = 1 << 2 
    }

    public ObjectsToHolster holster;
    public MeshRenderer holsterView = null;
    [SerializeField] Transform gunAnchor, grenadeAnchor;
    [SerializeField] bool isHolstering = false;
    AudioSource src = null;

    public bool IsHolstering => isHolstering;

    private void Start()
    {
        src = GetComponent<AudioSource>();
    }
    public void ToggleHolster()
    {
        src.Play();
        isHolstering = !isHolstering;
    }

    public void HolsterObject(GameObject objToHolster, GrabbableObj.ObjectType holsterType)
    {
        if (HolsterContains(holsterType))
        {
            switch (holsterType)
            {
                case GrabbableObj.ObjectType.GUN:
                    objToHolster.transform.position = gunAnchor.position;
                    objToHolster.transform.rotation = gunAnchor.rotation;
                    break;
                case GrabbableObj.ObjectType.GRENADE:
                    objToHolster.transform.position = grenadeAnchor.position;
                    objToHolster.transform.rotation = grenadeAnchor.rotation;
                    break;
            }
        }
        else
            Debug.LogWarning("Trying to snap to a holster that won't take it");
    }

    public bool HolsterContains(GrabbableObj.ObjectType objectType)
    {
        return (holster & (ObjectsToHolster)objectType) != 0;
    }
}
