using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class level3 : MonoBehaviour
{
    [SerializeField] GameObject donny;
    [SerializeField] GameObject viver;
    [SerializeField] GameObject text1;
    [SerializeField] GameObject text2;
    [SerializeField] GameObject text3;
    [SerializeField] GameObject text4;
    [SerializeField] GameObject text5;
    [SerializeField] GameObject text1v;
    [SerializeField] GameObject text2v;
    [SerializeField] GameObject text3v;
    [SerializeField] GameObject text4v;
    [SerializeField] GameObject text5v;
    [SerializeField] GameObject text6v;
    [SerializeField] GameObject player;
    bool isViverDied;

    PlayerMechanics playermechanicsRemote;
    BoxCollider2D playerBoxCollider;
    bool isNewJumpEnabled= false;
    bool init=true;
    bool toLab= false;
    int phase = 0, phase2=0;
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
        
        playerBoxCollider.size = new Vector2((float)0.2,(float)0.12);

        donny.SetActive(false);
        text1.SetActive(false);
        text2.SetActive(false);
        text3.SetActive(false);
        text4.SetActive(false);
        text5.SetActive(false);
        text1v.SetActive(false);
        text2v.SetActive(false);
        text3v.SetActive(false);
        text4v.SetActive(false);
        text5v.SetActive(false);
        text6v.SetActive(false);



        
    }
    void OnTraverseTime() {
        toLab = !toLab;
        if(toLab == true){
            donny.SetActive(true);
            StartCoroutine(FadeIn(donny.GetComponent<SpriteRenderer>()));
            if (phase<5){
                playermechanicsRemote.jumpEnabled=false;
            }
            else if(phase2<7){
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
            if(isNewJumpEnabled)
                playerBoxCollider.size = new Vector2((float)0.75, (float)0.12);

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
            isNewJumpEnabled=true;
            playermechanicsRemote.jumpEnabled=true;
            text1v.SetActive(true);
        }






        if(phase2==7){
            phase2+=1;
            text6v.SetActive(false);
            playermechanicsRemote.jumpEnabled=true;
            player.GetComponent<Rigidbody2D>().gravityScale = 2.4f;

        }
        if(phase2==6){
            phase2+=1;
            text5v.SetActive(false);
            text6v.SetActive(true);

        }

        if(phase2==5){
            phase2+=1;
            text4v.SetActive(false);
            text5v.SetActive(true);

        }

        if(phase2==4){
            phase2+=1;
            text3v.SetActive(false);
            text4v.SetActive(true);

        }
        if(phase2==3){
            phase2+=1;
            text2v.SetActive(false);
            text3v.SetActive(true);

        }

        if(phase2==2){
            phase2+=1;
            text1v.SetActive(false);
            text2v.SetActive(true);

        }

    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, viver.transform.position);
        if(phase2==8 &  distance > 21){
            viver.transform.position=new Vector3((float)126,(float)86, (float)0);
            phase2+=1;
            }
        if(distance<2&init){
            init=false;
            phase2=2;
            playermechanicsRemote.jumpEnabled=false;
        }
        if(isViverDied){
            player.GetComponent<Rigidbody2D>().gravityScale = 1;

        }
        
    }
}
