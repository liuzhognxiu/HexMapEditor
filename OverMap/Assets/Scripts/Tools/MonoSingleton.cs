using UnityEngine;

/// <summary>
/// 此单例继承于Mono，绝大多情况下，都不需要使用此单例类型。请使用Singleton
/// 不需要手动挂载
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    /// <summary>
    /// 线程锁
    /// </summary>
    private static readonly object _lock = new object();
    /// <summary>
    /// 程序是否正在退出
    /// </summary>
    protected static bool ApplicationIsQuitting { get; private set; }
    /// <summary>
    /// 是否为全局单例
    /// </summary>
    protected static bool isGolbal = true;

    static MonoSingleton()
    {
        ApplicationIsQuitting = false;
    }

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("[Singleton] " + typeof(T) +
                                            " already destroyed on application quit." +
                                            " Won't create again - returning null.");
                }

                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 先在场景中找寻
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] " + typeof(T).Name + " should never be more than 1 in scene!");
                        }

                        return _instance;
                    }

                    // 场景中找不到就创建新物体挂载
                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject();
                        _instance = singletonObj.AddComponent<T>();
                        singletonObj.name = "(singleton) " + typeof(T);

                        if (isGolbal && Application.isPlaying)
                        {
                            DontDestroyOnLoad(singletonObj);
                        }

                        return _instance;
                    }
                }

                return _instance;
            }
        }
    }

    /// <summary>
    /// 当工程运行结束，在退出时，不允许访问单例
    /// </summary>
    public void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }
}