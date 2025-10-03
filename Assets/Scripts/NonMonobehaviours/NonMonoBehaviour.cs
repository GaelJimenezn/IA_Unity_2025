using UnityEngine;


// La clave de las cosas que no heredan de monobehaviour es que las use algún monoBehaviour dentro de ellas.
public class NonMonoBehaviour 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        Debug.Log("Hola, soy una clase que no hereda de monobehaviour");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
