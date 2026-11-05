using System;
using System.Collections.Generic;
using UnityEngine;
using Enums;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipes/New Recipe")]
public class Recipe : ScriptableObject
{
    public List<CraftableUsing> ingredientRestrictions;
    public List<Ingredient> ingredients;
    public int resultItemId;
    public int resultItemCount;
    public float timeToCreate;
}

[Serializable]
public class Ingredient
{
    public int itemId;
    public int itemCountNeeded;
}