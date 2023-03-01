using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2Fix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LateStart());
    }

    IEnumerator LateStart() {
        yield return new WaitForSeconds(Mathf.Epsilon);
        PlayerMechanics.Instance.unlockedTimeTraversal = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
