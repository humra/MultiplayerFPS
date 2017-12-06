using UnityEngine;

public class Util {

	public static void SetLayerRecursive(GameObject obj, int newLayer)
    {
        if(obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach(Transform child in obj.transform)
        {
            if(child == null)
            {
                continue;
            }

            SetLayerRecursive(child.gameObject, newLayer);
        }
    }
}
