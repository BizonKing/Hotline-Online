using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public Transform playerTransform;

    // How much to be far away from the player
    int depth = -20;

    // Update is called once per frame
    void Update()
    {

        if (playerTransform != null)
        {
            transform.position = playerTransform.position + new Vector3(0, 0, depth);
        }
    }

    public void setTarget(Transform target)
    {
        playerTransform = target;
    }
}
