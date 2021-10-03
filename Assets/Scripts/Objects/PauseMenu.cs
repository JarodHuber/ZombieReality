using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PushableButton button = null;
    [SerializeField] Transform pedestal = null;

    public void Initialize(float height, Vector3 forward, Vector3 playerPosition)
    {
        pedestal.localScale = new Vector3(pedestal.localScale.x, height, pedestal.localScale.z);

        transform.forward = forward;
        transform.position = playerPosition - (transform.rotation * (Vector3.forward * (height / 2.0f)));

        button.transform.position = new Vector3(button.transform.position.x, 
            pedestal.position.y + ((height + button.transform.localScale.y) / 2.0f), button.transform.position.z);

        button.Initialize();
    }
}
