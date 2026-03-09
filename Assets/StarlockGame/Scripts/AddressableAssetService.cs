using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableAssetService : MonoBehaviour
{
    public static AddressableAssetService Instance { get; private set; }

    public bool IsReady { get; private set; }

    private Dictionary<string, UnityEngine.Object> loadedAssets = new Dictionary<string, UnityEngine.Object>();
    private List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }

    public void PreloadAssets(string label, Action<float> onProgress, Action onComplete, Action<string> onError)
    {
        StartCoroutine(PreloadAssetsRoutine(label, onProgress, onComplete, onError));
    }

    private IEnumerator PreloadAssetsRoutine(string label, Action<float> onProgress, Action onComplete, Action<string> onError)
    {
        var locationsHandle = Addressables.LoadResourceLocationsAsync(label);
        yield return locationsHandle;

        if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
        {
            onError?.Invoke("Failed to load resource locations");
            yield break;
        }

        var locations = locationsHandle.Result;
        int total = locations.Count;
        int loaded = 0;

        if (total == 0)
        {
            IsReady = true;
            onComplete?.Invoke();
            yield break;
        }

        foreach (var location in locations)
        {
            var loadHandle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
            handles.Add(loadHandle);

            loadHandle.Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    string key = location.PrimaryKey;
                    loadedAssets[key] = handle.Result;
                }

                loaded++;
                onProgress?.Invoke((float)loaded / total);

                if (loaded >= total)
                {
                    IsReady = true;
                    onComplete?.Invoke();
                }
            };
        }
    }

    public T GetAsset<T>(string address) where T : UnityEngine.Object
    {
        if (loadedAssets.TryGetValue(address, out var asset))
        {
            return asset as T;
        }
        return null;
    }

    public void LoadAsset<T>(string address, Action<T> onLoaded) where T : UnityEngine.Object
    {
        if (loadedAssets.TryGetValue(address, out var cached))
        {
            onLoaded?.Invoke(cached as T);
            return;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        handles.Add(handle);

        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[address] = h.Result;
                onLoaded?.Invoke(h.Result);
            }
            else
            {
                onLoaded?.Invoke(null);
            }
        };
    }

    public Sprite GetSprite(string address)
    {
        return GetAsset<Sprite>(address);
    }

    public AudioClip GetAudioClip(string address)
    {
        return GetAsset<AudioClip>(address);
    }

    private void OnDestroy()
    {
        foreach (var handle in handles)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
        handles.Clear();
        loadedAssets.Clear();
    }
}
