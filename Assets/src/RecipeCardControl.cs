using Godot;
using RecipeGame.Helpers;
using static RecipeGame.Helpers.Enums;

[Tool]
public class RecipeCardControl : MarginContainer
{
    [Export]
    public bool Purchasable;
    [Export]
    public ItemType itemType;
    [Export]
    public NodePath titlePath;
    [Export]
    public NodePath subtitlePath;
    [Export]
    public NodePath contentPath;
    [Export]
    public NodePath backgroundPath;

    [Export(PropertyHint.MultilineText)]
    public string title;
    [Export(PropertyHint.MultilineText)]
    public string subtitle;
    [Export(PropertyHint.MultilineText)]
    public string bodyContent;
    [Export]
    Texture backgroundTexture;

    private TextureRect backgroundTextureRect;

    public override void _Ready()
    {
        SetLabelValues();
    }

    public override void _Process(float delta)
    {
        // if(Engine.EditorHint)
        // {
            // SetLabelValues();
        // }
    }

    private void SetLabelValues()
    {
        this.MustGetNode<Label>(titlePath).Text = title;
        this.MustGetNode<Label>(subtitlePath).Text = subtitle;
        this.MustGetNode<RichTextLabel>(contentPath).BbcodeText = bodyContent;
        this.MustGetNode<TextureRect>(backgroundPath).Texture = backgroundTexture;
    }
}
