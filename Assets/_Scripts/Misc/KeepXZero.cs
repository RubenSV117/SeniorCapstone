using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepXZero : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector2(0, transform.position.y);
    }
}
