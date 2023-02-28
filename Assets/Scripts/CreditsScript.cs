using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    // Start is called before the first frame update


    void Start() {
        Cursor.visible = true;
    }
    public void Return() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 6);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
