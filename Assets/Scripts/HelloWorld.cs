using UnityEngine;
using UnityEngine.AI;

public class HelloWorld : MonoBehaviour
{
    private NonMonoBehaviour _nonMonoBehaviour;
    
    public Vector3 velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        velocity += new Vector3(0.0f, -9.81f, 0.0f) * Time.deltaTime;
        
        transform.position += velocity * Time.deltaTime;
        
      //transform.position += new Vector3(1, 0, 0) * Time.deltaTime;;
    }
    
    private void FixedUpdate()
    {
    
    }
}


