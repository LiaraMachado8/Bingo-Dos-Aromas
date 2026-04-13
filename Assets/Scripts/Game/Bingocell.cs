using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ============================================================
//  BingoCell.cs
//  Componente colocado em cada célula da cartela (25 ao total).
//  Hierarquia sugerida por célula:
//    Cell (Button)
//      └── CellImage (Image)         <- sprite da imagem
//      └── CellLabel (TextMeshProUGUI) <- nome opcional embaixo
//      └── MarkedOverlay (Image)     <- overlay semi-transparente ao marcar
// ============================================================

public class BingoCell : MonoBehaviour
{
    [Header("Referências UI")]
    public Image cellImage;       // Image que exibe o sprite
    public TextMeshProUGUI cellLabel; // Nome da imagem (pode ser nulo)
    public Image markedOverlay;   // Overlay colorido ao marcar
    public Image winOverlay;      // Overlay dourado ao fazer bingo

    [HideInInspector] public BingoData data;   // Imagem associada a esta célula
    [HideInInspector] public bool isFreeSpace; // Célula central (sempre marcada)
    [HideInInspector] public bool isMarcado;   // Está marcada?

    // Cores dos overlays (ajuste pelo Inspector do BingoManager)
    [Header("Cores")]
    public Color corMarcado = new Color(0.2f, 0.8f, 0.4f, 0.55f);
    public Color corBingo = new Color(1f, 0.85f, 0f, 0.65f);
    public Color corFreeSpace = new Color(0.3f, 0.6f, 1f, 0.55f);

    // ----------------------------------------------------------
    //  Inicialização
    // ----------------------------------------------------------

    /// <summary>
    /// Configura célula normal com uma BingoData.
    /// </summary>
    public void Setup(BingoData bingoData)
    {
        data = bingoData;
        isFreeSpace = false;
        isMarcado = false;

        if (cellImage != null) cellImage.sprite = bingoData.sprite;
        if (cellLabel != null) cellLabel.text = bingoData.nomeDaImagem;

        AtualizarVisual();
    }

    /// <summary>
    /// Configura a célula central (FREE SPACE) — já nasce marcada.
    /// </summary>
    public void SetupFreeSpace(Sprite starSprite = null)
    {
        data = null;
        isFreeSpace = true;
        isMarcado = true;

        if (cellImage != null) cellImage.sprite = starSprite;
        if (cellLabel != null) cellLabel.text = "FREE";

        AtualizarVisual();
    }

    // ----------------------------------------------------------
    //  Lógica de marcação
    // ----------------------------------------------------------

    /// <summary>
    /// Chamado pelo BingoManager quando esta imagem é sorteada.
    /// </summary>
    public void Marcar()
    {
        if (isMarcado) return;
        isMarcado = true;
        AtualizarVisual();
    }

    /// <summary>
    /// Destaque dourado quando a linha/coluna/diagonal completa o bingo.
    /// </summary>
    public void DestacarBingo()
    {
        if (winOverlay != null)
        {
            winOverlay.color = corBingo;
            winOverlay.enabled = true;
        }
    }

    // ----------------------------------------------------------
    //  Visual
    // ----------------------------------------------------------

    private void AtualizarVisual()
    {
        if (markedOverlay == null) return;

        if (isFreeSpace)
        {
            markedOverlay.color = corFreeSpace;
            markedOverlay.enabled = true;
        }
        else
        {
            markedOverlay.color = corMarcado;
            markedOverlay.enabled = isMarcado;
        }
    }

    /// <summary>
    /// Reset visual (usado no Restart).
    /// </summary>
    public void ResetCell()
    {
        isMarcado = false;
        if (markedOverlay != null) markedOverlay.enabled = false;
        if (winOverlay != null) winOverlay.enabled = false;
    }
}