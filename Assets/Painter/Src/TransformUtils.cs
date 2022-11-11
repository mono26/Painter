using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
    public static Transform FindRecursive(this Transform transformComponent, string objectName)
    {
        for (int i = 0; i < transformComponent.childCount; i++) {
            Transform child = transformComponent.GetChild(i);
            if (child.name.Equals(objectName)) {
                return child;
            }

            Transform transformInChild = child.FindRecursive(objectName);
            if (transformInChild != null) {
                return transformInChild;
            }

            continue;
        }

        return null;
    }
}
