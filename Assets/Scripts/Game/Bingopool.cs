using UnityEngine;
using System.Collections.Generic;

// ============================================================
//  BingoPool.cs
//  ScriptableObject onde defines todas as imagens do sorteio.
//
//  Como criar:
//  Clica direito em Assets → Create → Bingo → BingoPool
//  No Inspector clica + para adicionar cada imagem (nome + sprite).
// ============================================================

[CreateAssetMenu(fileName = "BingoPool", menuName = "Bingo/BingoPool")]
public class BingoPool : ScriptableObject
{
    [System.Serializable]
    public class BingoItem
    {
        public string nome;
        public Sprite sprite;
    }

    [Tooltip("Adiciona aqui todas as imagens do sorteio")]
    public List<BingoItem> imagens = new List<BingoItem>();
}