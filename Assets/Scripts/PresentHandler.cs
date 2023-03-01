using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class PresentHandler : MonoBehaviour {
    private TilemapRenderer rend;
    private CinemachineConfiner cinemachineConfiner;
    private AudioSource audioSource;
    private bool isFirstCreated = true;
    
    [Header("Children")] 
    [SerializeField] GameObject backgroundTilemap;
    private PolygonCollider2D boundingBox;
    private TilemapRenderer backgroundRend;

    [SerializeField] CinemachineVirtualCamera virtualCamera;

    private void Awake() {
        rend = GetComponent<TilemapRenderer>();
        boundingBox = backgroundTilemap.GetComponent<PolygonCollider2D>();
        backgroundRend = backgroundTilemap.GetComponent<TilemapRenderer>();
        cinemachineConfiner = virtualCamera.GetComponent<CinemachineConfiner>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        isFirstCreated = false;
    }
    
    private IEnumerator FadeIn(Renderer render) {
        for (float opacity = 0.05f; opacity <= 1f; opacity += 0.05f) {
            Material material = render.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator FadeOut(Renderer render) {
        for (float opacity = 1f; opacity >= 0f; opacity -= 0.05f) {
            Material material = render.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
        this.gameObject.SetActive(false);
    }

    public void OnEnable() {
        if (!isFirstCreated) {
            StartCoroutine(FadeIn(this.rend));
            
            backgroundTilemap.SetActive(true);
            cinemachineConfiner.m_BoundingShape2D = this.boundingBox;
            StartCoroutine(FadeIn(backgroundRend));
            
            foreach (Transform child in this.gameObject.transform) {
                Renderer render = child.GetComponent<Renderer>();
                if (!render)
                    child.gameObject.SetActive(true);
                else if (!child.gameObject.CompareTag("Background")) {
                    child.gameObject.SetActive(true);
                    StartCoroutine(FadeIn(render));
                }
            }

            audioSource.Play();
        } else {
            cinemachineConfiner.m_BoundingShape2D = this.boundingBox;
            virtualCamera.m_Lens.OrthographicSize = 3.5f;
            cinemachineConfiner.m_ConfineScreenEdges = true;
        }
    }
    
    public void StartFading() {
        foreach (Transform child in this.gameObject.transform) {
            Renderer render = child.GetComponent<Renderer>();
            if (!render)
                child.gameObject.SetActive(false);
            else if (!child.gameObject.CompareTag("Background"))
                StartCoroutine(FadeOut(render));
        }
        StartCoroutine(FadeOut(backgroundRend));
        StartCoroutine(FadeOut(this.rend));
        
        audioSource.Stop();
    }
}
