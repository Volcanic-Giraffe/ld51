using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Sworcher : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DOScale(Vector3.one * 2f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
