using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T:new()
{
	private static T instance;

	private static object objectLock = new object();

	public static T Instance{

		get{
			if (instance == null) {
				lock (objectLock)    
				{   
					if (instance == null)    
						instance = new T();   
				}   
			}
			return instance;
		}
	}

}

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour  
{  
	private static volatile T instance;  
	private static object objectLock = new Object();  
	public static T Instance  
	{  
		get  
		{  
			if (instance == null)  
			{  
				lock (objectLock)  
				{  
					if (instance == null)  
					{  
						T[] instances = FindObjectsOfType<T>();  
						if (instances != null)  
						{  
							for (var i = 0; i < instances.Length; i++)  
							{  
								Destroy(instances[i].gameObject);  
							}  
						}  
						GameObject go = new GameObject();  
						go.name = typeof(T).Name;  
						instance = go.AddComponent<T>();  
						DontDestroyOnLoad(go);  
					}  
				}  
			}  
			return instance;  
		}  
	}  
}  
