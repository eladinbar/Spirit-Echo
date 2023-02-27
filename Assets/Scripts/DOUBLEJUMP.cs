using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOUBLEJUMP : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
            PlayerMechanics.Instance.unlockedDoubleJump = true;
    }
}
