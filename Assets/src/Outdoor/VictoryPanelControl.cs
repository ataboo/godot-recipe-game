using Godot;

public class VictoryPanelControl : TextureRect
{
    [Export]
    public NodePath closeButtonPath;

    private Button _closeButton;
    public Button CloseButton => _closeButton ?? (_closeButton = GetNode<Button>(closeButtonPath));

    public void ShowPanel()
    {
        Visible = true;
    }
}
