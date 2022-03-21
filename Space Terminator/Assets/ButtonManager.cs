using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;

    public delegate void BeginTurn();
    public BeginTurn onBeginTurn;
    PlayableMovement player;

    Button attack, overwatch, duck, grenade;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        attack = transform.Find("Attack").GetComponent<Button>();
        overwatch = transform.Find("Overwatch").GetComponent<Button>();
        duck = transform.Find("Duck").GetComponent<Button>();
        grenade = transform.Find("Grenade").GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetButtonsToCurrentPlayer(PlayableMovement player)
    {
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
        player = null;
        attack.onClick.RemoveAllListeners();
        overwatch.onClick.RemoveAllListeners();
        duck.onClick.RemoveAllListeners();
        grenade.onClick.RemoveAllListeners();
    }
}
