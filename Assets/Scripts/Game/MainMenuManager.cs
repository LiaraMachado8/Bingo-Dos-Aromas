using UnityEngine;
using UnityEngine.UI;

// ============================================================
//  MainMenuManager.cs
//  Coloca num GameObject vazio chamado "MainMenuManager"
//  na scene MainMenu.
//
//  Hierarquia da scene MainMenu:
//  Canvas
//   ├── Background       (Image)
//   ├── TituloText       (TextMeshProUGUI)
//   └── BtnJogar         (Button)
// ============================================================

public class MainMenuManager : MonoBehaviour
{
    [Header("Botões")]
    public Button btnJogar;

    private void Awake()
    {
        btnJogar.onClick.AddListener(SceneLoader.IrParaBingo);
    }
}