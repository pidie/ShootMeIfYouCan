using System;
using UnityEngine;
using UnityEngine.UI;

namespace Loot
{
    public class LootTable : MonoBehaviour
    {
        [SerializeField] private DroppableItem[] item;
        [SerializeField] private int numberOfItemsToDrop;
    }

    public class DroppableItem : MonoBehaviour
    {
        [SerializeField] private Item itemData;
        [SerializeField] private GameObject model;
        [SerializeField] private float chanceToDrop;
    }

    public class Item : ScriptableObject
    {
        [SerializeField] private RawImage icon;
        // list of methods to be called when item picked up
        // [SerializeField] private Action[] methods;
        // list of actions may not show in inspector
        // if not, create an enum and assign each member to a method
    }

    public static class PickUpMethods
    {
        public static void HealthPickUp(float amount = 20f)
        {
            // health += amount
        }

        public static void ArmorPickUp(float amount = 10f)
        {
            // armor += amount
        }
    }
}