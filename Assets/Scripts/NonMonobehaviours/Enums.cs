using UnityEngine;



// Enum 
public enum ESteeringBehaviors : byte
{
    DontMove,
    Seek,
    Flee,
    Pursuit, // qué valor tendría pursuit si flee=42? si a uno tú le das valor específico, pero al siguiente no, entonces es el valor siguiente de dicho valor asignado. En este caso, 43.
    Evade,
    Arrive,
}

// sirve como bits para una máscara de bits.
public enum ELayer
{
    Default = 1, // [0000 0001] 0 en binario
    Enemy = 2, // [0000 0010] 2 en binario
    Player = 4, // [0000 0100] 4 en binario
    EnemyBullet = 8, // [0000 1000] 8 en binario
    PlayerBullet = 16, // [0001 0000] 16 en binario
    Wall = 32,
}

// los enum tiene un tipo de dato subyacente.
// si tu enumeración no va a llegar a valores muy altos, usa el tipo de dato de tamaño suficiente para 
// contener todas las cosas que vas a enumerar.
public enum EInt : int
{
     // cada variable del tipo "EInt" pesa lo mismo que un entero (4 bytes).
}

public enum EShort : short
{
    // cada variable del tipo "EShort" pesa lo mismo que un short (2 bytes).
}

public enum EByte : byte
{
    // cada variable del tipo "EByte" pesa lo mismo que un byte (1 byte).
}

// ejemplos rápidos:
// En minecraft hay muchísimos tipos de cosas que puedes llevar en tu inventario, probablemente más de 256 tipos,
// entonces tú no usarías una enumeración de tipo byte, porque solo llega hasta 256, tendrías que usar una de short,
// que llega hasta 2^16


public class Enums
{
    
}
