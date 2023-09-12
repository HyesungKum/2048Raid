using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// GameObject 풀
/// </summary>
public class PAGameObjectPool
{
	Queue<GameObject> _queue = new Queue<GameObject>();
	GameObject _prefab;	

    /// <summary>
    /// 지정된 프리팹의 경로(Asset/Resources 하위 경로여야 한다.)
    /// </summary>
    /// <param name="prefabPath">asdafs</param>
	public PAGameObjectPool(string prefabPath)
	{
        // 프리팹은 처음 한번만 로드한다.
		_prefab = Resources.Load<GameObject>(prefabPath);
	}

    /// <summary>
    /// 생성할 때 호출
    /// Pool 에 여분이 있다면 그것을 전달해 주고,
    /// 없다면, 새로 이미 로드해 놓은 프리팹을 참조해서 Instantiate
    /// </summary>
    /// <returns></returns>
	public virtual GameObject Spawn()
	{
		GameObject temp = null;

        // 여분이 없다면 새로 Instantiate
		if (_queue.Count == 0)
		{
			//새로생성해서반환
			temp = GameObject.Instantiate<GameObject>(_prefab);
			//temp.name = typeof(GameObject).ToString();
			temp.name = _prefab.name;
		}
        // 여분이 있다면 기존 것을 전달
		else
		{
			temp = _queue.Dequeue();
		}

        // 전달하기 전에 활성화는 기본
		temp.SetActive(true);
		return temp;
	}

	/// <summary>
	/// 생성된 오브젝트를 다 사용했다면 Pool 에 반환을 한다.
	/// </summary>
	/// <param name="obj"></param>
	public virtual void Despawn(GameObject obj)
	{
		_queue.Enqueue(obj);
		obj.SetActive(false);
	}
}

/// <summary>
/// MonoBehaviour 풀 - 일반형 지원(단, MonoBehaviour 상속 클래스)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PAMonoBehaviourPool<T> where T : MonoBehaviour
{
	Queue<T> _queue = new Queue<T>();
	T _prefab;

	/// <summary>
	/// 지정된 프리팹의 경로(Asset/Resources 하위 경로여야 한다.)
	/// </summary>
	/// <param name="prefabPath"></param>
	public PAMonoBehaviourPool(string prefabPath)
	{
		// 프리팹은 처음 한번만 로드한다.
		_prefab = Resources.Load<T>(prefabPath);
	}

	/// <summary>
	/// 생성할 때 호출
	/// Pool 에 여분이 있다면 그것을 전달해 주고,
	/// 없다면, 새로 이미 로드해 놓은 프리팹을 참조해서 Instantiate
	/// </summary>
	/// <returns></returns>
	public T Spawn()
	{
		T temp = null;

		// 여분이 없다면 새로 Instantiate
		if (_queue.Count == 0)
		{
			//새로생성해서반환
			temp = GameObject.Instantiate<T>(_prefab);
			temp.name = typeof(T).ToString();
		}
		// 여분이 있다면 기존 것을 전달
		else
		{
			temp = _queue.Dequeue();
		}

		// 전달하기 전에 활성화는 기본
		temp.gameObject.SetActive(true);
		return temp;
	}

	/// <summary>
	/// 생성된 오브젝트를 다 사용했다면 Pool 에 반환을 한다.
	/// </summary>
	/// <param name="obj"></param>
	public void Despawn(T obj)
	{
		_queue.Enqueue(obj);
		obj.gameObject.SetActive(false);
	}
}

/// <summary>
/// PAObjectPool 과는 다르게 모든 게임오브젝트를 담을 수 있는 Object Pool (PAObjectPool<T>(
/// 전역적으로 사용될 필요성이 있어서 Singleton으로 작성
/// </summary>
/// <typeparam name="T"></typeparam>
public class PAObjectPoolSingleton : Singleton<PAObjectPoolSingleton>
{
	/// <summary>
	/// string : Pooling 키는 프리팹의 경로로 한다. 
	/// </summary>
	// 
	Dictionary<string, PAGameObjectPool> pool = new Dictionary<string, PAGameObjectPool>();

    /// <summary>    
    /// 
    /// int : GameObject Instance ID (IID)
    /// string : 프리팹 경로
    /// 
    /// 게임오브젝트의 IID로 프리팹경로를 찾기 위해서 (Despawn 함수에서 사용)
    /// </summary>
    Dictionary<int, string> prefabNameTableByIID = new Dictionary<int, string>();

	/// <summary>
	/// 생성할 때 호출
	/// Pool 에 여분이 있다면 그것을 전달해 주고,
	/// 없다면, 새로 이미 로드해 놓은 프리팹을 참조해서 Instantiate
	/// 
	/// </summary>
	/// <returns></returns>
	public GameObject Spawn(string prefabPath)
	{
		PAGameObjectPool poolObj;
        if (pool.TryGetValue(prefabPath, out poolObj) == false)
        {
			poolObj = new PAGameObjectPool(prefabPath);
            if (poolObj == null)
                Debug.LogError("## Failed to Load Prefab : " + prefabPath);
            else
                pool.Add(prefabPath, poolObj);
		}
		
        // 새 객체 생성
        GameObject instObj = poolObj.Spawn();

        // 생성된 객체의 Instance ID 와 프리팹 경로 저장
        if (prefabNameTableByIID.TryAdd(instObj.GetInstanceID(), prefabPath) == false)
            Debug.Log("## 키가 이미 있다고 ?? 그러면 안되는데?");

		return instObj;
	}

    /// <summary>
    /// 생성된 오브젝트를 다 사용했다면 Pool 에 반환을 한다.
    /// </summary>
    /// <param name="obj"></param>
	public void Despawn(GameObject obj)
	{
		int iid = obj.GetInstanceID();
		// IID 로 프리팹 경로를 찾아서
		string prefabPath;
		if(prefabNameTableByIID.TryGetValue(iid, out prefabPath) == false)
        {
			Debug.LogError($"## IID 로 프리팹 경로를 찾을 수 없다 : IID <{obj.GetInstanceID()}>");
			return;
		}
		// Pool 에 반환
		PAGameObjectPool poolObj;
		if (pool.TryGetValue(prefabPath, out poolObj) == false)
        {
			Debug.LogError("## Pool 에 해당 Object Pool이 없다. prefabPath : " + prefabPath);
			return;
		}
		prefabNameTableByIID.Remove(iid);
		poolObj.Despawn(obj);
	}
}

/// <summary>
/// 오브젝트 풀링에서 일정 주기마다 사용하지 않는 오브젝트를 제거한다
/// </summary>
public class PAGameObjectGCPool
{
	private LinkedList<GameObject> _garbageList = new LinkedList<GameObject>();
	private GameObject _prefab;

	private const int delaySec = 8;

	private bool isDestroying;

	public Action NotifyDestroy;
	public void notifyDestroy() => NotifyDestroy?.Invoke();

	public PAGameObjectGCPool(string prefabPath)
	{
		_prefab = Resources.Load<GameObject>(prefabPath);

		isDestroying = false;
	}

	public GameObject Spawn()
	{
		GameObject temp = null;

		if (_garbageList.Count == 0) // 회수 리스트 먼저 체크
		{
			temp = GameObject.Instantiate<GameObject>(_prefab);
			temp.name = _prefab.name;
		}
		else
		{
			//가장 위에 것부터 사용
			temp = _garbageList.First.Value;
			_garbageList.RemoveFirst();
		}

		temp.SetActive(true);

		return temp;
	}

	public void Despawn(GameObject obj)
	{
		//넣을때도 위에서
		_garbageList.AddFirst(obj);
		obj.SetActive(false);

		if (!isDestroying) destroyAsync();
	}

	async void destroyAsync()
	{
		isDestroying = true;

		await Task.Delay(delaySec * 1000);

		if (_garbageList.Count == 0)
		{
			isDestroying = false;
			return;
		}

		GameObject obj = _garbageList.Last.Value;
		while (Application.isPlaying && !obj.activeInHierarchy)
		{
			GameObject.Destroy(obj.gameObject);
			_garbageList.RemoveLast();

			if (_garbageList.Count == 0) break;

			obj = _garbageList.Last.Value;
		}

		isDestroying = false;
	}
}

public class PAGameObjecGCPoolSingleton : Singleton<PAGameObjecGCPoolSingleton>
{
	/// <summary>
	/// string : Pooling 키는 프리팹의 경로로 한다. 
	/// </summary>
	// 
	Dictionary<string, PAGameObjectGCPool> pool = new Dictionary<string, PAGameObjectGCPool>();

	/// <summary>    
	/// 
	/// int : GameObject Instance ID (IID)
	/// string : 프리팹 경로
	/// 
	/// 게임오브젝트의 IID로 프리팹경로를 찾기 위해서 (Despawn 함수에서 사용)
	/// </summary>
	Dictionary<int, string> prefabNameTableByIID = new Dictionary<int, string>();

	/// <summary>
	/// 생성할 때 호출
	/// Pool 에 여분이 있다면 그것을 전달해 주고,
	/// 없다면, 새로 이미 로드해 놓은 프리팹을 참조해서 Instantiate
	/// </summary>
	/// <returns></returns>
	public GameObject Spawn(string prefabPath)
	{
		PAGameObjectGCPool poolObj;
		if (pool.TryGetValue(prefabPath, out poolObj) == false)
		{
			poolObj = new PAGameObjectGCPool(prefabPath);
			if (poolObj == null)
				Debug.LogError("## Failed to Load Prefab : " + prefabPath);
			else
				pool.Add(prefabPath, poolObj);
		}

		// 새 객체 생성
		GameObject instObj = poolObj.Spawn();

		// 생성된 객체의 Instance ID 와 프리팹 경로 저장
		if (prefabNameTableByIID.TryAdd(instObj.GetInstanceID(), prefabPath) == false)
			Debug.Log("## 키가 이미 있다고 ?? 그러면 안되는데?");

		return instObj;
	}

	/// <summary>
	/// 생성된 오브젝트를 다 사용했다면 Pool 에 반환을 한다.
	/// </summary>
	/// <param name="obj"></param>
	public void Despawn(GameObject obj)
	{
		int iid = obj.GetInstanceID();
		// IID 로 프리팹 경로를 찾아서
		string prefabPath;
		if (prefabNameTableByIID.TryGetValue(iid, out prefabPath) == false)
		{
			Debug.LogError($"## IID 로 프리팹 경로를 찾을 수 없다 : IID <{obj.GetInstanceID()}> name<{obj.name}>");
			return;
		}

		// Pool 에 반환
		PAGameObjectGCPool poolObj;
		if (pool.TryGetValue(prefabPath, out poolObj) == false)
		{
			Debug.LogError("## Pool 에 해당 Object Pool이 없다. prefabPath : " + prefabPath);
			return;
		}
		prefabNameTableByIID.Remove(iid);
		poolObj.Despawn(obj);
	}
}