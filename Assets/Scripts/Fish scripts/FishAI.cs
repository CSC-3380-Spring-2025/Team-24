using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class FishAI : MonoBehaviour
{
    //Events for ecosystem management
    public event Action<FishAI> OnFishDeath; //Event for when fish dies
    public event Action<FishAI> OnFishReproduce; //Event for when fish reproduces

    //Fish properties
    public FishType fishType; //Asigned by FishSpawner
    public SpawnRegion currentRegion; //Asigned by FishSpawner
    public Vector2 homePosition;
    public float age = 0f; //Age of the fish in minutes
    public float health = 1f; //Health of the fish, 1 is full health, 0 is dead
    public float hunger = 0f; //Hunger of the fish, 0 is full, 1 is starving
    public float maturity = 0f; //Maturity of the fish, 0 is immature, 1 is mature

    //Navigational properties
    public Vector2 velocity;
    public Transform currentLure = null;
    public float lureAttractionRadius = 5f; //Radius for lure attraction, fish will be attracted to lures within this radius

    //Components
    private SpriteRenderer spriteRenderer;
    Rigidbody2D rb;

    public void Initialize(FishType type, SpawnRegion region, Vector2 home)
    {
        fishType = type;
        currentRegion = region;
        homePosition = home;

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)); //Initialize velocity to a random direction

        lureAttractionRadius = fishType.lureAttractionRadius; //Set the lure attraction radius to the same as the neighbor radius 
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Initialize(fishType, currentRegion, homePosition); //Initialize fish properties
        if (fishType == null)
        {
            Debug.LogError("FishType is not assigned in FishAI. Please assign a FishType in the inspector.");
            return;
        }
    }

    private void FixedUpdate()
    {
        Vector2 acceleration = Vector2.zero;
        acceleration += Flock();
        acceleration += Wander();
        acceleration += HomeAttraction();
        acceleration += lureAttraction();

        velocity += acceleration * Time.fixedDeltaTime;
        velocity = Vector2.ClampMagnitude(velocity, fishType.maxSpeed);

        rb.MovePosition(rb.position +  velocity * Time.fixedDeltaTime);

        //Set the fish rotation to match the direction of movement
        if (velocity.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle);
        }

        //Set the direction of the fish to always be rightside up
        if (velocity.x < 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }

    private void Update()
    {
        UpdateLifeCycle(); //Update the life cycle of the fish
        CheckReproduction(); //Check if the fish can reproduce

    }

    private Vector2 Flock()
    {
        //Collections all neighbors within neighbor Radius
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, fishType.neighborRadius);
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        Vector2 separation = Vector2.zero;
        int count = 0;

        Vector2 forward = velocity; //Get the forward direction of the fish
        if (velocity.sqrMagnitude < 0.1f) //If the fish is not moving, set forward to a default direction
        {
            forward = Vector2.right;
        }


        //Goes though all neighbors within range
        foreach (Collider2D col in neighbors)
        {
            if (col.gameObject == gameObject)
                continue;

            FishAI other = col.GetComponent<FishAI>();
            
            if (other != null)
            {
                //Calculate the direction vector to the neighbor fish
                Vector2 dirToNeighbor = other.transform.position - transform.position;

                //Check if neighbor is within the field of view
                float angleToNeighbor = Vector2.Angle(forward, dirToNeighbor);
                if (angleToNeighbor > fishType.fieldOfView * 0.5f)
                {
                    continue;
                }

                if (dirToNeighbor.magnitude < fishType.separationDistance) //If the distance between other other fish and current fish is too small
                {
                    //Debug.Log($"Fish {gameObject.name} is too close to {other.gameObject.name}");
                    separation -= dirToNeighbor.normalized * dirToNeighbor.magnitude;
                }

                alignment += other.velocity; //alignment will be in direction that other fish is heading
                cohesion += (Vector2)other.transform.position; //Cohestion vector is in the direction of other fish
                
                count++;
            }
        }
        if (count > 0)
        {
            alignment = (alignment / count).normalized * fishType.maxSpeed - velocity;
            //alignment = Vector2.ClampMagnitude(alignment, maxForce);

            cohesion = ((cohesion / count) - (Vector2)transform.position).normalized * fishType.maxSpeed - velocity;
            //cohesion = Vector2.ClampMagnitude(cohesion, maxForce);

            separation = separation.normalized * fishType.maxSpeed - velocity;
            //separation = Vector2.ClampMagnitude(separation, maxForce);

        }
        
        return alignment * fishType.alignmentWeight + cohesion * fishType.cohesionWeight + separation * fishType.separationWeight;
    }


    //Source of random direction for fish  
    private Vector2 Wander()
    {
        Vector2 wanderForce = new Vector2 (UnityEngine.Random.Range(-1f,1f), UnityEngine.Random.Range(-1f,1f));
        wanderForce *= fishType.wanderWeight;
        return wanderForce; //Vector2.ClampMagnitude(wanderForce, maxForce);
    }

    //This function keeps the fishies near their home point. 
    private Vector2 HomeAttraction()
    {
        Vector2 toHome = (homePosition - (Vector2)transform.position).normalized * fishType.maxSpeed - velocity;
        toHome *= fishType.homeAttractionWeight;
        return toHome; //Vector2.ClampMagnitude(toHome, maxForce);
    }

    private Vector2 lureAttraction()
    {
        if (currentLure == null)
        {
            GameObject lureObject = GameObject.FindWithTag("Lure");
            if (lureObject != null)
            {
                currentLure = lureObject.transform;
            }
        }
        else
        {
            if (currentLure.gameObject.CompareTag("OccupiedLure"))
            {
                currentLure = null;
                return Vector2.zero;
            }
        }

        if (currentLure != null)
        {
            float distanceTolure = Vector2.Distance((Vector2)transform.position, (Vector2)currentLure.position);

            if (distanceTolure <= lureAttractionRadius)
            {
                float attractionStrength = Mathf.Lerp(0.5f, 1.5f, 1 - (distanceTolure / lureAttractionRadius));
                Vector2 toLure = ((Vector2)currentLure.position - (Vector2)transform.position).normalized * fishType.maxSpeed - velocity;
                return toLure * attractionStrength;
            }
        }
        return Vector2.zero;
    }

    private void UpdateLifeCycle()
    {
        //Increment age
        age += Time.deltaTime / 60f; //Convert to minutes

        maturity = Mathf.Clamp01(age / fishType.maturityAge); //Clamp maturity between 0 and 1

        hunger += fishType.hungerRate * Time.deltaTime / 60f; //Increment hunger based on hunger rate
        transform.localScale = Vector3.Lerp(Vector3.one * 0.25f, Vector3.one, maturity); //Scale fish based on maturity

        if (age > fishType.maxAge) //If fish is older than max age, die
        {
            Die();
        }
    }

    //Function to handle fish reproduction
    public void CheckReproduction()
    {
       
        if (maturity >= 1f) //If fish is mature
        {
            float reproductionChance = fishType.reproductionRate; //Calculate chance of reproduction
           
            if (UnityEngine.Random.value < reproductionChance) //If chance is met
            {
                OnFishReproduce?.Invoke(this); //Invoke the reproduction event
            }
        }
    }


    public void Die()
    {
        OnFishDeath?.Invoke(this); //Invoke the death event
        Destroy(gameObject); //Destroy the fish object
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, fishType.neighborRadius); //Draws white circle around neigbor seeing radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fishType.separationDistance); //Draws red circle around separtion seeing radius
    }

}
