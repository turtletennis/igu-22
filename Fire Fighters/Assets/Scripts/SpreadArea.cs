using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadArea : MonoBehaviour
{
    
    [SerializeField] float temperatureSpreadFactor = 0.0000001f;
    [SerializeField] float parentChangeFactor = 0.00000001f;
    Burnable parent;
    public float TempDiff{get; private set;}
    new Collider2D collider;
    HashSet<Burnable> objectsInRange = new HashSet<Burnable>();
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<Burnable>();
        collider = GetComponent<Collider2D>();
        var burnables = FindObjectsOfType<Burnable>();
        foreach(var burnable in burnables)
        {
            
            
            Vector3 otherPos = burnable.transform.position;
            if( collider.bounds.Contains(otherPos))
            {
                objectsInRange.Add(burnable);
            }
        
        }
    }

    void OnTriggerStay2D(Collider2D other) 
    {
        Burnable burnable = other.gameObject.GetComponent<Burnable>();
        if(burnable!=null && burnable != parent) objectsInRange.Add(burnable);
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        Burnable burnable = other.gameObject.GetComponent<Burnable>();
        if(burnable!=null) objectsInRange.Remove(burnable);
    }

    public void AddHeat(float amount)
    {
        TempDiff += amount * temperatureSpreadFactor;
    }

    // Update is called once per frame
    void Update()
    {
        if(objectsInRange.Count==0)
        {
            Debug.Log($"No objects in range for {name}");
        }
        if(!parent.Burnt)
        {

            foreach(var burnable in objectsInRange)
            {
                var size =  collider.bounds.size;
                float area = size.x * size.y;
                var deltaPos = transform.position - burnable.transform.position;
                
                var distanceFactor = (Mathf.Sqrt(area) - Vector3.Magnitude(deltaPos) )/Mathf.Sqrt(area) ;
                //Debug.Log($"distanceFactor = ({Mathf.Sqrt(area)} - {Vector3.Magnitude(deltaPos)} ) / {Mathf.Sqrt(area)} = {distanceFactor}");
                float delta = TempDiff *  distanceFactor * Time.deltaTime;
                if(burnable != parent)
                {
                    if(delta!=0) Debug.Log($"Changing temperature of {burnable.name} by {delta}");
                    burnable.ChangeTemperature(delta);
                }
                else
                {
                    burnable.ChangeTemperature(TempDiff * parentChangeFactor);
                }
            }
        }
    }
}
