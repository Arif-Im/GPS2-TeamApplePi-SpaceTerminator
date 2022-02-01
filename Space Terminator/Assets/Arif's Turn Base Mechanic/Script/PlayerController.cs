using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Unit unit;
    BattleSystem battleSystem;
    Rigidbody rb;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        battleSystem = FindObjectOfType<BattleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!unit.isCurrentTurn) { return; }

        if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0)
        {
            rb.velocity = new Vector3(Input.GetAxisRaw("Horizontal") * unit.speed, 0, Input.GetAxisRaw("Vertical") * unit.speed);
        }

        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Perform an Action");
            battleSystem.ChangeTurn(unit);
        }
    }
}
