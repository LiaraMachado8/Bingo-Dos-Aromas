using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public const string CENA_MENU = "MainMenu";
    public const string CENA_BINGO = "Bingo";

    public static void IrParaMenu() => SceneManager.LoadScene(CENA_MENU);
    public static void IrParaBingo() => SceneManager.LoadScene(CENA_BINGO);
}