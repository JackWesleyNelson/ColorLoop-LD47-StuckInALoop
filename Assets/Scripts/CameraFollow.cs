using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform player = null;

    [SerializeField]
    private float smoothSpeed = 1;

    [SerializeField]
    private float distance = 10;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, -distance);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
    }
}
