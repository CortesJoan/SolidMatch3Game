using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoaderTrigger : MonoBehaviour
{
    [SerializeField] private List<AssetReferencePairName> availableScenes;
    [SerializeField] private bool discardActivesceneWhenLoadingAnother = false;

    private void Start()
    {
        PreloadNextScenes();
    }

    void PreloadNextScenes()
    {
        foreach (AssetReferencePairName availableScene in availableScenes)
        {
            AdditiveSceneLoader.instance.LoadAdditiveSceneFromAddresables(availableScene, false);
        }
    }

    public void LoadScene(string name)
    {
        AdditiveSceneLoader.instance.ActivateSceneByName(name);
        if (discardActivesceneWhenLoadingAnother)
        {
            AdditiveSceneLoader.instance.UnloadAdditiveScene(SceneManager.GetActiveScene());
        }
    }
    
    [ContextMenu(nameof(AssignAssetName))]
    public  void AssignAssetName()
    {
        foreach (var asset in availableScenes)
        {
            asset.assetName = asset.assetReference.editorAsset.name;
        }
    }
}


[Serializable]
public class AssetReferencePairName
{
    public string assetName;
    public AssetReference assetReference;
}
