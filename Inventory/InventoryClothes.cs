using System.Collections.Generic;
using HamsterCombat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class InventoryClothes : InventoryGrid
{
    private readonly Target _target;

    public InventoryClothes(
        InventorySteamworks inventorySteamworks,
        List<GameObject> inventoryCells,
        TMP_Text nameText,
        TMP_Text descriptionText,
        Button button,
        Button nextPageButton,
        Button previousPageButton,
        Sprite invisibleSprite,
        List<int> defaultItemsIds,
        Target target,
        Dictionary<InventoryItemRarityType, Color> rarityColors)
        : base(
            inventorySteamworks,
            inventoryCells,
            nameText,
            descriptionText,
            button,
            nextPageButton,
            previousPageButton,
            invisibleSprite,
            defaultItemsIds,
            rarityColors)
    {
        _target = target;
        _inventoryItemsType = InventoryItemType.Clothes;
    }

    protected override void ShowItemInfo(int itemNumber)
    {
        base.ShowItemInfo(itemNumber);

        if (!_button.gameObject.activeSelf)
        {
            _button.gameObject.SetActive(true);
        }

        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() =>
        {
            PutOnClother(itemNumber);
        });

        _buttonText.text = Localization.Instance["Put"];
    }

    private void PutOnClother(int clotherNumber)
    {
        _target.ChangeTarget(_items[clotherNumber]);
    }
}
