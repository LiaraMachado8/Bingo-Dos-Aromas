using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

// ============================================================
//  BingoManager.cs
//  Controlador principal do jogo. Coloque este script num
//  GameObject vazio chamado "BingoManager" na cena.
//
//  HIERARQUIA DE CENA SUGERIDA:
//  ─────────────────────────────────────────────────────────
//  Canvas
//   ├── Header
//   │    └── TituloText (TMP)
//   ├── SorteioAtual
//   │    ├── SorteioImage (Image)        <- sprite atual
//   │    └── SorteioNomeText (TMP)       <- nome da imagem atual
//   ├── BotaoProxima (Button)
//   ├── BotaoRestart (Button)
//   ├── StatusText (TMP)                 <- "5 / 75 sorteadas"
//   ├── BingoText (TMP)                  <- "🎉 BINGO!" (inicialmente inativo)
//   ├── Cartela (GridLayoutGroup, 5x5)
//   │    └── Cell_00 ... Cell_24        <- 25 filhos com BingoCell
//   └── HistoricoScroll (ScrollRect)
//        └── Viewport
//             └── Content (VerticalLayoutGroup)
//                  └── [instâncias de BingoHistoricoItem]
// ─────────────────────────────────────────────────────────

public class BingoManager : MonoBehaviour
{
    // ----------------------------------------------------------
    //  Referências — arraste pelo Inspector
    // ----------------------------------------------------------

    [Header("Dados")]
    [Tooltip("Arraste o ScriptableObject BingoPool aqui")]
    public BingoPool bingoPool;

    [Header("Cartela")]
    [Tooltip("Arraste os 25 GameObjects da cartela (ordem: linha por linha, topo→base)")]
    public BingoCell[] celulas = new BingoCell[25];

    [Tooltip("Sprite exibido na célula FREE (centro). Pode ser null.")]
    public Sprite freeSpaceSprite;

    [Header("Sorteio Atual")]
    public Image sorteioImage;   // grande imagem sorteada
    public TextMeshProUGUI sorteioNome;    // nome da imagem sorteada
    public Sprite placeholderSprite; // exibido antes do primeiro sorteio

    [Header("Histórico (Scroll)")]
    [Tooltip("Prefab com BingoHistoricoItem")]
    public GameObject historicoItemPrefab;
    [Tooltip("Transform Content do ScrollRect")]
    public Transform historicoContent;
    [Tooltip("Referência ao ScrollRect para rolar automaticamente ao topo")]
    public ScrollRect historicoScrollRect;

    [Header("Botões")]
    public Button btnProxima;
    public Button btnRestart;

    [Header("Textos de Status")]
    public TextMeshProUGUI statusText;
    public GameObject bingoObj; // GameObject com o texto/painel de BINGO

    // ----------------------------------------------------------
    //  Estado interno
    // ----------------------------------------------------------

    private List<BingoData> _pool = new List<BingoData>();  // pool embaralhado
    private List<BingoData> _sorteadas = new List<BingoData>();  // já sorteadas
    private List<GameObject> _historicoItens = new List<GameObject>(); // itens instanciados
    private bool _bingoAlcancado = false;

    // Linhas, colunas e diagonais para verificar bingo (índices 0–24)
    private static readonly int[][] _linhas = new int[][]
    {
        new[]{0,1,2,3,4},       // linha 0
        new[]{5,6,7,8,9},       // linha 1
        new[]{10,11,12,13,14},  // linha 2
        new[]{15,16,17,18,19},  // linha 3
        new[]{20,21,22,23,24},  // linha 4
        new[]{0,5,10,15,20},    // coluna 0
        new[]{1,6,11,16,21},    // coluna 1
        new[]{2,7,12,17,22},    // coluna 2
        new[]{3,8,13,18,23},    // coluna 3
        new[]{4,9,14,19,24},    // coluna 4
        new[]{0,6,12,18,24},    // diagonal principal
        new[]{4,8,12,16,20},    // diagonal secundária
    };

    // ----------------------------------------------------------
    //  Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        btnProxima.onClick.AddListener(SortearProxima);
        btnRestart.onClick.AddListener(Restart);
    }

    private void Start()
    {
        IniciarJogo();
    }

    // ----------------------------------------------------------
    //  Inicialização / Restart
    // ----------------------------------------------------------

    private void IniciarJogo()
    {
        // Validação básica
        if (bingoPool == null || bingoPool.todasAsImagens.Count < 25)
        {
            Debug.LogError("BingoManager: BingoPool precisa ter ao menos 25 imagens!");
            return;
        }

        _bingoAlcancado = false;
        _sorteadas.Clear();

        // Embaralha o pool
        _pool = Embaralhar(bingoPool.todasAsImagens);

        // Sorteia 24 imagens únicas para a cartela (posição 12 = FREE SPACE)
        List<BingoData> imagensCartela = _pool.Take(24).ToList();
        imagensCartela = Embaralhar(imagensCartela);

        // Configura as 25 células
        int idx = 0;
        for (int i = 0; i < 25; i++)
        {
            if (i == 12)
            {
                celulas[i].SetupFreeSpace(freeSpaceSprite);
            }
            else
            {
                celulas[i].Setup(imagensCartela[idx]);
                idx++;
            }
        }

        // Limpa histórico
        LimparHistorico();

        // UI inicial
        if (sorteioImage != null) sorteioImage.sprite = placeholderSprite;
        if (sorteioNome != null) sorteioNome.text = "Aguardando sorteio...";
        if (bingoObj != null) bingoObj.SetActive(false);

        AtualizarStatus();
        btnProxima.interactable = true;
    }

    public void Restart()
    {
        // Reseta visual de todas as células
        foreach (var c in celulas) c.ResetCell();
        IniciarJogo();
    }

    // ----------------------------------------------------------
    //  Sorteio
    // ----------------------------------------------------------

    public void SortearProxima()
    {
        // Filtra imagens que ainda não foram sorteadas
        var restantes = _pool.Where(p => !_sorteadas.Contains(p)).ToList();

        if (restantes.Count == 0)
        {
            btnProxima.interactable = false;
            return;
        }

        BingoData sorteada = restantes[0]; // próxima da lista embaralhada
        _sorteadas.Add(sorteada);

        // Atualiza painel de sorteio atual
        if (sorteioImage != null) sorteioImage.sprite = sorteada.sprite;
        if (sorteioNome != null) sorteioNome.text = sorteada.nomeDaImagem;

        // Marca células da cartela que contenham esta imagem
        foreach (var celula in celulas)
        {
            if (!celula.isFreeSpace && celula.data == sorteada)
                celula.Marcar();
        }

        // Adiciona ao histórico
        AdicionarHistorico(sorteada);

        // Verifica bingo
        if (!_bingoAlcancado)
            VerificarBingo();

        AtualizarStatus();

        if (_sorteadas.Count >= _pool.Count)
            btnProxima.interactable = false;
    }

    // ----------------------------------------------------------
    //  Histórico
    // ----------------------------------------------------------

    private void AdicionarHistorico(BingoData data)
    {
        // Remove badge "Atual" do item anterior
        if (_historicoItens.Count > 0)
        {
            var anterior = _historicoItens[_historicoItens.Count - 1]
                               .GetComponent<BingoHistoricoItem>();
            if (anterior != null) anterior.Setup(
                _sorteadas[_sorteadas.Count - 2],
                _sorteadas.Count - 1,
                false);
        }

        // Instancia novo item no topo do Content
        GameObject item = Instantiate(historicoItemPrefab, historicoContent);
        item.transform.SetAsFirstSibling(); // mais recente no topo

        var comp = item.GetComponent<BingoHistoricoItem>();
        if (comp != null) comp.Setup(data, _sorteadas.Count, true);

        _historicoItens.Add(item);

        // Rola scroll para o topo
        if (historicoScrollRect != null)
            Canvas.ForceUpdateCanvases();
        historicoScrollRect.verticalNormalizedPosition = 1f;
    }

    private void LimparHistorico()
    {
        foreach (var item in _historicoItens)
            if (item != null) Destroy(item);

        _historicoItens.Clear();
    }

    // ----------------------------------------------------------
    //  Verificação de Bingo
    // ----------------------------------------------------------

    private void VerificarBingo()
    {
        foreach (var linha in _linhas)
        {
            bool completa = true;
            foreach (int i in linha)
            {
                if (!celulas[i].isMarcado) { completa = false; break; }
            }

            if (completa)
            {
                _bingoAlcancado = true;

                // Destaca as células vencedoras
                foreach (int i in linha)
                    celulas[i].DestacarBingo();

                // Exibe painel de bingo
                if (bingoObj != null) bingoObj.SetActive(true);

                break;
            }
        }
    }

    // ----------------------------------------------------------
    //  Utilitários
    // ----------------------------------------------------------

    private void AtualizarStatus()
    {
        if (statusText != null)
            statusText.text = $"{_sorteadas.Count} / {_pool.Count} sorteadas";
    }

    /// <summary>
    /// Fisher-Yates shuffle genérico.
    /// </summary>
    private List<T> Embaralhar<T>(List<T> lista)
    {
        List<T> copia = new List<T>(lista);
        for (int i = copia.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (copia[i], copia[j]) = (copia[j], copia[i]);
        }
        return copia;
    }
}