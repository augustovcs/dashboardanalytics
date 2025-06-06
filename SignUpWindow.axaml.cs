using System;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System.Text.Json;
using Avalonia.Dialogs;

namespace AvaloniaApplication1;

public partial class SignUpWindow : Window
{
    private const string ConfigFileName = "FirstTimeCredentials.json";

    public SignUpWindow()
    {
        InitializeComponent();
    }

    public void OnSubmitClick(object? sender, RoutedEventArgs e)
    {
        try
        {   
            var username = UsernameTextBox.Text ?? string.Empty;
            var password = PasswordTextBox.Text ?? string.Empty;
            ClearMessages();
            
            if (!ValidatePassword(password, out var cleanPassword))
                return;

            if (!ValidateUsername(username, out var cleanUsername))
                return;

            SaveCredentials(cleanPassword, cleanUsername);
            _showDashboard();

        }
        catch (Exception ex)
        {
            ErrorTextBlock.Text = $"Error: {ex.Message}";
        }
    }

    private void ClearMessages()
    {
        ErrorTextBlock.Text = string.Empty;
        ResultTextBlock.Text = string.Empty;

    }

    

    private bool ValidateUsername(string username, out string cleanUsername)
    {
        cleanUsername = new string(username.Where(c => char.IsLetterOrDigit(c)).ToArray());

        if (string.IsNullOrEmpty(username))
        {
            ResultTextBlock.Text = "Username cannot be empty";
            return false;
        }

        if (cleanUsername.Length < 10)
        {
            ErrorTextBlock.Text = "Username must be at least 10 characters";
            return false;
        }

        return true;

    }
    private bool ValidatePassword(string password, out string cleanPassword)
    {

        cleanPassword = new string(password.Where(c => char.IsDigit(c)).ToArray());

        if (string.IsNullOrEmpty(password))
        {
            ResultTextBlock.Text = "Password cannot be empty";
            return false;
        }

        if (cleanPassword.Length < 4)
        {
            ErrorTextBlock.Text = "Password must be at least 4 characters";
            return false;
        }

        return true;
    }

    public static void SaveCredentials(string password, string username)
    {
        try
        {
            var configPath = GetConfigPath();
            Directory.CreateDirectory(configPath);  

            var credentials = new Credentials { Password = password, Username = username};
            var jsonString = JsonSerializer.Serialize(credentials);

            var tempFilePath = Path.Combine(configPath, "temp_" + ConfigFileName);
            File.WriteAllText(tempFilePath, jsonString);

            var finalFilePath = GetCredentialsFilePath();
            File.Move(tempFilePath, finalFilePath, overwrite: true);
        }
        catch (Exception ex)
        {
            throw new Exception("Error saving credentials", ex);
        }
    }

    public static Credentials GetCredentials()
    {
        var filePath = GetCredentialsFilePath();
    
        if (!File.Exists(filePath))
            return new Credentials();

        try
        {
            var json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<Credentials>(json) ?? new Credentials();
        }
        catch
        {
            return new Credentials();  
        }
    }
    
    public static string GetConfigPath()
    {
        // Retorna o caminho do DIRETÓRIO (não do arquivo)
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dashboardanalysis"  // 
        );
    }

    public static string GetCredentialsFilePath()
    {
        // Retorna o caminho COMPLETO do arquivo JSON
        return Path.Combine(GetConfigPath(), ConfigFileName);
    }

    private void _showDashboard()
    {
        if (Application.Current?.ApplicationLifetime is not
            IClassicDesktopStyleApplicationLifetime desktop)
            return;

        Dispatcher.UIThread.Post(() =>
        {
            desktop.MainWindow = new LogInWindow();
            desktop.MainWindow.Show();
            Close();
        });
    }

    
}


    

    
    
    

    
    
