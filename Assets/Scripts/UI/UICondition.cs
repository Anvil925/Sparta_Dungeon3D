using UnityEngine;

public class UICondition : MonoBehaviour
{
    public Condition health;
    public Condition energy;
    public Condition water;

    private void Start()
    {
        CharacterManager.Instance.Player.condition.uiCondition = this;
    }
}