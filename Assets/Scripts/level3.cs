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
    bool init=false;
    [SerializeField] GameObject pastTilemap;
    [SerializeField] public GameObject presentTilemap;

    bool got_time_traverse_instruction = false;
    float time_traverse_instruction = 8f;
    private int order = 0;



    // Start is called before the first frame update
    void Start()
    {
        playermechanicsRemote = player.GetComponent<PlayerMechanics>();
        playerBoxCollider= player.GetComponent<BoxCollider2D>();
        donny.SetActive(false);
        timeTraverseInstruction.SetActive(false);
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
        if(order==2 & !init &  distance > 21)
        {
            init = true;
            viver.transform.position=new Vector3((float)126,(float)85.45, (float)0);
        }
    }
}
