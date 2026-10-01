using UnityEngine;

public class Acceleration : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boule")) {
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();

            // Inspiré de mon Trou.cs mini-golf
            other.gameObject.GetComponent<Boule>().AjouterCharge();
        }
    }
}
