using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Extebnsions
{
    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static T PickRandom<T>(this IList<T> collection)
    {
        return collection.Count == 0 ? default : collection[Random.Range(0, collection.Count)];
    }

    public static T PickRandom<T>(this T[] collection)
    {
        return collection.Length == 0 ? default : collection[Random.Range(0, collection.Length)];
    }

}
