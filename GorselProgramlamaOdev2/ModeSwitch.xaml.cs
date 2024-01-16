using Microsoft.Maui.ApplicationModel;

namespace GorselProgramlamaOdev2;

public partial class ModeSwitch : ContentPage
{
	public ModeSwitch()
	{
		InitializeComponent();
    }
    private void OnThemeSwitchToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            
            Application.Current.UserAppTheme = AppTheme.Light;
        }
        else
        {
            
            Application.Current.UserAppTheme = AppTheme.Dark;
        }
    }
}