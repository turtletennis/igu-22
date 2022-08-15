using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float temperatureDelta;
    [SerializeField] float lifetimeAfterCollision = 0.2f;
    
    bool active = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.tag == "ground")
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(!active) return;

        var burnable = other.gameObject.GetComponent<Burnable>();
        if(burnable != null)
        {
            active = false;
            burnable.ChangeTemperature(temperatureDelta);
            StartCoroutine(Kill());
            
        }
    }

    private IEnumerator Kill()
    {
        yield return new WaitForSeconds(lifetimeAfterCollision);
        
        Destroy(gameObject);
    }
}
