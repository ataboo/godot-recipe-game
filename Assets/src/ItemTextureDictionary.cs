using Godot;
using System;
using static RecipeGame.Helpers.Enums;

public class ItemTextureDictionary : Node
{
    [Export]
    public Texture amphibianCharmTexture;
    [Export]
    public Texture berryTexture;
    [Export]
    public Texture bogWaterTexture;
    [Export]
    public Texture burntMuckTexture;
    [Export]
    public Texture caveSlimeTexture;
    [Export]
    public Texture charismaCharmTexture;
    [Export]
    public Texture eyeOfNewtTexture;
    [Export]
    public Texture freshWaterTexture;
    [Export]
    public Texture frogTexture;
    [Export]
    public Texture glowCharmTexture;
    [Export]
    public Texture grainTexture;
    [Export]
    public Texture gruelTexture;
    [Export]
    public Texture guanoTexture;
    [Export]
    public Texture jellyfishTexture;
    [Export]
    public Texture lodestoneTexture;
    [Export]
    public Texture mushroomTexture;
    [Export]
    public Texture oysterTexture;
    [Export]
    public Texture oysterSoupTexture;
    [Export]
    public Texture peatMossTexture;
    [Export]
    public Texture saltTexture;
    [Export]
    public Texture saltWaterTexture;
    [Export]
    public Texture truffleTexture;
    [Export]
    public Texture woodTexture;

    public Texture GetTexture(ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.AmphibianCharm:
                return amphibianCharmTexture;
            case ItemType.BogWater:
                return bogWaterTexture;
            case ItemType.BurntMuck:
                return burntMuckTexture;
            case ItemType.CaveSlime:
                return caveSlimeTexture;
            case ItemType.CharismaCharm:
                return charismaCharmTexture;
            case ItemType.EyeOfNewt:
                return eyeOfNewtTexture;
            case ItemType.FreshWater:
                return freshWaterTexture;
            case ItemType.Frog:
                return frogTexture;
            case ItemType.GlowCharm:
                return glowCharmTexture;
            case ItemType.Grain:
                return grainTexture;
            case ItemType.Gruel:
                return gruelTexture;
            case ItemType.Guano:
                return guanoTexture;
            case ItemType.Jellyfish:
                return jellyfishTexture;
            case ItemType.Lodestone:
                return lodestoneTexture;
            case ItemType.Oyster:
                return oysterTexture;
            case ItemType.OysterSoup:
                return oysterSoupTexture;
            case ItemType.PeatMoss:
                return peatMossTexture;
            case ItemType.Salt:
                return saltTexture;
            case ItemType.SaltWater:
                return saltWaterTexture;
            case ItemType.Truffle:
                return truffleTexture;
            case ItemType.Wood:
                return woodTexture;
            default:
                return mushroomTexture;
        }
    }
}
