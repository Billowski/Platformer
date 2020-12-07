using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{

    public int maxHealth;
    public int currentHealth;
    public float invincibilityLength;
    private float invincibilityCounter;
    public PlayerController thePlayer;
    public Renderer playerRenderer;
    private float flashCounter;
    public float flashLenght = 0.1f;
    private bool isRespawning;
    private Vector3 respawnPoint;
    public float respawnLenght;
    public GameObject deathEffect;
    public Image blackScreen;
    private bool isFadeToBlack;
    private bool isFadeFromBlack;
    public float fadeSpeed;
    public float waitForFade;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        //thePlayer = FindObjectOfType<PlayerController>();

        respawnPoint = thePlayer.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(invincibilityCounter > 0)
        {
            invincibilityCounter -= Time.deltaTime;

            flashCounter -= Time.deltaTime;
            if (flashCounter <= 0)
            {
                playerRenderer.enabled = !playerRenderer.enabled;
                flashCounter = flashLenght;
            }

            if(invincibilityCounter <= 0)
            {
                playerRenderer.enabled = true;
            }
        }
        if(isFadeToBlack)
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if(blackScreen.color.a == 1f)
            {
                isFadeToBlack = false;
            }
        }

        if (isFadeFromBlack)
        {
            blackScreen.color = new Color(blackScreen.color.r, blackScreen.color.g, blackScreen.color.b, Mathf.MoveTowards(blackScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (blackScreen.color.a == 0f)
            {
                isFadeFromBlack = false;
            }
        }
    }

    public void Respawn()
    {
        //CharacterController charController = thePlayer.GetComponent<CharacterController>();
        //charController.enabled = false;
        //thePlayer.transform.position = respawnPoint;
        //charController.enabled = true;
        //currentHealth = maxHealth;
        if (!isRespawning)
        {
            StartCoroutine("RespawnCo");
        }
    }

    public IEnumerator RespawnCo()
    {
        isRespawning = true;
        thePlayer.gameObject.SetActive(false);
        Instantiate(deathEffect, thePlayer.transform.position, thePlayer.transform.rotation);

        yield return new WaitForSeconds(respawnLenght);

        isFadeToBlack = true;

        yield return new WaitForSeconds(waitForFade);

        isFadeToBlack = false;
        isFadeFromBlack = true;

        isRespawning = false;

        thePlayer.gameObject.SetActive(true);
        
        CharacterController charController = thePlayer.GetComponent<CharacterController>();
        charController.enabled = false;
        thePlayer.transform.position = respawnPoint;
        charController.enabled = true;
        currentHealth = maxHealth;
        invincibilityCounter = invincibilityLength;
        playerRenderer.enabled = false;
        flashCounter = flashLenght;

    }
    public void HurtPlayer(int damage, Vector3 direction)
    {
        if (invincibilityCounter <= 0)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                Respawn();
            }
            else
            {

                thePlayer.Knockback(direction);

                invincibilityCounter = invincibilityLength;

                playerRenderer.enabled = false;

                flashCounter = flashLenght;
            }
        }
    }
    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }
}
