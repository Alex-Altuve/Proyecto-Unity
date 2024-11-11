using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectedColliders = new List<Collider2D>();
    Collider2D col;
    // Start is called before the first frame update
  
    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("si detecta algo");
        detectedColliders.Add(collision);
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        detectedColliders.Remove(collision);
    }
}
