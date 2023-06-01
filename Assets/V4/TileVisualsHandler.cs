using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
public class TileVisualsHandler : MonoBehaviour
{
    [SerializeField] private Tile tile;
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Color originalColor;
    float selectScaleDuration = 0.15f;
    private float selectMaxScale = 1.4f;

    private void OnEnable()
    {
        tile.onDestroyStart.AddListener(OnTileDestroyStart);
        tile.onDestroyFinish.AddListener(OnTileDestroyFinish);
        tile.onHighLight.AddListener(OnHighLight);
        tile.onFullySpawned.AddListener(OnTileFullySpawned);
        tile.onTileSelected.AddListener(OnTileSelected);
        tile.onTileDeselected.AddListener(OnTileDeselected);
    }

    private void OnTileFullySpawned()
    {
        renderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = renderer.color;
    }

    private void OnHighLight(bool status)
    {
        if (status)
        {
            // Change color of the tile or change sprite if you have different sprites for hint state
            renderer.color = Color.yellow;
        }
        else
        {
            // Reset tile color or change back to original sprite
            renderer.color = originalColor;
        }
    }

    private void OnTileDestroyStart()
    {
        GetComponent<SpawnDestructionEffect>().PlayDestructionEffect();
    }

    private void OnTileDestroyFinish()
    {
        GetComponent<SpawnDestructionEffect>().PlayDestructionEffect();
    }

    public void OnTileSelected()
    {
        ScaleUp();
        // Add any other visual changes here, e.g., change color or add an outline effect.
    }

    public void OnTileDeselected()
    {
        ScaleDown();
        // Revert any visual changes applied in OnTileSelected() here.
    }

    private void ScaleUp()
    {
         tile.transform.DOScale(new Vector3(selectMaxScale,selectMaxScale, 1), selectScaleDuration);
    }

    private void ScaleDown()
    {
         tile.transform.DOScale(Vector3.one, selectScaleDuration);
    }
}