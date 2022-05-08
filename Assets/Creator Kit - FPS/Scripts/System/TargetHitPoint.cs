using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitPoint : MonoBehaviour
{
    [SerializeField] Target target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Got(float damage)
    {
        if (target)
            target.Got(damage);
    }
}
