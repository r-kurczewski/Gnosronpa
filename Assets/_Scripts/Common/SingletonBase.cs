using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : SingletonBase<T>
{
	private static T _instance;

	public static T Instance => _instance;

	protected void Awake()
	{
		if (_instance != null)
		{
			Debug.LogWarning($"There is already an instance of {typeof(T).Name}.");
			Destroy(this);
			return;
		}

		_instance = (T)this;
	}
}