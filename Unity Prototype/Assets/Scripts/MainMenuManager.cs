using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Les différents menus
    private GameObject transitionSlider;
    private GameObject mainMenu;
    private GameObject levelSelectMenu;
    private GameObject optionsMenu;

    // Éléments de transition
    private RectTransform mask;
    private RectTransform pattern;

    // Éléments du menu des options
    private Slider soundVolumeSlider;
    private Text soundVolumeText;
    private Slider musicVolumeSlider;
    private Text musicVolumeText;

    // Sélection du personnage
    private int characterIndex = 0;
    private Text characterName;

    void Start ()
    {
        // Récupération de chaque objet présent dans la scène afin de pouvoir interagir avec eux plus tard. Les noms des objets sont utilisés.
        transitionSlider = GameObject.Find ("Menus Background");
        mainMenu = GameObject.Find ("Main Menu");
        levelSelectMenu = GameObject.Find ("Level Selection Menu");
        optionsMenu = GameObject.Find ("Options Menu");

        // Éléments de transition
        mask = GameObject.Find ("Mask").GetComponent<RectTransform> ();
        pattern = GameObject.Find ("Pattern").GetComponent<RectTransform> ();

        // Éléments du menu des options
        soundVolumeSlider = GameObject.Find ("Sound Volume Slider").GetComponent<Slider> ();
        soundVolumeText = GameObject.Find ("Sound Volume Value Text").GetComponent<Text> ();
        musicVolumeSlider = GameObject.Find ("Music Volume Slider").GetComponent<Slider> ();
        musicVolumeText = GameObject.Find ("Music Volume Value Text").GetComponent<Text> ();

        // Éléments du menu de sélection
        characterName = GameObject.Find ("Character Name").GetComponent<Text> ();
        characterName.text = DataManager.singleton.characterPrefabs[characterIndex].ToString ();

        // On affiche le menu principal en premier.
        ShowMainMenu ();
        mask.anchoredPosition = new Vector2 (-2400, 0);
        transitionSlider.SetActive (false);
    }

    // Fonctions d'affichage des différents menus
    void HideMenus ()
    {
        mainMenu.SetActive (false);
        levelSelectMenu.SetActive (false);
        optionsMenu.SetActive (false);
    }

    public void ShowMainMenu ()
    {
        HideMenus ();
        mainMenu.SetActive (true);
    }

    public void ShowLevelSelectionMenu ()
    {
        HideMenus ();
        levelSelectMenu.SetActive (true);
    }

    public void ShowOptionsMenu ()
    {
        HideMenus ();
        UpdateOptionsMenuValues ();
        optionsMenu.SetActive (true);
    }

    public void ShowMenu (string newMenu)
    {
        StartCoroutine (SlideTransition (newMenu));
    }

    IEnumerator SlideTransition (string newMenu)
    {
        transitionSlider.SetActive (true);

        for (int i = -30; i < 30; i++)
        {
            mask.anchoredPosition = new Vector2 (i * 70, 0);
            pattern.anchoredPosition = new Vector2 (0 - (i * 70), 0);

            if (i == 0)
            {
                HideMenus ();

                switch (newMenu)
                {
                    default:
                        mainMenu.SetActive (true);
                        break;

                    case "OPTIONS":
                        UpdateOptionsMenuValues ();
                        optionsMenu.SetActive (true);
                        break;

                    case "LEVEL":
                        levelSelectMenu.SetActive (true);
                        break;
                }
            }

            yield return new WaitForSecondsRealtime (0.005f);
        }

        transitionSlider.SetActive (false);
    }

    // Modification du personnage joué
    public void CharacterSelection (int indexModifier)
    {
        // La modification de l'index relatif au personnage ne peut être que de 1 ou -1;
        if (indexModifier == 0)
            indexModifier = 1;

        characterIndex += Mathf.Clamp (indexModifier, -1, 1);

        // Le nouvel index doit être compris dans les limites de la liste des personnages disponibles.
        if (characterIndex < 0)
            characterIndex = DataManager.singleton.characterPrefabs.Length - 1;
        else if (characterIndex >= DataManager.singleton.characterPrefabs.Length)
            characterIndex = 0;

        // On affiche le nouveau personnage choisi.
        DataManager.singleton.chosenCharacter = characterIndex;
        characterName.text = DataManager.singleton.characterPrefabs[characterIndex].ToString ();
    }

    // Fonctions relatives au menu des options
    void UpdateOptionsMenuValues ()
    {
        // Volume des effets sonores
        soundVolumeSlider.value = DataManager.singleton.soundVolume;
        soundVolumeText.text = DataManager.singleton.soundVolume.ToString ();

        // Volume des musiques
        musicVolumeSlider.value = DataManager.singleton.musicVolume;
        musicVolumeText.text = DataManager.singleton.musicVolume.ToString ();
    }

    public void UpdateAudioVolumesTexts ()
    {
        soundVolumeText.text = soundVolumeSlider.value.ToString ();
        musicVolumeText.text = musicVolumeSlider.value.ToString ();
    }

    public void SaveOptionsMenuValues ()
    {
        DataManager.singleton.soundVolume = (int) soundVolumeSlider.value;
        DataManager.singleton.musicVolume = (int) musicVolumeSlider.value;
    }

    // Sortie des menus pour aller vers le jeu en lui-même
    public void ExitToGame ()
    {
        SceneManager.LoadScene ("Level 1-1");
    }

    // Chargement d'un niveau spécifique
    public void LoadLevel (string newLevel)
    {
        SceneManager.LoadScene ("Level " + newLevel);
    }
}
