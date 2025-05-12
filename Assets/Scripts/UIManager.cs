using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindObjectOfType<Canvas>();
        
    }
    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTakeDamage;
        CharacterEvents.characterHealed += CharacterHeal;
    }
    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTakeDamage;
        CharacterEvents.characterHealed -= CharacterHeal;
    }
    public void CharacterTakeDamage(GameObject character, float damageReceived)
    {
        //tao ra text luc bi tan cong
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();
        tmpText.text = damageReceived.ToString();
    }
    public void CharacterHeal(GameObject character, float healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();
        tmpText.text = healthRestored.ToString();
    }
}
