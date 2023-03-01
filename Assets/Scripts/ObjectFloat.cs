using System.Collections;
using UnityEngine;

public class ObjectFloat : MonoBehaviour
{
    bool floatup;
    
    void Start(){
        floatup = false;
    }
    
    void Update(){
        if(floatup)
            StartCoroutine(floatingUp());
        else if(!floatup)
            StartCoroutine(floatingDown());
    }
    
    IEnumerator floatingUp() {
        Vector2 position = this.transform.position;
        transform.position = new Vector2(position.x, position.y + 0.3f * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        floatup = false;
    }
    
    IEnumerator floatingDown() {
        Vector2 position = this.transform.position;
        transform.position = new Vector2(position.x, position.y - 0.3f * Time.deltaTime);
        yield return new WaitForSeconds(0.5f);
        floatup = true;
    }
}
