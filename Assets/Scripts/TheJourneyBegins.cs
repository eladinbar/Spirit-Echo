using UnityEngine;
using UnityEngine.InputSystem;

public class TheJourneyBegins : MonoBehaviour {
    private PlayerInput playerInput;
    
    void Awake() {
        playerInput = PlayerMechanics.Instance.GetComponent<PlayerInput>();
    }
    
    void Start() {
        
    }
    
    void Update() {
        
    }
}
