using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UISuggestionSwitcher : MonoBehaviour
{
    private Image spriteToSwitch;

    public Sprite keyboardScheme;
    public Sprite controllerScheme;

    private playerScheme currentScheme;


    private void Awake()
    {
        setup();
    }

    private async Task setup()
    {
        // Wait for these values GameController needs to exist and be enabled.
        while (PlayerDataHolder.Instance == null || PlayerDataHolder.Instance.enabled == false || GameController.Instance.initialized == false)
        {
            await Task.Delay(5);
        }

        // Once it finds it initialize the scene
        Debug.Log("Initializing Lobby Manager...         [Lobby Manager]");
        StartCoroutine(StartSwitcher());

        return;
    }

    private IEnumerator StartSwitcher()
    {
        // Get the Host Scheme
        spriteToSwitch = this.GetComponent<Image>();
        
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if(PlayerDataHolder.Instance == null)
        {
            yield return null;
        }
        else
        {
            PlayerData data = PlayerDataHolder.Instance.GetHost();
            string scheme = data.input.currentControlScheme;

            Debug.Log("The Host Scheme is: " + scheme);

            switch (scheme)
            {
                case "Keyboard&Mouse":
                    currentScheme = playerScheme.Keyboard;
                    spriteToSwitch.sprite = keyboardScheme;
                    break;
                case "Gamepad":
                    currentScheme = playerScheme.Controller;
                    spriteToSwitch.sprite = controllerScheme;
                    break;
            }
        
            // Change the Sprite based on the scheme changed
        }
    }

    private enum playerScheme
    {
        Keyboard,
        Controller
    }
}
