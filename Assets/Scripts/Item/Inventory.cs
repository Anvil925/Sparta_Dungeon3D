using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;  // 싱글턴 패턴 사용

    private Dictionary<ItemData, int> items = new Dictionary<ItemData, int>(); // 아이템과 개수를 저장하는 딕셔너리

    private void Awake()
    {
        if (Instance == null) Instance = this; 
        else Destroy(gameObject);
    }

    public void AddItem(ItemData item)
    {
        if (items.ContainsKey(item)) 
            items[item]++;
        else
            items.Add(item, 1);
    }

    public void UseItem(ItemData item)
    {
        if (items.ContainsKey(item) && items[item] > 0)
        {
            items[item]--;
            Debug.Log($"{item.displayName} 사용! 남은 개수: {items[item]}");

            if (items[item] <= 0)
                items.Remove(item);
        }
    }

    public int GetItemCount(ItemData item)
    {
        return items.ContainsKey(item) ? items[item] : 0;
    }
}