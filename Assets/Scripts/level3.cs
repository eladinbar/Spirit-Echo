using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DefaultNamespace;

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
    [SerializeField] GameObject text1v2;
    [SerializeField] GameObject text2v2;
    [SerializeField] GameObject text3v2;
    [SerializeField] GameObject text4v2;
    [SerializeField] GameObject text5v2;
    [SerializeField] public GameObject player;
    [SerializeField] GameObject timeTraverseInstruction;
    [SerializeField] public AudioClip part1AudioClip;
    [SerializeField] AudioClip part2AudioClip;
    [SerializeField] AudioClip part3AudioClip;

    
    public bool isViverDied = false;
    bool isJumpEnabledInPresent = true;
    bool isJumpEnabledInPast = true;
    PlayerMechanics playermechanicsRemote;
    BoxCollider2D playerBoxCollider;
    bool isNewJumpEnabled= false;
    bool init=true;
    int phase = 0, phase2=0, phase3=0;
    [SerializeField] GameObject pastTilemap;
    [SerializeField] public GameObject presentTilemap;

    bool got_time_traverse_instruction = false;
    float time_traverse_cooldown = 8f;
    private bool init1 = true;    


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
        text1v2.SetActive(false);
        text2v2.SetActive(false);
        text3v2.SetActive(false);
        text4v2.SetActive(false);
        text5v2.SetActive(false);
        isJumpEnabledInPresent = true;
        isJumpEnabledInPast=false;
        timeTraverseInstruction.SetActive(false);
        
    }
    void OnTraverseTime() {
        if (phase2 >= 7)
        {
            presentTilemap.GetComponent<AudioSource>().clip = part3AudioClip;

        }
        
        if (got_time_traverse_instruction)
        {
            timeTraverseInstruction.SetActive(false);
        }
        got_time_traverse_instruction = true;

        
    }

    void OnJump() {
        bool isPast = pastTilemap.activeSelf;
        if(isPast){
            phase+=1;
            playerBoxCollider.size = new Vector2((float)0.2,(float)0.12);

        }
        else{
            if(phase>=3)
                playerBoxCollider.size = new Vector2((float)0.75, (float)0.12);
            if(phase2>0)
                phase2+=1;
            if(phase3>1)
                phase3+=1;
        }

        if(phase==1){
            playermechanicsRemote.timeTraverseEnabled=false;
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
            playermechanicsRemote.timeTraverseEnabled=true;
            playermechanicsRemote.jumpEnabled=true;
            isJumpEnabledInPast = true;
            text1v.SetActive(true);
        }

        if(phase3==7){
            text4v2.SetActive(false);
            text5v2.SetActive(true);
            isJumpEnabledInPresent=true;
            playermechanicsRemote.jumpEnabled=true;
            viver.GetComponent<ViverMechanics>().canDie = true;
        }
        if(phase3==6){
            text3v2.SetActive(false);
            text4v2.SetActive(true);

        }
        if(phase3==5){
            text3v2.SetActive(false);
            text4v2.SetActive(true);

        }
        if(phase3==4){
            text2v2.SetActive(false);
            text3v2.SetActive(true);

        }
        if(phase3==3){
            text1v2.SetActive(false);
            text2v2.SetActive(true);

        }


        if(phase2==7){
            text6v.SetActive(false);
            playermechanicsRemote.jumpEnabled=true;
            player.GetComponent<Rigidbody2D>().gravityScale = 2.4f;
            isJumpEnabledInPresent=true;
            presentTilemap.GetComponent<AudioSource>().clip = part2AudioClip;
            presentTilemap.GetComponent<AudioSource>().volume = 0.4f;
            presentTilemap.GetComponent<AudioSource>().Play();

        }
        if(phase2==6){
            text5v.SetActive(false);
            text6v.SetActive(true);

        }

        if(phase2==5){
            text4v.SetActive(false);
            text5v.SetActive(true);

        }

        if(phase2==4){
            text3v.SetActive(false);
            text4v.SetActive(true);

        }
        if(phase2==3){
            text2v.SetActive(false);
            text3v.SetActive(true);

        }

        if(phase2==2){
            text1v.SetActive(false);
            text2v.SetActive(true);
            isJumpEnabledInPresent=false;

        }

    }

    // Update is called once per frame
    void Update()
    {
        time_traverse_cooldown -= Time.deltaTime;
        if (!got_time_traverse_instruction && time_traverse_cooldown <= Mathf.Epsilon)
        {
            got_time_traverse_instruction = true;
            timeTraverseInstruction.SetActive(true);
            
            
        }
        float distance = Vector3.Distance(player.transform.position, viver.transform.position);
        if(phase2>=8 && phase3==0 &  distance > 21)
        {
            viver.transform.position=new Vector3((float)126,(float)85.45, (float)0);
            phase3=1;
        }
        if(phase3==1 &  distance < 4){
            playermechanicsRemote.jumpEnabled=false;
            text1v2.SetActive(true);
            isJumpEnabledInPresent=false;
            playermechanicsRemote.jumpEnabled=false;
            phase3=2;

        }

        if(distance<2&init){
            init=false;
            phase2=1;
            playermechanicsRemote.jumpEnabled=false;
        }

        
    }
}
