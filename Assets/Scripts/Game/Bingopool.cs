using UnityEngine;
using System.Collections.Generic;

// ============================================================
//  BingoPool.cs
//  ScriptableObject que agrupa TODAS as BingoDatas do jogo.
//  Como criar: clique direito na pasta Assets > Create > Bingo > BingoPool
// ============================================================

[CreateAssetMenu(fileName = "BingoPool", menuName = "Bingo/BingoPool")]
public class BingoPool : ScriptableObject
{
    [Tooltip("Arraste aqui todos os BingoData que compõem o pool de sorteio")]
    public List<BingoData> todasAsImagens = new List<BingoData>();
}