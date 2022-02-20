using RecipeGame.Models;

public class CauldronNoisesControl : AudioLibraryPlayer
{
    override public void _Ready()
    {
        base._Ready();
    }

    public void UpdateCauldron(Cauldron cauldron)
    {
        var shouldPlayFire1 = cauldron.HeatLevel > 30 && cauldron.HeatLevel < 260;
        var shouldPlayFire2 = cauldron.HeatLevel > 240;

        var shouldPlayBubble1 = cauldron.HasAnyLiquids() && cauldron.Temperature > 50 && cauldron.Temperature < 200;
        var shouldPlayBubble2 = cauldron.HasAnyLiquids() && cauldron.Temperature > 190 && cauldron.Temperature < 350;
        var shouldPlayBubble3 = cauldron.HasAnyLiquids() &&cauldron.Temperature > 340 && cauldron.Temperature < 500;

        SyncPlaying("fire1", shouldPlayFire1);
        SyncPlaying("fire2", shouldPlayFire2);
        SyncPlaying("bubble1", shouldPlayBubble1);
        SyncPlaying("bubble2", shouldPlayBubble2);
        SyncPlaying("bubble3", shouldPlayBubble3);
    }

    private void SyncPlaying(string soundName, bool shouldPlay)
    {
        var sound = sources[soundName];
        if(shouldPlay && !sound.Playing)
        {
            sound.Play();
        }
        else if(!shouldPlay && sound.Playing)
        {
            sound.Stop();
        }
    }
}
