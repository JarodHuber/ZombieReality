using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class LightTrigger : MonoBehaviour
{
    [SerializeField] HDAdditionalLightData[] lightData;

    bool called = false;
    private void LateUpdate()
    {
        called = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (called/* && !other.CompareTag("Player") && !other.CompareTag("Enemy")*/)
            return;

        called = true;
        for (int x = 0; x < lightData.Length; ++x)
            lightData[x].RequestShadowMapRendering();
    }
}
