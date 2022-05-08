using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float dampingFactor;
    [SerializeField] bool ignoreXAngle = true;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            target = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            
            Vector3 targetPos = target.position;
            Vector3 targetDir = target.forward;
            if (ignoreXAngle)
            {
                targetDir.y = 0;
                targetDir.Normalize();
            }
            if (dampingFactor > 0)
            {
                float t = dampingFactor * Time.deltaTime;
                t = Mathf.Min(t,1);
                transform.position = Vector3.Lerp(transform.position, target.position, t);
                transform.forward = Vector3.Lerp(transform.forward, targetDir, t);
            } else
            {
                transform.position = targetPos;
                transform.forward = targetDir;
            }
            
        }
    }
}
