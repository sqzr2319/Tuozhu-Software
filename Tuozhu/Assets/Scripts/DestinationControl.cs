using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class DestinationControl : MonoBehaviour
{
    public GravityCalculation gravityCalculation;

    void Update()
    {
        gravityCalculation.destinationCalculation();
    }
}