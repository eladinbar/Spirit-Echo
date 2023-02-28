using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace DefaultNamespace
{
    public class End_Screen : MonoBehaviour
    {
        void Start() {
            Cursor.visible = true;
        }
        public void Return() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 5);
        }

    }
}