using Godot;
using RecipeGame.Helpers;
using System;

public class MainMenuControl : Node
{
    [Signal]
    public delegate void OnStartNewGame();

    [Export]
    public NodePath newGameButtonPath;
    [Export]
    public NodePath instructionsButtonPath;
    [Export]
    public NodePath instructionsClosePath;
    [Export]
    public NodePath instructionsPanelPath;
    [Export]
    public NodePath hamburgerButtonPath;
    [Export]
    public NodePath confirmNewGamePath;
    [Export]
    public NodePath confirmYesPath;
    [Export]
    public NodePath confirmNoPath;
    [Export]
    public NodePath speedClockPath;

    private Control mainPanel;
    private Control instructionsPanel;

    public bool ConfirmNewGames = false;

    private Button newGameButton;
    private Button confirmYesButton;
    private Button confirmNoButton;
    private Control confirmNewGamePanel;
    private TextureButton _hamburgerButton;
    public TextureButton HamburgerButton => _hamburgerButton ?? (_hamburgerButton = GetNode<TextureButton>(hamburgerButtonPath));
    private SpeedClockControl _speedClock;
    public SpeedClockControl SpeedClock => _speedClock ?? (_speedClock = GetNode<SpeedClockControl>(speedClockPath));

    public bool Visible 
    {
        get 
        {
            return mainPanel.Visible;
        }
        set 
        {
            confirmNewGamePanel.Visible = false;
            mainPanel.Visible = value;
        }
    }

    public override void _Ready()
    {
        newGameButton = this.MustGetNode<Button>(newGameButtonPath);
        confirmYesButton = this.MustGetNode<Button>(confirmYesPath);
        confirmNoButton = this.MustGetNode<Button>(confirmNoPath);
        confirmNewGamePanel = this.MustGetNode<Control>(confirmNewGamePath);

        mainPanel = GetNode<Control>("Panel") ?? throw new NullReferenceException();
        
        var instructionsButton = GetNode<Button>(instructionsButtonPath) ?? throw new NullReferenceException();
        instructionsButton.Connect("pressed", this, nameof(HandleInstructionsPressed));

        var instructionCloseButton = GetNode<Button>(instructionsClosePath) ?? throw new NullReferenceException();
        instructionCloseButton.Connect("pressed", this, nameof(HandleInstructionsClosePressed));

        instructionsPanel = GetNode<Control>(instructionsPanelPath) ?? throw new NullReferenceException();

        newGameButton.Connect("pressed", this, nameof(HandlePressedNewGame));
        confirmNoButton.Connect("pressed", this, nameof(HandlePressedNewGameNo));
        confirmYesButton.Connect("pressed", this, nameof(HandlePressedNewGameYes));
    }

    void HandleInstructionsPressed()
    {
        instructionsPanel.Visible = true;
    }

    void HandleInstructionsClosePressed()
    {
        instructionsPanel.Visible = false;
    }

    void HandlePressedNewGame()
    {
        if(ConfirmNewGames)
        {
            confirmNewGamePanel.Visible = true;
        }
        else
        {
            EmitSignal(nameof(OnStartNewGame));
        }
    }

    void HandlePressedNewGameNo()
    {
        confirmNewGamePanel.Visible = false;
    }

    void HandlePressedNewGameYes()
    {
        EmitSignal(nameof(OnStartNewGame));
    }

}
