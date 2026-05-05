using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
//  HistoricoItem.cs
//  Componente do prefab instanciado no scroll de histórico.
//
//  Hierarquia do prefab:
//  HistoricoItem  (HistoricoItem + LayoutElement)
//   ├── ItemImagem   (Image)
//   └── ItemNome     (TextMeshProUGUI)
// ============================================================

public class HistoricoItem : MonoBehaviour
{
    public Image itemImagem;
    public TextMeshProUGUI itemNome;

    public void Setup(BingoPool.BingoItem item, int numero)
    {
        if (itemImagem != null) itemImagem.sprite = item.sprite;
        if (itemNome != null) itemNome.text = $"#{numero} {item.nome}";
    }
}