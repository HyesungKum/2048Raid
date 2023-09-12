using UnityEngine;

/*
 * Singleton
 */
public class Singleton<T> where T : class, new()
{
	private static object _syncobj = new object();
	private static volatile T _instance = null;
	public static T Inst
	{
		get
		{
			if (_instance == null)
			{
				lock (_syncobj)
				{
					_instance = new T();
				}
			}
			return _instance;
		}
	}
}

/*
 * MonobehaviourSinglton
 */
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
	private static T _instance = null;
	public static T Inst
	{
		get
		{
			//Debug.Log($"##MonoBehaviourSingleton : " + typeof(T).ToString());
			if (_isShuttingDown) return null;
			if (_instance == null)
			{
				// find
				_instance = GameObject.FindObjectOfType(typeof(T)) as T;
				if (_instance == null)
				{
					// Create
					_instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
					if (_instance == null)
					{
						Debug.LogError("##[Error]MonoBehaviourSingleton Instance Init ERROR - " + typeof(T).ToString());
					}


				}

			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null)
		{
			//base.Awake();
			transform = base.transform;
			gameObject = base.gameObject;
			InstanceID = GetInstanceID();

			//Debug.Log( "##[Info]Instance Set : " + +GetInstanceID() );
			_instance = this as T;
		}
		else
		{
			if (_instance != this)
				//Debug.Log( "##[Info]Instance Already : " + GetInstanceID() );
				DestroyImmediate(base.gameObject);
		}

		InitInAwake();
	}

	protected virtual void InitInAwake()
	{
#if SINGLETON_AWAKE_TEXT
        Debug.Log("##[Info]## MonoBehaviourSingleton - InitInAwake : " + typeof(T).ToString());
#endif // SINGLETON_AWAKE_TEXT
	}

	protected void OnDestroy()
	{
		DestroyInSingleton();

		_instance = null;
	}

	protected virtual void DestroyInSingleton() { }

	static bool _isShuttingDown = false;
	protected virtual void OnApplicationQuit()
	{
		_instance = null;
		_isShuttingDown = true;
	}

	[HideInInspector]
	public int InstanceID;
	public new Transform transform { get; private set; }
	public new GameObject gameObject { get; private set; }
	protected bool _initialized = false;
	public bool Initialized
	{
		get { return _initialized; }
		private set { }
	}
}

/*
 * ImmortalMonoBehaviourSingleton
 */
public class ImmortalMonoBehaviourSingleton<T> : MonoBehaviour where T : ImmortalMonoBehaviourSingleton<T>
{

	private static T _instance = null;
	public static T Inst
	{
		get
		{
			if (_isShuttingDown) return null;
			if (_instance == null)
			{
				// find
				_instance = GameObject.FindObjectOfType(typeof(T)) as T;
				if (_instance == null)
				{
					// Create
					_instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
					if (_instance == null)
					{
						Debug.LogError("##[Error]ImmortalMonoBehaviourSingleton Instance Init ERROR - " + typeof(T).ToString());
					}
				}
			}
			return _instance;
		}
	}

	void Awake()
	{
		if (_instance == null)
		{
			transform = base.transform;
			gameObject = base.gameObject;
			InstanceID = GetInstanceID();

			_instance = this as T;
		}
		else
		{
			if (_instance != this)
				DestroyImmediate(base.gameObject);
		}

		DontDestroyOnLoad(gameObject);
		InitInAwake();
	}

	public virtual void InitInAwake()
	{
#if SINGLETON_AWAKE_TEXT
        Debug.Log("##[Info]## Immortal -- InitInAwake : " + typeof(T).ToString());
#endif // SINGLETON_AWAKE_TEXT
	}

	protected void OnDestroy()
	{
		_instance = null;

		DestroyInSingleton();
	}

	protected virtual void DestroyInSingleton() { }

	static bool _isShuttingDown = false;
	protected virtual void OnApplicationQuit()
	{
		DestroyImmediate(base.gameObject);
		_instance = null;
		_isShuttingDown = true;
	}

	[HideInInspector]
	public int InstanceID;
	public new Transform transform { get; private set; }
	public new GameObject gameObject { get; private set; }
	protected bool _initialized = false;
	public bool Initialized
	{
		get { return _initialized; }
		private set { }
	}
}
