using UnityEngine;

// ============================================================
//  BingoData.cs
//  ScriptableObject que representa UMA imagem do pool do bingo.
//  Como criar: clique direito na pasta Assets > Create > Bingo > BingoData
// ============================================================

[CreateAssetMenu(fileName = "NovaBingoData", menuName = "Bingo/BingoData")]
public class BingoData : ScriptableObject
{
    [Tooltip("Nome da imagem exibido na UI (ex: Cachorro, Estrela)")]
    public string nomeDaImagem;

    [Tooltip("Sprite que aparece na cartela e no painel de sorteio")]
    public Sprite sprite;
}