using System.Collections.Generic;
using UnityEngine;

public class TimeBody : MonoBehaviour {
    private bool isRewinding = false;
    
    Stack<Vector3> positions;
    Stack<Quaternion> rotations;
    Rigidbody2D rigidbody;
    
    void Start()    {
        positions = new Stack<Vector3>();
        rotations = new Stack<Quaternion>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if(isRewinding)
            Rewind();
        else
            Record();
    }

    public void StartRewind() {
        isRewinding = true;
        rigidbody.isKinematic = true;
    }

    public void StopRewind() {
        isRewinding = false;
        rigidbody.isKinematic = false;
    }

    void Rewind() {
        if (positions.Count > 0) {
            this.transform.position = positions.Pop();
            this.transform.rotation = rotations.Pop();
        }
        else
            Destroy(this.gameObject);
    }

    void Record() {
        positions.Push(this.transform.position);
        rotations.Push(this.transform.rotation);
    }
}
