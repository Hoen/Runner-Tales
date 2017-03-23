using UnityEngine;

public class PrototypeCharacter : PlayableCharacter
{
    // Pouvoir spécial unique au personnage
    protected override void ActivatePower ()
    {
        // Le pouvoir est identique au fennec : le personnage court moins vite.
        if (!isPowerActive & power >= 100)
        {
            actualSpeed = baseSpeed * 0.75f;
            isPowerActive = true;
        }
    }

    protected override void DeactivatePower ()
    {
        actualSpeed = baseSpeed;
        isPowerActive = false;
    }
}
