using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotItemPrefab : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemText;

    public void ItemSetting(Color itemSprite, string txt)
    {
        itemImage.color = itemSprite;
        itemText.text = txt;
    }

}
