using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PushableButton button = null;
    [SerializeField] Transform pedestal = null;
    [SerializeField] float minDist = 1.0f;

    public void Initialize(float height, Vector3 forward, Vector3 playerPosition)
    {
        pedestal.localScale = new Vector3(pedestal.localScale.x, height, pedestal.localScale.z);

        transform.forward = forward;
        transform.position = playerPosition - (transform.rotation * (Vector3.forward * Mathf.Max((height / 2.0f), minDist)));

        button.transform.position = new Vector3(button.transform.position.x, 
            pedestal.position.y + ((height + button.transform.localScale.y) / 2.0f), 
            button.transform.position.z);

        button.Initialize();
    }
}
