using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    static public DataManager singleton;

    public int soundVolume = 100;
    public int musicVolume = 100;

    public GameObject[] characterPrefabs;
    [HideInInspector] public int chosenCharacter = 0;

    // Le singleton doit être déclaré dans la fonction Awake.
    void Awake ()
    {
        // Déclaration du singleton, empêchant la présence de plusieurs DataManager.
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy (gameObject);

        // Objet permanent
        DontDestroyOnLoad (transform.gameObject);
    }

    // Fonction mettant à jour les paramètres audio.
    public void UpdateAudioVolumes (int newSoundVolume, int newMusicVolume)
    {
        soundVolume = Mathf.Clamp (newSoundVolume, 0, 100);
        musicVolume = Mathf.Clamp (newMusicVolume, 0, 100);
    }
}
