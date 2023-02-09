using UnityEngine;

public class ScenePersist : MonoBehaviour {
    void Awake() {
        int gameScenePersists = FindObjectsOfType<ScenePersist>().Length;
        if(gameScenePersists > 1)
            Destroy(this.gameObject);
        else
            DontDestroyOnLoad(this.gameObject);
    }

    public void ResetScenePersist() {
        Destroy(this.gameObject);
    }
}