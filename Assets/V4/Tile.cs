using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[Serializable]
public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int tileType;
    [SerializeField] private bool isFullySpawned;
    [SerializeField] int numberOfSameTilesToMatch;
    [FormerlySerializedAs("onDestroy")] public UnityEvent onDestroyStart;
    public UnityEvent onDestroyFinish;
    public UnityEvent onFullySpawned;
    public UnityEvent onTileSelected;
    [FormerlySerializedAs("onTileUnSelected")] public UnityEvent onTileDeselected;
    public UnityEvent<bool> onHighLight;
 
    public void Init(int xPos, int yPos, int type)
    {
        x = xPos;
        y = yPos;
        tileType = type;
     }

    public void MoveToPosition(int newX, int newY, float duration)
    {
        x = newX;
        y = newY;
        isFullySpawned = false;

        transform.DOLocalMove(new Vector2(x, y), duration)
            .OnComplete(() => { isFullySpawned = true; });
    }

    public void DestroyTile()
    {
        onDestroyStart?.Invoke();
        onDestroyFinish?.Invoke();
    }
    
    public bool IsFullySpawned()
    {
        return isFullySpawned;
    }

    public void SetIsFullySpawned(bool value)
    {
        isFullySpawned = value;
        if (isFullySpawned)
        {
            onFullySpawned?.Invoke();
        }
    }

    public int GetMinimumTileCountForMatch()
    {   
        return numberOfSameTilesToMatch;
    }
    public void Highlight(bool status)
    {
        if (!isFullySpawned)
        {
            return;
        }
        onHighLight?.Invoke(status);
       
    }

    public void Select()
    {
        onTileSelected?.Invoke();
    }

    public void UnSelect()
    {
        onTileDeselected?.Invoke();
    }
}