using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Collections.Specialized;

public class BoatFishing : MonoBehaviour
{
    private GameObject player;
    public bool startBoat = false;
    public bool boatMoving = false;
    public bool move;

    private Rigidbody2D rb;
    private Animator camAnimator;
    public float speed;

    public string sceneName;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        camAnimator = Camera.main.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (startBoat && Input.GetKeyDown(KeyCode.E) && !boatMoving)
        {
            boatMoving = true;
            player.GetComponent<PlayerScript>().Boat();

            StartCoroutine(SailAway());
        }

        if (move) 
        {
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * speed;
            rb.linearVelocity = Vector2.down * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player") 
        {
            startBoat = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            startBoat = false;  
        }
    }

    IEnumerator SailAway() 
    {
        yield return new WaitForSeconds(0.5f);
        player.transform.parent = transform;
        move = true;

        yield return new WaitForSeconds(1.3f);
        camAnimator.SetBool("Fade", true);
        yield return new WaitForSeconds(.7f);
        SceneManager.LoadScene(sceneName);
    }
}
