using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelPickup : MonoBehaviour
{
    [SerializeField] private int value = 50;
    [SerializeField] private float respawnTime = 2f;
    bool disabled;
    float lastDisabledAt;
    public int Value{get{return value;}}
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(disabled && Time.time > lastDisabledAt + respawnTime)
        {
            
            disabled = false;
        }
    }

    public IEnumerator Collect()
    {
        Debug.Log("disabling");
        enabled = false;
        foreach(var child in GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = false;
        }
        yield return new WaitForSeconds(respawnTime);
        foreach(var child in GetComponentsInChildren<MeshRenderer>())
        {
            child.enabled = true;
        }
        Debug.Log("enabling");
        enabled = true;
    }
    
}
