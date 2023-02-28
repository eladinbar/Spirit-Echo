using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DefaultNamespace;

public class level3 : MonoBehaviour
{
    [SerializeField] GameObject donny;
    [SerializeField] GameObject viver;

    [SerializeField] public GameObject player;
    [SerializeField] GameObject timeTraverseInstruction;
    [SerializeField] public AudioClip part1AudioClip;
    [SerializeField] AudioClip part2AudioClip;
    [SerializeField] AudioClip part3AudioClip;
    [SerializeField] GameObject Qbutton;
    [SerializeField] DisplayStoryText _displayStoryText;
    
    public bool isViverDied = false;
    bool isJumpEnabledInPresent = true;
    bool isJumpEnabledInPast = true;
    PlayerMechanics playermechanicsRemote;
    BoxCollider2D playerBoxCollider;
    bool init=false;
    int phase = 0, phase2=0, phase3=0;
    [SerializeField] GameObject pastTilemap;
    [SerializeField] public GameObject presentTilemap;

    bool got_time_traverse_instruction = false;
    float time_traverse_instruction = 8f;
    private int order = 0;

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
        isJumpEnabledInPresent = true;
        isJumpEnabledInPast=false;
        timeTraverseInstruction.SetActive(false);
        _displayStoryText.trigger.AddListener(trigger);
    }

    void trigger()
    {
        order++;
        
        switch (order)
        {
            case 1:
                ins1();
                break;
            case 2:
                ins2();
                break;
            case 3:
                ins3();
                break;
            case 4:
                ins4();
                break;
            case 5:
                ins5();
                break;

            
        }
    }
    void ins1()
    {
        donny.SetActive(false);
        playerBoxCollider.size = new Vector2((float)0.75, (float)0.12);

    }

    void ins2()
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 2.4f;
        presentTilemap.GetComponent<AudioSource>().clip = part2AudioClip;
        presentTilemap.GetComponent<AudioSource>().volume = 0.4f;
        presentTilemap.GetComponent<AudioSource>().Play();
    }
    void ins3()
    {
        viver.GetComponent<ViverMechanics>().canDie = true;

    }
    void ins4()
    {
    }
    void ins5()
    {
        
    }
    
    
    
    void OnTraverseTime() {
        if (order >= 3)
        {
            presentTilemap.GetComponent<AudioSource>().clip = part3AudioClip;

        }
        
        if (got_time_traverse_instruction)
        {
            timeTraverseInstruction.SetActive(false);
            Qbutton.SetActive(false);
        }
        got_time_traverse_instruction = true;

        
    }

    void OnJump() {
        bool isPast = pastTilemap.activeSelf;
        if(isPast){
            //phase+=1;
            playerBoxCollider.size = new Vector2((float)0.2,(float)0.12);

        }
        else{
            if(phase>=3)
                playerBoxCollider.size = new Vector2((float)0.75, (float)0.12);
            //if(phase2>0)
                //phase2+=1;
            //if(phase3>1)
                //phase3+=1;
        }

     
        if(phase==5){

            playermechanicsRemote.timeTraverseEnabled=true;
            playermechanicsRemote.jumpEnabled=true;
            isJumpEnabledInPast = true;

        }

        if(phase3==7){
            isJumpEnabledInPresent=true;
            playermechanicsRemote.jumpEnabled=true;
            viver.GetComponent<ViverMechanics>().canDie = true;
        }
    
        if(phase2==7){
            playermechanicsRemote.jumpEnabled=true;
            player.GetComponent<Rigidbody2D>().gravityScale = 2.4f;
            isJumpEnabledInPresent=true;
            presentTilemap.GetComponent<AudioSource>().clip = part2AudioClip;
            presentTilemap.GetComponent<AudioSource>().volume = 0.4f;
            presentTilemap.GetComponent<AudioSource>().Play();

        }
    

    }

    // Update is called once per frame
    void Update()
    {
        time_traverse_instruction -= Time.deltaTime;
        if (!got_time_traverse_instruction && time_traverse_instruction <= Mathf.Epsilon)
        {
            got_time_traverse_instruction = true;
            timeTraverseInstruction.SetActive(true);
            Qbutton.SetActive(true);
            
            
        }
        float distance = Vector3.Distance(player.transform.position, viver.transform.position);
        if(order==3 & !init &  distance > 21)
        {
            init = true;
            viver.transform.position=new Vector3((float)126,(float)85.45, (float)0);
        }
        if(phase3==1 &  distance < 4){
            playermechanicsRemote.jumpEnabled=false;
            isJumpEnabledInPresent=false;
            playermechanicsRemote.jumpEnabled=false;
            phase3=2;

        }


        
    }
}
