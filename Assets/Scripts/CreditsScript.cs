using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    void Start() {
        Cursor.visible = true;
    }
    
    public void Return() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 6);
    }
}
