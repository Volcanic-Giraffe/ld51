using UnityEngine;

public static class GameObjectExtensions
{
    public static T GetClosestChild<T>(this GameObject obj)
    {
        return obj.GetComponent<T>() ?? obj.GetComponentInChildren<T>();
    }

    public static T GetClosestParent<T>(this GameObject obj)
    {
        return obj.GetComponent<T>() ?? obj.GetComponentInParent<T>();
    }
    
    public static T GetClosestChild<T>(this Component obj)
    {
        return obj.GetComponent<T>() ?? obj.GetComponentInChildren<T>();
    }

    public static T GetClosestParent<T>(this Component obj)
    {
        return obj.GetComponent<T>() ?? obj.GetComponentInParent<T>();
    }
}