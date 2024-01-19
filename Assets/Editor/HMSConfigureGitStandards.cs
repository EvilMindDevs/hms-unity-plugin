using UnityEngine;

/// <summary>
/// This class is used to configure Git when the Unity project starts.
/// It uses the System.Diagnostics.Process class to run a Git command in a new process.
/// </summary>
public class HMSConfigureGitStandards
{
    // Define Git commands as constants
    #region Git Commands
    private const string GitCommand = "git";
    private const string GitConfigCommand = "config --local include.path ../.gitconfig";
    private const string GitUnsetConfigCommand = "config --local --unset include.path";
    private const string GitGetConfigCommand = "config --local --get include.path";
    private const string GitVersionCommand = "--version";
    #endregion

    public static void Start()
    {
        if (IsGitInstalled())
        {
            if (!IsGitConfigured())
            {
                ConfigureGit();
            }
        }
        else
        {
            Debug.LogWarning("Git is not installed");

        }
    }
    private static void ConfigureGit()
    {
        Debug.Log("Configuring Git");
        try
        {
            RunProcess(GitCommand, GitConfigCommand);
        }
        catch (System.Exception e)
        {
            Debug.Log("Error: " + e.Message);
            RunProcess(GitCommand, GitUnsetConfigCommand);
        }
        finally
        {
            Debug.Log("Git Configured");
        }
    }
    private static int RunProcess(string fileName, string arguments)
    {
        var proc = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };

        proc.Start();
        proc.WaitForExit();

        return proc.ExitCode;
    }
    private static bool IsGitInstalled()
    {
        return RunProcess(GitCommand, GitVersionCommand) == 0;
    }
    private static bool IsGitConfigured()
    {
        return RunProcess(GitCommand, GitGetConfigCommand) == 0;
    }
}
