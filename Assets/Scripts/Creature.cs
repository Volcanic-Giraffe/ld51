
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Creature : MonoBehaviour
{
    public float Radius = 4f;

    private Animator _animator;

    private Vector3 _initialPoint;

    private void Awake()
    {
        _initialPoint = transform.position;
        _animator = GetComponentInChildren<Animator>();
        StartCoroutine(GoToRandomPoint());
    }

    IEnumerator GoToRandomPoint()
    {
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        while (true)
        {
            var randomPoint = _initialPoint + Random.onUnitSphere * Random.Range(Radius / 2, Radius);
            randomPoint.z = _initialPoint.z;

            var rot = transform.rotation;
            var pos = transform.position;
            
            float angleS = Vector3.SignedAngle(randomPoint - pos, transform.right, -Vector3.forward);
            var targetRot = Quaternion.Euler(0, 0, angleS + 90);

            var child = transform.GetChild(0);

            for (float i = 0; i <= 1.0; i += 0.05f)
            {
                child.rotation = Quaternion.Lerp(rot, targetRot, i);
                yield return new WaitForEndOfFrame();
            }

            //_animator.SetBool("Walking", true);

            var childPos = child.localPosition;
            yield return new WaitForSeconds(2f);
            for (float i = 0; i <= 1.0; i += 0.005f)
            {
                transform.position = Vector3.Lerp(pos, randomPoint, i);
                
                child.localPosition = childPos + new Vector3(0, 0,
                    i < 0.1f ? Mathf.Lerp(0, 0.5f, i / 0.1f) :
                    (i < 0.4f) ? Mathf.Lerp(0.5f, -2, (i-0.1f) / 0.3f) : Mathf.Lerp(-2, 0, (i - 0.4f) / 0.6f));
                child.localScale = new Vector3(1, 1, 
                    (i < 0.1f ? Mathf.Lerp(1, 0.5f, i/0.1f) : (i < 0.4f ? Mathf.Lerp(0.5f, 1.5f, (i-0.1f)/0.3f) : (Mathf.Lerp(1.5f, 1, (i-0.4f)/0.6f)))));
                yield return new WaitForEndOfFrame();
            }

            //_animator.SetBool("Walking", false);
            yield return new WaitForSeconds(Random.Range(5f, 8f));
        }
    }

    private void Update()
    {
        
    }
}
