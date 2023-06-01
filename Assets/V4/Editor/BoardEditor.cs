using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    private Board board;

    private void OnEnable()
    {
        board = (Board)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create board"))
        {
            Undo.RecordObject(board, "Create new board");
            PrefabUtility.RecordPrefabInstancePropertyModifications(board);
            DestroyBoardTiles();
            board.Awake();
            var prechargedTiles = new List<Tile>();
            foreach (var item in board.GetTileMatrix()) // loop through the array
            {
                prechargedTiles.Add(item); // add each item to the list
            }
            board.prechargedTiles = prechargedTiles;
        }
        if (GUILayout.Button("Destroy board"))
        {
            Undo.RecordObject(board, " Destroy board");
            PrefabUtility.RecordPrefabInstancePropertyModifications(board);
            DestroyBoardTiles();
        }
    }

    public void DestroyBoardTiles()
    {
        var tileMatrix = board.GetTileMatrix();
        if (tileMatrix != null)
        {
            foreach (var tile in tileMatrix)
            {
                if (tile != null)
                {
                    DestroyImmediate(tile.gameObject);
                }
            }
        }
        board.SetTileMatrix(null);
    }
}