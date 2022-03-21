using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static ButtonManager instance;

    public delegate void BeginTurn();
    public BeginTurn onBeginTurn;
    PlayableMovement player;

    bool interactable = false;

    Button attack, lever, overwatch, duck, grenade;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        attack = transform.Find("Attack").GetComponent<Button>();
        lever = transform.Find("Lever").GetComponent<Button>();
        overwatch = transform.Find("Overwatch").GetComponent<Button>();
        duck = transform.Find("Duck").GetComponent<Button>();
        grenade = transform.Find("Grenade").GetComponent<Button>();

        attack.interactable = false;
        lever.interactable = false;
        overwatch.interactable = false;
        duck.interactable = false;
        grenade.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonsToCurrentPlayer(PlayableMovement player)
    {
        interactable = true;
        attack.interactable = true;
        lever.interactable = true;
        overwatch.interactable = true;
        duck.interactable = true;
        grenade.interactable = true;

        this.player = player;

        if (!this.player.gameObject.activeInHierarchy) return;
        attack.onClick.AddListener(() => this.player.SetEnemy());
        overwatch.onClick.AddListener(() => this.player.unit.Activate(() =>
        {
            this.player.unit.isOverwatch = true;
            this.player.unit.overwatchCooldown = 2;
        }));
        duck.onClick.AddListener(() => this.player.unit.Activate(() => this.player.unit.isDucking = true));
        grenade.onClick.AddListener(() => this.player.GetComponent<ScoutMovement>()?.ActivateGrenadeMode());
    }

    public void ResetButtons()
    {
        interactable = false;

        player = null;
        attack.onClick.RemoveAllListeners();
        overwatch.onClick.RemoveAllListeners();
        duck.onClick.RemoveAllListeners();
        grenade.onClick.RemoveAllListeners();

        attack.interactable = false;
        lever.interactable = false;
        overwatch.interactable = false;
        duck.interactable = false;
        grenade.interactable = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(interactable)
            player.overUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (interactable)
            player.overUI = false;
    }
}
