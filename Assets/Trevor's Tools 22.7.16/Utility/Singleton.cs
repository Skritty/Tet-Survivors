using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[DefaultExecutionOrder(-50)]
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour 
{
	public static T Instance { get; private set; }
	[field: SerializeField]
	public bool UseDontDestroyOnLoad { get; private set; } = false;

	public virtual void Awake()
	{
		if (Instance != null && Instance != this as T)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this as T;
	}

	public virtual void Start()
	{
        if (UseDontDestroyOnLoad)
        {
			transform.parent = null;
			DontDestroyOnLoad(gameObject);
		}
	}

	public virtual void OnApplicationQuit()
	{
		Instance = null;
		Destroy(this);
	}
}