using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera : MonoBehaviour
{

    public Transform player;

    // Update is called once per frame
    void Update()
    {
        if (!OverworldManager.Instance.controllingCamera) transform.position = player.transform.position + new Vector3(0.0f, .1f, -5);
    }
}
