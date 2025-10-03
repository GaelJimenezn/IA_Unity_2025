using System;
using System.Collections;
using UnityEngine;




/*
 * Las dos principales fortalezas de las corrutinas son
 * 1) Manejo de tiempo (cosas que se tienen que hacer después de X tiempo, o cada X tiempo)
 * 2) Manejo de secuencias de acciones (cosas que tienen un orden).
 * Entonces, si tiene un orden de secuencias de cosas que pasan cada X tiempo, pues mucho mejor.
 */


public class ExplicacionCorrutinas : MonoBehaviour
{
    private float transcurredTime = 0;
    public float timeToActivate = 5.0f;

    private Coroutine coroutineContarCincoSegundos;
    private Coroutine coroutineCountNSeconds;
    private Coroutine coroutineShoot;
    private Coroutine coroutineReload;
    
    IEnumerator ContarCincoSegundosUnaVez()
    {
        yield return new WaitForSeconds(timeToActivate);
            
        Debug.Log($"Ya pasaron {timeToActivate} segundos con la corrutina de una sola vez");
    }
    
    IEnumerator ContarCincoSegundos()
    {
        while (true)
        {
            // deja la ejecución del código de esta corrutina a partir de este punto, y reinicia desde aquí en el siguiente frame
            // yield return null; 
            // Deja pausada la ejecución de este código durante N-segundos.
            yield return new WaitForSeconds(timeToActivate);
            
            Debug.Log($"Ya pasaron {timeToActivate} segundos con la corrutina");
        }
    }
    
    IEnumerator CountNSeconds(float seconds)
    {
        while (true)
        {
            // deja la ejecución del código de esta corrutina a partir de este punto, y reinicia desde aquí en el siguiente frame
            // yield return null; 
            // Deja pausada la ejecución de este código durante N-segundos.
            yield return new WaitForSeconds(seconds);
            
            Debug.Log($"Ya pasaron {seconds} segundos con la corrutina de seconds");
        }
    }
    
    IEnumerator CountNSecondsXTimes(float seconds, int times)
    {
        while (times > 0)
        {
            // Deja pausada la ejecución de este código durante N-segundos.
            yield return new WaitForSeconds(seconds);
            
            Debug.Log($"Ya pasaron {seconds} segundos con la corrutina de seconds");
            times--;
        }
    }

    public bool canAttack = true;
    public int currentBulletsOnMagazine = 10;
    public int maxBulletsOnMagazine = 10;
    public float reloadTime = 2.5f;
    public float shootRate = 1.0f;

    
    IEnumerator Reload(float seconds)
    {
        Debug.Log("Comenzando recarga");
        // empezó a recargar, entonces no puede atacar
        canAttack = false;
        // hacemos una espera de lo que duraría la recarga del personaje.
        yield return new WaitForSeconds(seconds);
        // rellenamos las current bullets con las máximas que puede tener.
        currentBulletsOnMagazine = maxBulletsOnMagazine;
        // después de esos N segundos que dura la recarga, ya puede volver a atacar.
        canAttack = true;
        Debug.Log("terminando recarga");

        
        // Ya que acabó esta corrutina, ya podemos reiniciar la de Shoot.
        StartCoroutine(Shoot(shootRate));
    }

    IEnumerator Shoot(float seconds)
    {
        Debug.Log("Comenzando a disparar");

        while (true)
        {
            if (!canAttack)
                yield return null; //si ahorita no puede atacar, se espera a volver a checar en el siguiente frame.
                
            Debug.Log($"disparando, me quedan {currentBulletsOnMagazine} balas");
            currentBulletsOnMagazine--;
            
            if(currentBulletsOnMagazine == 0)
            {   
                // si después de disparar ya no tengo balas, recargo, en vez de esperarme al siguiente disparo.
                StartCoroutine(Reload(reloadTime));
                
                // después de mandar a llamar el reload, me salgo de esta corrutina de shoot,
                // porque de todos modos no puedo volver a disparar hasta que se acabe de ejecutar Reload.
                yield break; // esto es: Cancela completamente esta corrutina en la que estoy.
            }
            
            // me espero N segundos que es el tiempo entre disparo y disparo
            yield return new WaitForSeconds(seconds);
        }
    }


    private void CancelCoroutinesExample()
    {
        // Cancelar una corrutina
        // 1) Es la no-confiable, porque tú le dices que cancele UNA corrutina de ese nombre, no cuál de todas las posibles con ese nombre.
        StopCoroutine(ContarCincoSegundos()); // si está activa bueno, y si no, no.
        StopCoroutine(nameof(ContarCincoSegundos)); // esta es lo mismo pero con el string del nombre de la función.
        
        // 2) La sí-confiable, que requiere de una variable coroutine 
        if(coroutineContarCincoSegundos != null) // checas si sí está activa, y si sí, la detienes.
            StopCoroutine(coroutineContarCincoSegundos);
        // Prefieran siempre el método 2 y 3, el 1 no. El 1 se ve como mala práctica.
        
        // 3) es la de yield break; dentro de la corrutina misma que se va a cancelar.
        // véase el ejemplo de la corrutina de Shoot(). 
        
        // Detenemos todas las corrutinas de este script, por limpieza, disciplina y buena práctica.
        StopCoroutine(coroutineShoot);
        StopCoroutine(coroutineCountNSeconds);
    }

    private void OnDestroy()
    {
        CancelCoroutinesExample();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transcurredTime = 0;
        
        StartCoroutine(ContarCincoSegundosUnaVez());
        
        // Aguas, puedes mandar a llamar más de una vez la misma corrutina, y cada una de ellas es independiente.
        coroutineContarCincoSegundos = StartCoroutine(ContarCincoSegundos());
        
        coroutineCountNSeconds = StartCoroutine(CountNSeconds(3.1416f));
        
        StartCoroutine(CountNSecondsXTimes(2.0f, 3));

        coroutineShoot = StartCoroutine(Shoot(shootRate));

        // después de 4 segundos se autodestruya este objeto.
        Destroy(gameObject, 4.0f); // esto es: destrúyete, pero dentro de 4 segundos.
    }

    // Update is called once per frame
    void Update()
    {
        
        // transcurredTime += Time.deltaTime;
        // if (transcurredTime >= timeToActivate)
        // {
        //     transcurredTime = 0;
        //     Debug.Log($"Ya pasaron {timeToActivate} segundos");
        // }
    }
}
