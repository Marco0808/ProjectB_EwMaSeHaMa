using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncounterPopup : MonoBehaviour
{
    public static event Action<bool> OnEncounterSolution;

    public void CooperatePressed()
    {
        OnEncounterSolution?.Invoke(true);
    }

    public void ConflictPressed()
    {
        OnEncounterSolution?.Invoke(false);
    }
}
