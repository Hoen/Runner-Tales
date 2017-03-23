using UnityEngine;

public class SheepCharacter : PlayableCharacter
{
    public GameObject woolPrefab;
    private GameObject wool;

    // Pouvoir spécial : une boule de laine est déposée, et le joueur se retrouve téléporté en arrière lorsque la jauge de pouvoir est consommée.
    protected override void ActivatePower ()
    {
        if (!isPowerActive && power >= 100)
        {
            wool = Instantiate (woolPrefab);
            wool.transform.position = transform.position;
            isPowerActive = true;
        }
    }

    protected override void DeactivatePower ()
    {
        bonusScore += (int) Mathf.Abs (transform.position.x - wool.transform.position.x);

        transform.position = wool.transform.position;
        Destroy (wool);
        isPowerActive = false;
    }
}
