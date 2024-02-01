using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class StatisticsCanvas : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI resultText;
    [SerializeField] TextMeshProUGUI totalText;
    [SerializeField] int totalResult;
    public List<DiceRoller> diceRollers = new List<DiceRoller>();

    private void Start()
    {
        for (int i = 0; i < diceRollers.Count; i++)
        {
            diceRollers[i].OnRollOver += OnDiceRollOver;
        }
        totalText.text = "?";
        totalText.text = totalResult.ToString();
    }

    private void OnDiceRollOver(int result)
    {
        resultText.text = result.ToString();
        totalResult += result;
        totalText.text = totalResult.ToString();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < diceRollers.Count; i++)
        {
            diceRollers[i].OnRollOver -= OnDiceRollOver;
        }
    }
}
