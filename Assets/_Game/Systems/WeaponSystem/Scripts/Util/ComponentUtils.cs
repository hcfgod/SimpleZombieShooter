using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentUtils : MonoBehaviour
{
	public static T AddOrGetComponent<T>(GameObject gameObject) where T : MonoBehaviour
	{
		T component = gameObject.GetComponent<T>();
    
		if (component == null)
		{
			component = gameObject.AddComponent<T>();
		}
    
		return component;
	}
	
	public static void RemoveDuplicateComponents<T>(GameObject gameObject) where T : class
	{
		List<MonoBehaviour> componentsToRemove = new List<MonoBehaviour>();
		T foundComponent = null;

		foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
		{
			if (component is T)
			{
				if (foundComponent == null)
				{
					foundComponent = component as T;
				}
				else
				{
					componentsToRemove.Add(component);
				}
			}
		}

		foreach (MonoBehaviour component in componentsToRemove)
		{	
            #if UNITY_EDITOR
			DestroyImmediate(component);
            #else
			Destroy(component);
            #endif
		}
	}
	
	public static List<Transform> CollectAllTransforms(Transform root)
	{
		List<Transform> _ignoreTransforms = new List<Transform>();
		CollectAllTransformsRecursive(root, _ignoreTransforms);
		return _ignoreTransforms;
	}

	private static void CollectAllTransformsRecursive(Transform root, List<Transform> list)
	{
		list.Add(root);
		foreach (Transform child in root)
		{
			CollectAllTransformsRecursive(child, list);
		}
	}
	
	public static List<string> CollectAllTransformsNames(Transform root)
	{
		List<string> _ignoreTransformsNames = new List<string>();
		CollectAllTransformsNamesRecursive(root, _ignoreTransformsNames);
		return _ignoreTransformsNames;
	}

	private static void CollectAllTransformsNamesRecursive(Transform root, List<string> list)
	{
		list.Add(root.name);
		
		foreach (Transform child in root)
		{
			CollectAllTransformsNamesRecursive(child, list);
		}
	}
	
	public static void DeactivateComponentsAndColliders(GameObject root)
	{
		// Deactivate MonoBehaviour components
		MonoBehaviour[] monoBehaviours = root.GetComponents<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in monoBehaviours)
		{
			monoBehaviour.enabled = false;
		}

		// Deactivate Colliders
		Collider[] colliders = root.GetComponents<Collider>();
		foreach (Collider collider in colliders)
		{
			collider.enabled = false;
		}

		// Recursively deactivate components and colliders in child GameObjects
		foreach (Transform child in root.transform)
		{
			DeactivateComponentsAndColliders(child.gameObject);
		}
	}
}
