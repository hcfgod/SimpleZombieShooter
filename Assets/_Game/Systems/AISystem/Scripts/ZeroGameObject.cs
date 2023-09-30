using UnityEngine;

public static class ZeroGameObject
{
    public static void ZeroGameObjectPosition(GameObject gameObject)
    {
        gameObject.transform.position = Vector3.zero;
    }
    public static void ZeroGameObjectLocalPosition(GameObject gameObject)
    {
        gameObject.transform.localPosition = Vector3.zero;
    }

    public static void ZeroGameObjectRotation(GameObject gameObject)
    {
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    public static void ZeroGameLocalRotation(GameObject gameObject)
    {
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
