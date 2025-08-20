using UnityEngine;

public class Oveja : MonoBehaviour
{

    public Rigidbody2D ribidbody2D;
    public float speed = 100;
    private Vector2 velocity;
    private Vector2 startPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        Rebote();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rebote()
    {
        transform.position = startPosition;
        ribidbody2D.linearVelocity = Vector2.zero;
        velocity.x = Random.Range(-1f, 1f);
        velocity.y = 1;
        ribidbody2D.AddForce(velocity * speed);

    }
}
