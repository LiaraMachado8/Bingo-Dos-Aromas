using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
//  BingoHistoricoItem.cs
//  Prefab instanciado dinamicamente no scroll de histórico.
//
//  Hierarquia sugerida do prefab:
//    HistoricoItem (VerticalLayoutGroup)
//      └── ItemImage  (Image)            <- sprite sorteado
//      └── ItemLabel  (TextMeshProUGUI)  <- nome + número de ordem
//      └── LatestBadge (GameObject)      <- badge "Atual" (ativo só no mais recente)
// ============================================================

public class BingoHistoricoItem : MonoBehaviour
{
    [Header("Referências UI")]
    public Image itemImage;
    public TextMeshProUGUI itemLabel;
    public GameObject latestBadge; // exibe "Atual" no item mais recente

    /// <summary>
    /// Preenche o item com os dados da imagem sorteada.
    /// </summary>
    /// <param name="data">BingoData sorteada</param>
    /// <param name="numero">Número de ordem no sorteio (1, 2, 3...)</param>
    /// <param name="isLatest">True apenas para o item mais recente</param>
    public void Setup(BingoData data, int numero, bool isLatest)
    {
        if (itemImage != null)
            itemImage.sprite = data.sprite;

        if (itemLabel != null)
            itemLabel.text = $"#{numero}\n{data.nomeDaImagem}";

        if (latestBadge != null)
            latestBadge.SetActive(isLatest);
    }
}