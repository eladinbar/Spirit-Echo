using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class level3 : MonoBehaviour
{
    [SerializeField] GameObject donny;
    [SerializeField] GameObject text1;
    [SerializeField] GameObject text2;
    [SerializeField] GameObject text3;
    [SerializeField] GameObject text4;
    [SerializeField] GameObject text5;
    [SerializeField] GameObject player;
    PlayerMechanics playermechanicsRemote;
    BoxCollider2D playerBoxCollider;

    bool toLab= false;
    int phase = 0;
    [SerializeField] GameObject pastTilemap;






    private IEnumerator FadeIn(Renderer render) {
        for (float opacity = 0.05f; opacity <= 1f; opacity += 0.05f) {
            Material material = render.material;
            Color color = material.color;
            color.a = opacity;
            material.color = color;
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playermechanicsRemote = player.GetComponent<PlayerMechanics>();
        playerBoxCollider= player.GetComponent<BoxCollider2D>();









        donny.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5.SetActive(false);


        
    }
    void OnTraverseTime() {
        toLab = !toLab;
        if(toLab == true){
            donny.SetActive(true);
            StartCoroutine(FadeIn(donny.GetComponent<SpriteRenderer>()));
            if (phase<5){
                playermechanicsRemote.jumpEnabled=false;
            }
            else{
                playermechanicsRemote.jumpEnabled=true;
            }
            if(phase==1)
            {
                System.Threading.Thread.Sleep(2500);
                text1.SetActive(true);
            }
            if(phase==2)
                text2.SetActive(true);
            if(phase==3)
                text3.SetActive(true);
            if(phase==4)
                text4.SetActive(true);
            if(phase==5){
                text5.SetActive(true);
            }
        }
        else{
            playermechanicsRemote.jumpEnabled=true;
            donny.SetActive(false);
            text1.SetActive(false);
            text2.SetActive(false);
            text3.SetActive(false);
            text4.SetActive(false);
            text5.SetActive(false);
        }        




    }

    void OnJump() {
        bool isPast = pastTilemap.activeSelf;
        if(isPast)
            phase+=1;

        if(phase==1){
            text1.SetActive(true);
        }
        if(phase==2){
            text1.SetActive(false);
            text2.SetActive(true);

        }
        if(phase==3){
            text2.SetActive(false);
            text3.SetActive(true);
        }
        if(phase==4){
            text3.SetActive(false);
            text4.SetActive(true);
        }
        if(phase==5){
            text4.SetActive(false);
            text5.SetActive(true);
            playermechanicsRemote.jumpEnabled=true;


        }


        //StartCoroutine(FadeIn(donny.GetComponent<SpriteRenderer>()));


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
