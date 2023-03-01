using System.Collections;
using UnityEngine;
using DefaultNamespace;

public class Level3 : MonoBehaviour
{
    [SerializeField] GameObject donny;
    [SerializeField] GameObject viver;
    [SerializeField] public GameObject player;
    [SerializeField] GameObject timeTraverseInstruction;
    [SerializeField] public AudioClip part1AudioClip;
    [SerializeField] AudioClip part2AudioClip;
    [SerializeField] AudioClip part3AudioClip;
    [SerializeField] GameObject Qbutton;
    PlayerMechanics playermechanicsRemote;
    BoxCollider2D playerBoxCollider;
    [SerializeField] GameObject pastTilemap;
    [SerializeField] public GameObject presentTilemap;

    bool got_time_traverse_instruction = false;
    float time_traverse_instruction = 8f;
    private int order = 0;

    
    void Start() {
        playermechanicsRemote = player.GetComponent<PlayerMechanics>();
        playerBoxCollider= player.GetComponent<BoxCollider2D>();
        donny.SetActive(false);
        timeTraverseInstruction.SetActive(false);
        StartCoroutine(LateStart());
    }
    
    IEnumerator LateStart() {
        yield return new WaitForSeconds(Mathf.Epsilon);
        PlayerMechanics.Instance.unlockedWallClimb = false;
        playerBoxCollider.size = new Vector2(0.2f, 0.12f);
    }

    public void trigger()
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
        }
    }
    
    void ins1()
    {
        donny.SetActive(false);
        playerBoxCollider.size = new Vector2(0.75f, 0.12f);
        PlayerMechanics.Instance.unlockedWallClimb = true;

    }

    void ins2()
    {
        player.GetComponent<Rigidbody2D>().gravityScale = 2.4f;
        presentTilemap.GetComponent<AudioSource>().clip = part2AudioClip;
        presentTilemap.GetComponent<AudioSource>().Play();
    }
    void ins3()
    {
        viver.transform.position=new Vector3(126f,85.45f, 0f);

    }
    
    void ins4()
    {
        viver.GetComponent<ViverMechanics>().canDie = true;

    }
    
    void OnTraverseTime() {
        if (order >= 2)
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

    
    void Update() {
        time_traverse_instruction -= Time.deltaTime;
        if (!got_time_traverse_instruction && time_traverse_instruction <= Mathf.Epsilon)
        {
            got_time_traverse_instruction = true;
            timeTraverseInstruction.SetActive(true);
            Qbutton.SetActive(true);
        }
        

    }
}
