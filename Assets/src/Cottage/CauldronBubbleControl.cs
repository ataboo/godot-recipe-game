using System.Linq;
using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;

public class CauldronBubbleControl : CPUParticles2D
{
    [Export]
    public float maxBubbleRate = 50;

    public override void _Ready()
    {
        EmissionShape = EmissionShapeEnum.Sphere;
        EmissionSphereRadius = 196;   
    }

    public void UpdateBubbles(Cauldron cauldron)
    {
        var shouldEmit = false;
        if(cauldron.HasAnyLiquids())
        {
            var t = Mathf.Clamp(((float)cauldron.Temperature - 25) / (CauldronService.MaxTemperature - 25), 0, 1f);
            var particleAmount = Mathf.Lerp(0, maxBubbleRate, t);
            // More coarse adjustments since the emitter restarts when amount changed.
            particleAmount = Mathf.Ceil(particleAmount / 5) * 5;
            shouldEmit = particleAmount > 0;

            if(shouldEmit && particleAmount != Amount)
            {
                Amount = (int)particleAmount;
            }
        }

        if(Emitting != shouldEmit)
        {
            Emitting = shouldEmit;
        }
    }
}
