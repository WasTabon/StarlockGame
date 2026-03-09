using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ContentLoader : MonoBehaviour
{
    public static ContentLoader Instance { get; private set; }

    private const string ContentLabel = "GameContent";

    public event Action<float> OnDownloadProgress;
    public event Action OnDownloadComplete;
    public event Action<string> OnDownloadError;
    public event Action OnNoInternet;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartDownload()
    {
        StartCoroutine(CheckInternetAndDownload());
    }

    private IEnumerator CheckInternetAndDownload()
    {
        while (true)
        {
            using (var request = new UnityWebRequest("https://www.google.com"))
            {
                request.method = UnityWebRequest.kHttpVerbHEAD;
                request.timeout = 5;

                yield return request.SendWebRequest();

                bool hasInternet = request.result != UnityWebRequest.Result.ConnectionError
                                && request.result != UnityWebRequest.Result.ProtocolError;

                if (hasInternet)
                {
                    StartCoroutine(CheckAndDownloadContent());
                    yield break;
                }

                OnNoInternet?.Invoke();
                yield return new WaitForSeconds(2f);
            }
        }
    }

    private IEnumerator CheckAndDownloadContent()
    {
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        var sizeHandle = Addressables.GetDownloadSizeAsync(ContentLabel);
        yield return sizeHandle;

        if (sizeHandle.Status != AsyncOperationStatus.Succeeded)
        {
            OnDownloadError?.Invoke("Failed to check download size");
            yield break;
        }

        long downloadSize = sizeHandle.Result;

        if (downloadSize == 0)
        {
            Debug.Log("Content already cached");
            StartPreload();
            yield break;
        }

        Debug.Log($"Need to download: {downloadSize / (1024f * 1024f):F2} MB");

        var downloadHandle = Addressables.DownloadDependenciesAsync(ContentLabel);

        while (!downloadHandle.IsDone)
        {
            OnDownloadProgress?.Invoke(downloadHandle.PercentComplete);
            yield return null;
        }

        if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(downloadHandle);
            StartPreload();
        }
        else
        {
            OnDownloadError?.Invoke("Download failed");
        }
    }

    private void StartPreload()
    {
        if (AddressableAssetService.Instance == null)
        {
            GameObject serviceObj = new GameObject("AddressableAssetService");
            serviceObj.AddComponent<AddressableAssetService>();
        }

        AddressableAssetService.Instance.PreloadAssets(
            ContentLabel,
            progress => OnDownloadProgress?.Invoke(progress),
            () => OnDownloadComplete?.Invoke(),
            error => OnDownloadError?.Invoke(error)
        );
    }

    public void Retry()
    {
        StopAllCoroutines();
        StartDownload();
    }
}
