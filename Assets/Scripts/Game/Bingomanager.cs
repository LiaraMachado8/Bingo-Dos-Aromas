using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// ============================================================
//  BingoManager.cs
//  Controlador principal do jogo de bingo.
//  Coloca num GameObject vazio chamado "BingoManager".
//
//  LAYOUT DA SCENE BINGO:
//  ──────────────────────────────────────────────────────────
//  Canvas
//   ├── LadoEsquerdo          (metade esquerda — imagem atual)
//   │    ├── ImagemAtual      (Image — grande, centro do lado)
//   │    └── NomeAtual        (TextMeshProUGUI — nome da imagem)
//   │
//   ├── LadoDireito           (metade direita — scroll histórico)
//   │    ├── TituloHistorico  (TextMeshProUGUI — "Já sorteados")
//   │    └── ScrollHistorico  (ScrollRect)
//   │         └── Viewport
//   │              └── Content (VerticalLayoutGroup + ContentSizeFitter)
//   │
//   ├── Rodape                (barra inferior com os 3 botões)
//   │    ├── BtnVoltar        (Button — "← Menu")
//   │    ├── BtnProximo       (Button — "Próximo ▶")
//   │    └── BtnBingo         (Button — "BINGO!")
//   │
//   └── PainelVitoria         (painel de celebração — inativo no início)
//        ├── TextoVitoria     (TextMeshProUGUI — "🎉 BINGO!")
//        ├── TextoContagem    (TextMeshProUGUI — "X imagens sorteadas")
//        └── BtnFecharVitoria (Button — "Continuar")
// ──────────────────────────────────────────────────────────

public class BingoManager : MonoBehaviour
{
    // ----------------------------------------------------------
    //  Inspector
    // ----------------------------------------------------------

    [Header("Dados")]
    [Tooltip("Arraste o ScriptableObject BingoPool aqui")]
    public BingoPool bingoPool;

    [Header("Lado Esquerdo — Imagem Atual")]
    public Image imagemAtual;
    public TextMeshProUGUI nomeAtual;
    public Sprite spriteInicial; // exibido antes do primeiro sorteio

    [Header("Lado Direito — Histórico")]
    public GameObject historicoItemPrefab;  // prefab com HistoricoItem
    public Transform historicoContent;     // Content do ScrollRect
    public ScrollRect historicoScrollRect;

    [Header("Botões")]
    public Button btnVoltar;
    public Button btnProximo;
    public Button btnBingo;

    [Header("Painel de Vitória")]
    public GameObject painelVitoria;
    public TextMeshProUGUI textoContagem;   // "X de Y imagens sorteadas"
    public Button btnFecharVitoria;

    // ----------------------------------------------------------
    //  Estado interno
    // ----------------------------------------------------------

    private List<BingoPool.BingoItem> _pool = new();
    private List<BingoPool.BingoItem> _sorteadas = new();
    private List<GameObject> _itensCriados = new();

    // ----------------------------------------------------------
    //  Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        btnVoltar.onClick.AddListener(SceneLoader.IrParaMenu);
        btnProximo.onClick.AddListener(SortearProximo);
        btnBingo.onClick.AddListener(MostrarVitoria);

        if (btnFecharVitoria != null)
            btnFecharVitoria.onClick.AddListener(FecharVitoria);
    }

    private void Start()
    {
        IniciarJogo();
    }

    // ----------------------------------------------------------
    //  Inicialização
    // ----------------------------------------------------------

    private void IniciarJogo()
    {
        if (bingoPool == null || bingoPool.imagens.Count == 0)
        {
            Debug.LogError("BingoManager: BingoPool está vazio ou não foi atribuído!");
            return;
        }

        // Embaralha o pool
        _pool = Embaralhar(bingoPool.imagens);
        _sorteadas.Clear();

        // Limpa histórico
        foreach (var item in _itensCriados)
            if (item != null) Destroy(item);
        _itensCriados.Clear();

        // UI inicial
        if (imagemAtual != null) imagemAtual.sprite = spriteInicial;
        if (nomeAtual != null) nomeAtual.text = "Prima \"Próximo\" para começar";
        if (painelVitoria != null) painelVitoria.SetActive(false);

        btnProximo.interactable = true;
    }

    // ----------------------------------------------------------
    //  Sorteio
    // ----------------------------------------------------------

    private void SortearProximo()
    {
        // Filtra imagens ainda não sorteadas
        var restantes = _pool.Where(p => !_sorteadas.Contains(p)).ToList();

        if (restantes.Count == 0)
        {
            btnProximo.interactable = false;
            if (nomeAtual != null) nomeAtual.text = "Todas as imagens sorteadas!";
            return;
        }

        var sorteada = restantes[0];
        _sorteadas.Add(sorteada);

        // Atualiza imagem grande no lado esquerdo
        if (imagemAtual != null) imagemAtual.sprite = sorteada.sprite;
        if (nomeAtual != null) nomeAtual.text = sorteada.nome;

        // Adiciona ao topo do histórico
        AdicionarHistorico(sorteada);
    }

    // ----------------------------------------------------------
    //  Histórico
    // ----------------------------------------------------------

    private void AdicionarHistorico(BingoPool.BingoItem item)
    {
        var go = Instantiate(historicoItemPrefab, historicoContent);
        go.transform.SetAsFirstSibling(); // mais recente no topo

        var comp = go.GetComponent<HistoricoItem>();
        if (comp != null) comp.Setup(item, _sorteadas.Count);

        _itensCriados.Add(go);

        // Rola para o topo
        if (historicoScrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            historicoScrollRect.verticalNormalizedPosition = 1f;
        }
    }

    // ----------------------------------------------------------
    //  Painel de Vitória
    // ----------------------------------------------------------

    private void MostrarVitoria()
    {
        if (painelVitoria == null) return;

        painelVitoria.SetActive(true);

        if (textoContagem != null)
            textoContagem.text = $"{_sorteadas.Count} de {_pool.Count} imagens sorteadas";
    }

    private void FecharVitoria()
    {
        if (painelVitoria != null)
            painelVitoria.SetActive(false);
    }
    // ----------------------------------------------------------
    //  Utilitários
    // ----------------------------------------------------------

    private List<T> Embaralhar<T>(List<T> lista)
    {
        var copia = new List<T>(lista);
        for (int i = copia.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (copia[i], copia[j]) = (copia[j], copia[i]);
        }
        return copia;
    }
}  // <- fecho da classe BingoManager