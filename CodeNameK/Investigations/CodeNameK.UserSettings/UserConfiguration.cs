using System.Reflection;

namespace UserSettingsDemo;
class UserConfiguration
{
    public static readonly string FilePath = Path.Combine(
    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, // exe folder
        "usersettings.jsonc"    // filename
    );
    public bool IsSyncEnabled { get; set; } = false;
}