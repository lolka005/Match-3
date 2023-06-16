using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System;

public sealed class Board : MonoBehaviour
{
    public static Board Instance { get; private set; }
    public Row[] rows;
    bool[,] boolTitle = new bool[9, 9];
    public Tile[,] Tiles { get; private set; }
    private readonly List<Tile> _selection = new List<Tile>();

    private const float TweenDuration = 0.15f;

    public int Width => Tiles.GetLength(0);
    public int Height => Tiles.GetLength(1);

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Tiles = new Tile[rows.Max(row => row.tiles.Length), rows.Length];
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var tile = rows[y].tiles[x];
                tile.x = x;
                tile.y = y;
                tile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0,ItemDatabase.Items.Length)];
                Tiles[x, y] = rows[y].tiles[x];
            }
        }
    }

    public async void Select(Tile tile)
    {
        if (!_selection.Contains(tile))
        {
            if (_selection.Count > 0)
            {
                if (Array.IndexOf(_selection[0].Neighbours, tile) != -1)
                {
                    _selection.Add(tile);
                }
            }
            else
            {
                _selection.Add(tile);
            }
        }
        


        if (_selection.Count < 2) return;
        await Swap(_selection[0], _selection[1]);
        if (CanPop())
        {
            Pop();
        }
        else
        {
            await Swap(_selection[0], _selection[1]);
        }
        _selection.Clear();
    }

    public async Task Swap(Tile tile1, Tile tile2)
    {
        var icon1 = tile1.icon;
        var icon2 = tile2.icon;
        var icon1Transform = icon1.transform;
        var icon2Transform = icon2.transform;

        var sequence = DOTween.Sequence();
        sequence.Join(icon1Transform.DOMove(icon2Transform.position, TweenDuration)).Join(icon2Transform.DOMove(icon1Transform.position, TweenDuration));
        await sequence.Play().AsyncWaitForCompletion();
        icon1Transform.SetParent(tile2.transform);
        icon2Transform.SetParent(tile1.transform);
        tile1.icon = icon2;
        tile2.icon = icon1;

        var tileItem = tile1.Item;
        tile1.Item = tile2.Item;
        tile2.Item = tileItem;
    }

    private bool CanPop()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                if (Tiles[x, y].GetConnectetTilesTopBottom().Skip(1).Count() >=2 || Tiles[x, y].GetConnectetTilesLeftRight().Skip(1).Count() >= 2) return true;
        return false;
    }

    private async void Pop()
    {
        MyTimer.Instance.Stop();
        for (var y = 0; y < Height; y++)
            for(var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var connectetTiles = tile.GetConnectetTilesTopBottom();
                if (connectetTiles.Skip(1).Count() < 2) continue;
                var deflateSequence = DOTween.Sequence();
                foreach (var connectetTile in connectetTiles)
                {
                    deflateSequence.Join(connectetTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                    boolTitle[connectetTile.x, connectetTile.y] = true;
                }
                GameScore.Instance.Score += tile.Item.value * connectetTiles.Count;
                await deflateSequence.Play().AsyncWaitForCompletion();
            }
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var connectetTiles = tile.GetConnectetTilesLeftRight();
                if (connectetTiles.Skip(1).Count() < 2) continue;
                var deflateSequence = DOTween.Sequence();
                foreach (var connectetTile in connectetTiles)
                {
                    deflateSequence.Join(connectetTile.icon.transform.DOScale(Vector3.zero, TweenDuration));
                    boolTitle[connectetTile.x, connectetTile.y] = true;
                }
                GameScore.Instance.Score += tile.Item.value * connectetTiles.Count;
                await deflateSequence.Play().AsyncWaitForCompletion();
            }
        await Down();
        MyTimer.Instance.time = 5;
        MyTimer.Instance.StartTimer();
        ShowAll();
        await Fill();
    }

    private async Task Down()
    {
        for (var y = 8; y >= 0; y--)
            for (var x = 8; x >= 0; x--)
            {
                var tile = Tiles[x, y];
                if (boolTitle[x, y] == true)
                {
                    for(var i = y; i >= 0; i--)
                    {
                        var inflateSequence = DOTween.Sequence();
                        var deflateSequence = DOTween.Sequence();
                        var tile2 = Tiles[x, i];
                        if (boolTitle[x,i] == false)
                        {
                            boolTitle[x, i] = true;
                            boolTitle[x, y] = false;
                            tile.Item = tile2.Item;
                            inflateSequence.Join(tile2.icon.transform.DOScale(Vector3.zero, 0.10f));
                            await inflateSequence.Play().AsyncWaitForCompletion();
                            deflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, 0.10f));
                            await deflateSequence.Play().AsyncWaitForCompletion();
                            break;
                        }
                    }
                }
            }
        if (CanPop())
            Pop();
    }
    private async Task Fill()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                var inflateSequence = DOTween.Sequence();
                if (boolTitle[x, y] == true)
                {
                    tile.Item = ItemDatabase.Items[UnityEngine.Random.Range(0, ItemDatabase.Items.Length)];
                    inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, TweenDuration));
                    boolTitle[x, y] = false;
                }
                await inflateSequence.Play().AsyncWaitForCompletion();
            }
    }
    private void ShowAll()
    {
        var inflateSequence = DOTween.Sequence();
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
            {
                var tile = Tiles[x, y];
                inflateSequence.Join(tile.icon.transform.DOScale(Vector3.one, 0));
            }
    }
}