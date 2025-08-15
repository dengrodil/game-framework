using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public static class SingletonManager {

	private static Dictionary<System.Type, MonoBehaviour> singletons = new Dictionary<System.Type, MonoBehaviour>();

    static SingletonManager()
    {
        SceneManager.sceneUnloaded += delegate
        {
            // Remove null singletons
            var toDelete = singletons.Keys.Where(k => !singletons[k]).ToList();
            foreach (var key in toDelete) singletons.Remove(key);
        };
    }

    /// <summary>
    /// Gets a singleton object. Object must be registered.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
	public static T Get<T>() where T : MonoBehaviour
	{
        MonoBehaviour obj = null;
        if (singletons.TryGetValue(typeof(T), out obj))
        {
            // Clear registered singleton if it was destroyed
            if (!obj) singletons.Remove(typeof(T));
        }

        //Assert.IsNotNull(obj, typeof(T).Name + " singleton doesn't exist");
        return obj as T;
	}

    /// <summary>
    /// Registers the object as a singleton reference.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The singleton component</param>
	public static void Register<T>(T obj) where T : MonoBehaviour
	{
        Assert.IsNull(Get<T>(), typeof(T).Name + " singleton already exists");
        singletons[typeof(T)] = obj;
	}

    /// <summary>
    /// Removes the singleton from the reference list.
    /// This will not destroy the object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public static void Remove<T>() where T : MonoBehaviour
	{
        singletons.Remove(typeof(T));
	}

    /// <summary>
    /// Removes all singletons from the reference list
    /// </summary>
	public static void RemoveAll()
	{
		singletons.Clear();
	}
}
