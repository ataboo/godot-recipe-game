using System;
using System.Linq;
using Godot;
using RecipeGame.Helpers;
using RecipeGame.Models;
using static RecipeGame.Helpers.Enums;

namespace RecipeGame.Inventory
{
    public class CauldronService 
    {
        public const int MaxHeatLevel = 500;
        public const int MaxTemperature = 500;
        public const int HeatPerWood = 50;
        public const float TempInertia = 5f;
        public const float HeatDecay = 5f;

        private readonly RandomNumberGenerator _rand;

        public int MaxBatchCount = InventoryService.CauldronVolumeCapacity / 1000;

        public CauldronService()
        {
            _rand = new RandomNumberGenerator();
            _rand.Randomize();
        }

        public CauldronState Update(Cauldron cauldron, float delta)
        {
            cauldron.HeatLevel = Mathf.Clamp(cauldron.HeatLevel - HeatDecay * delta, 0, MaxHeatLevel);
            cauldron.Temperature = Mathf.Lerp(cauldron.Temperature, cauldron.HeatLevel, delta / TempInertia);
            cauldron.IngredientVolume = cauldron.Ingredients.Values.Sum(i => i.Volume);

            if(cauldron.Products.Count > 0)
            {
                return CauldronState.WaitingForProductRemoval;
            }

            if(cauldron.CurrentRecipe != null)
            {
                cauldron.CookTimer += delta;

                if(cauldron.Temperature < cauldron.CurrentRecipe.Stats.Temperature.x || cauldron.Temperature > cauldron.CurrentRecipe.Stats.Temperature.y)
                {
                    cauldron.CurrentRecipe = null;
                    return CauldronState.RecipeInterrupted;
                }

                if(cauldron.CookTimer >= cauldron.CurrentRecipe.Stats.CookTime)
                {
                    CreateProducts(cauldron);
                    cauldron.CurrentRecipe = null;
                    cauldron.Ingredients.Clear();

                    return CauldronState.JustFinishedRecipe;
                }

                return CauldronState.RecipeInProgress;
            }

            var newRecipe = FindRecipeToStart(cauldron);
            if(newRecipe != null)
            {
                cauldron.CurrentRecipe = newRecipe;
                cauldron.CookTimer = 0f;
                return CauldronState.JustStartedRecipe;
            }

            return CauldronState.WaitingToCook;
        }

        public bool ClickedOnCauldronProduct(PlayerData playerData, int index)
        {
            if(playerData.Cauldron.Products.Count <= index)
            {
                GD.PushError("Index out of range, clicked on cauldron.");
                return false;
            }

            var product = playerData.Cauldron.Products[index];

            if(playerData.HeldItem != null)
            {
                if(playerData.HeldItem.Stats.ItemType != product.Stats.ItemType)
                {
                    return false;
                }

                var amount = Mathf.Min(playerData.HeldItem.Stats.StackSize - playerData.HeldItem.StackAmount, product.StackAmount);
                if(amount > 0) 
                {
                    product.StackAmount -= amount;
                    playerData.HeldItem.StackAmount += amount;
                    if(product.StackAmount == 0)
                    {
                        playerData.Cauldron.Products.Remove(product);
                    }

                    return true;
                }
                
                return false;
            }

            playerData.Cauldron.Products.Remove(product);
            playerData.HeldItem = product;

            return true;
        }

        public bool ClickedOnCauldron(PlayerData playerData, bool leftClick)
        {
            if(playerData.HeldItem == null || HeldItemIsWood(playerData))
            {
                return false;
            }

            int amount;
            if(playerData.HeldTool == HeldTool.Empty)
            {
                amount = leftClick ? playerData.HeldItem.StackAmount : playerData.HeldItem.StackAmount / 2;
            }
            else
            {
                amount = playerData.HeldTool == HeldTool.Scoop ? 1 : 1000;

                if(leftClick || playerData.HeldItem.StackAmount < amount)
                {
                    return false;
                }
            }

            if(amount == playerData.HeldItem.StackAmount)
            {
                if(PushIngredient(playerData, playerData.HeldItem))
                {
                    playerData.HeldItem = null;
                    return true;
                }
                return false;
            }

            var addition = playerData.HeldItem.Clone();
            addition.StackAmount = amount;
            if(PushIngredient(playerData, addition))
            {
                playerData.HeldItem.StackAmount -= amount;
            }

            return true;
        }

        public bool ClickedOnFire(PlayerData playerData)
        {
            if(!HeldItemIsWood(playerData))
            {
                return false;
            }

            if(playerData.Cauldron.HeatLevel + HeatPerWood > MaxHeatLevel)
            {
                return false;
            }

            playerData.Cauldron.HeatLevel += HeatPerWood;

            if(playerData.HeldItem.StackAmount <= 1)
            {
                playerData.HeldItem = null;
            } 
            else 
            {
                playerData.HeldItem.StackAmount -= 1;
            }

            return true;
        }

        private bool PushIngredient(PlayerData playerData, InventoryItem item)
        {
            if(playerData.Cauldron.Products.Count > 0)
            {
                return false;
            }

            var currentFill = playerData.Cauldron.Ingredients.Values.Sum(i => i.Volume);

            if(currentFill + item.Volume > InventoryService.CauldronVolumeCapacity)
            {
                return false;
            }

            if(playerData.Cauldron.Ingredients.ContainsKey((item.Stats.ItemType, item.Processed)))
            {
                playerData.Cauldron.Ingredients[(item.Stats.ItemType, item.Processed)].StackAmount += item.StackAmount;
            }
            else 
            {
                playerData.Cauldron.Ingredients[(item.Stats.ItemType, item.Processed)] = item;
            }

            playerData.Cauldron.CurrentRecipe = null;

            return true;
        }
        
        private BatchedRecipe FindRecipeToStart(Cauldron cauldron)
        {
            return StatsDefinitions.RecipeStats.Where(r => r.Temperature.x < cauldron.Temperature && r.Temperature.y >= cauldron.Temperature)
                .Select(r => new BatchedRecipe{ 
                    Stats = r,
                    BatchCount = NumberOfViableBatches(r, cauldron)
                }).FirstOrDefault(r => r.BatchCount > 0);
        }

        private int NumberOfViableBatches(RecipeStats recipe, Cauldron cauldron)
        {
            if(!IngredientTypesMatchRecipe(recipe, cauldron))
            {
                return 0;
            }

            var peakViableBatchCount = 0;
            
            for(int batch = 0; batch < MaxBatchCount; batch++)
            {
                var shortIngredients = recipe.Ingredients.Where(i => i.Range.x * batch > cauldron.Ingredients[(i.ItemType, i.Processed)].StackAmount);
                var overIngredients = recipe.Ingredients.Where(i => i.Range.y * batch < cauldron.Ingredients[(i.ItemType, i.Processed)].StackAmount);

                if(shortIngredients.Any())
                {
                    break;
                }

                if(!overIngredients.Any())
                {
                    peakViableBatchCount = batch;
                }
            }

            return peakViableBatchCount;
        }

        private bool IngredientTypesMatchRecipe(RecipeStats recipe, Cauldron cauldron)
        {
            return recipe.Ingredients.Length == cauldron.Ingredients.Count
                && recipe.Ingredients.Select(i => (i.ItemType, i.Processed)).ToHashSet().SetEquals(cauldron.Ingredients.Keys);
        }


        private bool HeldItemIsWood(PlayerData playerData)
        {
            return playerData.HeldItem != null && !playerData.HeldItem.Processed && playerData.HeldItem.Stats.ItemType == ItemType.Wood;
        } 

        public void EmptyCauldron(Cauldron cauldron)
        {
            cauldron.Ingredients.Clear();
            cauldron.CurrentRecipe = null;
            cauldron.CookTimer = 0;
            cauldron.Products.Clear();
        }

        private void CreateProducts(Cauldron cauldron)
        {
            foreach(var productStat in cauldron.CurrentRecipe.Stats.Products)
            {
                for(int i=0; i<cauldron.CurrentRecipe.BatchCount; i++){
                    if(_rand.RandiRange(1, 100) <= productStat.DropProbability)
                    {
                        cauldron.Products.Add(new InventoryItem
                        {
                            Stats = StatsDefinitions.MappedItemStats.Value[productStat.ItemType],
                            StackAmount = _rand.RandiRange(productStat.MinCount, productStat.MaxCount),
                            Processed = false
                        });
                    }
                }
            }

            cauldron.Products = cauldron.Products.AccumulateItemStacks().ToList();
        }
    }
}