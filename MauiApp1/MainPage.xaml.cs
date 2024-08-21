using Sentry.Maui;
using System.Reflection;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
#pragma warning disable CS0618
        public IReadOnlyList<string> AllCrashTypes { get; } = Enum.GetNames(typeof(CrashType)).AsReadOnly();

        private CrashType _selectedCrashType;
        public CrashType SelectedCrashType
        {
            get => _selectedCrashType;
            set
            {
                _selectedCrashType = value;
                OnPropertyChanged();
            }
        }
#pragma warning restore CS0618

        public string SentrySdkVersion { get; } = GetSentrySdkVersion();

        public MainPage()
        {
            BindingContext = this;
            InitializeComponent();
        }

        private void Crash_Clicked(object sender, EventArgs e)
        {
#pragma warning disable CS0618
            SentrySdk.CauseCrash(SelectedCrashType);
#pragma warning restore CS0618
        }

        private void Capture_Clicked(object sender, EventArgs e)
        {
            try
            {
                throw new ApplicationException("This exception was thrown and capture manually, without crashing the app.");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        private static string GetSentrySdkVersion()
        {
            string version;

            try
            {
                version = typeof(SentryMauiOptions)
                    .Assembly
                    .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!
                    .InformationalVersion;
            }
            catch
            {
                version = "Error";
            }

            return version;
        }
    }

}
