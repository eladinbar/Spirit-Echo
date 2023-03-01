using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
            PlayerMechanics.Instance.unlockedDoubleJump = true;
    }
}
