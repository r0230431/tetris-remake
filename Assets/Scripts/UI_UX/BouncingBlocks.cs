using UnityEngine;

public class BouncingBlocks : MonoBehaviour
{
    [SerializeField]
    public GameObject[] blocks; // Array van tiles om te laten stuiteren
    [SerializeField]
    public float speed = 5f; // Snelheid van de tiles

   private void Start()
    {
        Rigidbody2D[] blocks = GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in blocks) // Voor elk blokje in de array
        {
            // Willekeurige richting voor elk blokje
            float xDir = Random.value < 0.5f ? -1f : 1f;
            float yDir = Random.value < 0.5f ? -1f : 1f;

            // Voeg wat variatie toe aan de snelheid
            float xSpeed = speed * Random.Range(0.8f, 1.2f) * xDir;
            float ySpeed = speed * Random.Range(0.8f, 1.2f) * yDir;

            // Stel de bewegingsnelheid van het blokje in
            rb.linearVelocity = new Vector2(xSpeed, ySpeed);
        }
    }
}