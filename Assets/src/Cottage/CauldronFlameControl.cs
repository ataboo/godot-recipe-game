using Godot;
using RecipeGame.Inventory;
using RecipeGame.Models;

public class CauldronFlameControl : CPUParticles2D
{
    [Export]
    float flameRadius = 350;
    [Export]
    int pointCount = 128;
    [Export]
    float maxFlameRate = 30;

    public override void _Ready()
    {
        GenerateEmissionPointsAndVectors();
    }

    private void GenerateEmissionPointsAndVectors()
    {
        var points = new Vector2[pointCount];
        var normals = new Vector2[pointCount];
        EmissionShape = EmissionShapeEnum.DirectedPoints;

        for(int i=0; i<pointCount; i++)
        {
            var angle = 2 * Mathf.Pi * ((float)i / pointCount);
            points[i] = new Vector2(0, flameRadius).Rotated(angle);
            normals[i] = new Vector2(1, 0).Rotated(angle);
        }

        EmissionPoints = points;
        EmissionNormals = normals;
    }

    public void UpdateFlameRate(Cauldron cauldron)
    {
        var t = (float)cauldron.HeatLevel / CauldronService.MaxHeatLevel;
        float particleAmount = Mathf.Lerp(0, maxFlameRate, t);
        // More coarse adjustments since the emitter restarts when amount changed.
        particleAmount = Mathf.CeilToInt(particleAmount / 3) * 3;
        var shouldEmit = particleAmount > 0;
        if(Emitting != shouldEmit)
        {
            Emitting = shouldEmit;
        }

        if(shouldEmit && particleAmount != Amount)
        {
            Amount = (int)particleAmount;
        }
    }
}
