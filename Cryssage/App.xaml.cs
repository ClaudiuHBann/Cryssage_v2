namespace Cryssage;

public partial class App : Application
{
    readonly MainPage mp;

    public App(MainPage mp)
    {
        InitializeComponent();

        this.mp = mp;
        MainPage = new AppShell();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        window.MinimumWidth = 1280;
        window.MinimumHeight = 720;

        window.Destroying += (sender, args) => mp.Context.Destructor();

        return window;
    }
}
