using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Tilemapper : MonoBehaviour
{
    private Tilemap _tilemap;
    private int[] _blockableTiles = new int[] { 2, 3, 4, 5, 6 };
    private int _mowableCells; // The # of mowable cells
    private int _tilesMowed; // The count of the # of tiles mowed by the player
    private int _score;

    public int currentScene;
    public Sprite[] sprites;
    public TextAsset[] scenes;

    public Vector3 StartPosition { get; private set; }
    public bool IsReady { get; private set; }
    public int Score => _score;

    public bool LevelComplete => _tilesMowed == _mowableCells;

    public void MoveToNextScreen()
    {
        currentScene++;
        currentScene %= scenes.Length;
        PlayerPrefs.SetInt("currentScene", currentScene);
        PlayerPrefs.SetInt("score", Score);
    }

    public void MowLawn(Vector3 flymoPos)
    {
        if (!IsReady)
            return;

        var gridPos = _tilemap.WorldToCell(flymoPos);
        var tile = _tilemap.GetTile<Tile>(gridPos);
        if (tile != null)
        {
            if (tile.sprite == sprites[0])
            {
                tile.sprite = sprites[1];
                _tilemap.RefreshTile(gridPos);
                _tilesMowed++;
                _score = _score + 10;
            }
        }
    }

    public bool IsBlocker(Vector3 pos)
    {
        var cellPos = _tilemap.WorldToCell(pos);
        var tile = _tilemap.GetTile<Tile>(cellPos);
        return tile != null && tile.colliderType == Tile.ColliderType.Grid;
    }

    IEnumerator Start()
    {
        _tilemap = GetComponent<Tilemap>();
        _tilemap.size = new Vector3Int(40, 21);

        GetInteger("currentScene", (value) => currentScene = value);
        GetInteger("score", (score) => _score = score);

        currentScene = currentScene % scenes.Length; // Just in case :D 
        var tmxFile = Serializer.Deserialize(scenes[currentScene].text);
        var visual = tmxFile.layers.FirstOrDefault(layer => layer.name == "Visual");

        for (int y = 0; y < 21; y++)
        {
            for (int x = 0; x < 40; x++)
            {
                var spriteId = visual.data[y * 40 + x] - 1;
                if (spriteId > -1)
                {
                    var tile = ScriptableObject.CreateInstance<Tile>();
                    tile.sprite = sprites[spriteId];

                    tile.colliderType = Blocks(spriteId);
                    if (tile.colliderType == Tile.ColliderType.None)
                    {
                        _mowableCells++;
                    }
                    _tilemap.SetTile(new Vector3Int(x, 20 - y), tile);
                }
            }
        }

        StartPosition = _tilemap.CellToWorld(Vector3Int.zero);
        IsReady = true;
        yield return null;
    }

    private void GetInteger(string prefName, Action<int> action)
    {
        if (PlayerPrefs.HasKey(prefName))
        {
            action(PlayerPrefs.GetInt(prefName));
        }
    }

    private Tile.ColliderType Blocks(int tileId)
    {
        return _blockableTiles.Contains(tileId) ? Tile.ColliderType.Grid : Tile.ColliderType.None;
    }
}
