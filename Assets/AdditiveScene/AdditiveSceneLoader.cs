using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class AdditiveSceneLoader : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Scene> onLoadedAdditiveScene;

    public static AdditiveSceneLoader instance;
    [SerializeField]
    public List<AsyncOperationHandle<SceneInstance>> loadedAdditiveScenes =
        new List<AsyncOperationHandle<SceneInstance>>();
    [SerializeField] public List<string> loadedSceneNames;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadAdditiveSceneFromAddresables(AssetReferencePairName assetReference, bool loadActivated = true)
    {
        if (assetReference.assetName == null || assetReference.assetName.Equals("") || loadedSceneNames.Contains(assetReference.assetName))
        {
            return;
        }
        AsyncOperationHandle<SceneInstance> obj =
            Addressables.LoadSceneAsync(assetReference.assetReference, LoadSceneMode.Additive, true);
        loadedAdditiveScenes?.Add(obj);
        loadedSceneNames.Add(assetReference.assetName);
        obj.Completed += OnLoadAdditiveScene;
        obj.Completed += (obj) =>
        {
            obj.Result.Scene.GetRootGameObjects().ToList().ForEach((g) => g.SetActive(loadActivated));
        };
    }

    private void OnLoadAdditiveScene(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status.Equals(AsyncOperationStatus.Succeeded))
        {
            Debug.Log("Additive scene is loaded");
            onLoadedAdditiveScene?.Invoke(obj.Result.Scene);
        }
    }

    public void UnloadAdditiveScene(Scene scene)
    {
        AsyncOperationHandle<SceneInstance> sceneReference = new AsyncOperationHandle<SceneInstance>();
        foreach (AsyncOperationHandle<SceneInstance> loadedScene in loadedAdditiveScenes)
        {
            if (loadedScene.Result.Scene.name.Equals(scene.name))
            {
                sceneReference = loadedScene;
            }
        } 
        loadedSceneNames.Remove(scene.name);
        if (!sceneReference.IsValid())
        {
            Debug.Log("Delete scene normally");
            SceneManager.UnloadSceneAsync(scene);
            return;
        }
        UnloadAdditiveScene(sceneReference);

    }

    private void UnloadAdditiveScene(AsyncOperationHandle<SceneInstance> obj)
    {
        
        Addressables.UnloadSceneAsync(obj.Result, true).Completed += OnSceneUnloaded;
    }   
      

    public Scene GetSceneByName(string name)
    {
        Scene sceneToReturn = SceneManager.GetActiveScene();
        foreach (AsyncOperationHandle<SceneInstance> loadedScene in loadedAdditiveScenes)
        {
            Scene scene = loadedScene.Result.Scene;
            if (scene.name.Equals(name))
            {
                sceneToReturn = scene;
            }
        }
        return sceneToReturn;
    }

    public void ActivateSceneByName(string name)
    {
        foreach (AsyncOperationHandle<SceneInstance> loadedScene in loadedAdditiveScenes)
        {
            Scene scene = loadedScene.Result.Scene;
            if (scene.name!=null && scene.name.Equals(name))
            {
                loadedScene.Result.ActivateAsync();
                scene.GetRootGameObjects().ToList().ForEach((g) => g.SetActive(true));
            }
        }
    }

    private void OnSceneUnloaded(AsyncOperationHandle<SceneInstance> obj)
    {
        loadedAdditiveScenes.Remove(obj);
        Debug.Log("Additive scene is unloaded");
    }
}