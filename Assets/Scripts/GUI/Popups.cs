using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popups : MonoBehaviour {


    public GameObject damagePopupPrefab;
    public GameObject healPopupPrefab;
    public GameObject critPopupPrefab;
    public GameObject missPopupPrefab;


    public GameObject poisonedPopupPrefab;
    public GameObject burnedPopupPrefab;
    public GameObject stunnedPopupPrefab;

    private static int sortingOrder = 1;

    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    //Creation
    public void Create(GameObject parent, int damageAmount, bool damage, bool isCriticalHit) {

        GameObject popupGameObject;
        if (damage)
        {
            if (isCriticalHit) {
                popupGameObject = Instantiate(critPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
            }
            else { 
            popupGameObject = Instantiate(damagePopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
            }
        }
        else {

            popupGameObject = Instantiate(healPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;

        }

        Popups popup = popupGameObject.GetComponent<Popups>();

        popup.Setup(damageAmount);
    }

    public void CreateStatusText(GameObject parent, BaseAttack.StatusEffects status) {

        GameObject popupGameObject;

        switch (status)
        {
            case (BaseAttack.StatusEffects.NONE):
                popupGameObject = Instantiate(healPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
                break;
            case (BaseAttack.StatusEffects.PARALYSIS):
                popupGameObject = Instantiate(healPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
           
                break;
            case (BaseAttack.StatusEffects.BURN):
                popupGameObject = Instantiate(burnedPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
              
                break;
            case (BaseAttack.StatusEffects.POISON):
                popupGameObject = Instantiate(poisonedPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
               
                break;
            case (BaseAttack.StatusEffects.FROSTBURN):
                popupGameObject = Instantiate(healPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
             
                break;
            case (BaseAttack.StatusEffects.STUN):
                popupGameObject = Instantiate(stunnedPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
              
                break;

            default:
                popupGameObject = Instantiate(healPopupPrefab, parent.transform.position, Quaternion.identity, parent.transform) as GameObject;
                break;
        }

        Popups popup = popupGameObject.GetComponent<Popups>();
        popup.SetupText();
    }

    private void Awake()
    {
         textMesh = transform.GetComponent<TextMeshPro>();
        
    }
    public void Setup(int damageAmount) {
        
        textMesh.SetText(damageAmount.ToString());          
        textColor = textMesh.color;

        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;

        disappearTimer = 1f;
    }

    public void SetupText()
    {
        transform.position += new Vector3(0, 0.5f, 0);
        textMesh.SetText(gameObject.GetComponent<TextMeshPro>().text);
        textColor = textMesh.color;
        textMesh.sortingOrder = sortingOrder + 1;

        disappearTimer = 2f;
    }

    private void Update()
    {
        float moveYSpeed = 0.2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (gameObject.name != "PopupManager")
        {
            if (disappearTimer < 0) {
                //Start disappearing
                float disappearSpeed = 3f;
                textColor.a -= disappearSpeed * Time.deltaTime;
                if (transform.GetComponent<TextMeshPro>() != null)
                {
                    textMesh.color = textColor;
                }
                if (textColor.a < 0) {
                    Destroy(gameObject);
                }
            }
        }
        
    }
}
