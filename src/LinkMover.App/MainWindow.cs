using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using LinkMover.Core;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Win32;

namespace LinkMover.App;

public sealed class MainWindow : Window
{
    private static readonly IBrush Canvas = new LinearGradientBrush
    {
        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
        GradientStops = new GradientStops
        {
            new GradientStop(Color.Parse("#EAF1FF"), 0),
            new GradientStop(Color.Parse("#F5F7FC"), 0.48),
            new GradientStop(Color.Parse("#EDE7FF"), 1)
        }
    };
    private static readonly IBrush Ink = Brush.Parse("#172033");
    private static readonly IBrush Muted = Brush.Parse("#65718A");
    private static readonly IBrush Accent = Brush.Parse("#4F5FEA");
    private static readonly IBrush BorderColor = Brush.Parse("#BFFFFFFF");
    private static readonly IBrush Glass = Brush.Parse("#CFFFFFFF");
    private static readonly IBrush GlassStrong = Brush.Parse("#E8FFFFFF");
    private readonly TextBox source = new() { PlaceholderText = "Selecione a pasta que deseja mover", IsReadOnly = true, Background = Brush.Parse("#E8FFFFFF") };
    private readonly TextBox destination = new() { PlaceholderText = "Selecione onde os arquivos ficarão", IsReadOnly = true, Background = Brush.Parse("#E8FFFFFF") };
    private readonly TextBlock status = new() { Text = "Tudo pronto. Escolha a origem e o destino.", TextWrapping = TextWrapping.Wrap, Foreground = Muted, FontSize = 13 };
    private readonly TextBox log = new() { IsReadOnly = true, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap, MinHeight = 96, MaxHeight = 150 };
    private readonly Button run = new() { Content = "Validar e mover", HorizontalAlignment = HorizontalAlignment.Stretch, Height = 48, FontSize = 14, FontWeight = FontWeight.SemiBold };
    private readonly Button sourceBrowse = new() { Content = "Procurar", MinWidth = 104, Height = 40 };
    private readonly Button destinationBrowse = new() { Content = "Procurar", MinWidth = 104, Height = 40 };
    private readonly ProgressBar progress = new() { IsIndeterminate = false, Minimum = 0, Maximum = 100, Height = 4 };

    public MainWindow(string[]? args = null)
    {
        Title = "LinkMover";
        Width = 900;
        Height = 800;
        MinWidth = 680;
        MinHeight = 650;
        Background = Canvas;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        FontFamily = new FontFamily("Inter, Segoe UI, sans-serif");

        ApplyStyles();
        var parsed = args ?? Array.Empty<string>();
        for (int i = 0; i + 1 < parsed.Length; i++)
        {
            if (parsed[i] == "--source") source.Text = parsed[++i];
            else if (parsed[i] == "--destination") destination.Text = parsed[++i];
        }

        var shell = new Grid { RowDefinitions = new RowDefinitions("Auto,*") };
        shell.Children.Add(BuildHeader());

        var content = new StackPanel { Spacing = 22, Margin = new Thickness(32, 28, 32, 24), MaxWidth = 820, HorizontalAlignment = HorizontalAlignment.Center };
        content.Children.Add(new StackPanel
        {
            Spacing = 7,
            Children =
            {
                Eyebrow("GERENCIAMENTO DE ARMAZENAMENTO"),
                new TextBlock { Text = "Mova sem quebrar caminhos", FontSize = 27, FontWeight = FontWeight.Bold, Foreground = Ink, TextWrapping = TextWrapping.Wrap },
                new TextBlock { Text = "Transfira pastas para outro disco e mantenha o caminho original funcionando.", FontSize = 14, Foreground = Muted, TextWrapping = TextWrapping.Wrap, MaxWidth = 650 }
            }
        });

        content.Children.Add(BuildPathCard("01", "Pasta de origem", "A pasta atual do aplicativo ou dos arquivos.", source, sourceBrowse));
        content.Children.Add(BuildPathCard("02", "Novo local", "O LinkMover criará a pasta de destino dentro deste local.", destination, destinationBrowse));
        content.Children.Add(BuildSafetyCard());
        content.Children.Add(run);
        content.Children.Add(new Border { Background = Accent, Height = 2, CornerRadius = new CornerRadius(1), Child = progress });
        content.Children.Add(BuildStatusCard());
        content.Children.Add(new TextBlock
        {
            Text = "Antes de começar: feche o aplicativo que usa essa pasta e mantenha um backup dos dados importantes.",
            FontSize = 11, Foreground = Muted, TextWrapping = TextWrapping.Wrap, HorizontalAlignment = HorizontalAlignment.Center, TextAlignment = TextAlignment.Center, MaxWidth = 700
        });

        var scroll = new ScrollViewer
        {
            Content = content,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
        };
        Grid.SetRow(scroll, 1);
        shell.Children.Add(scroll);
        Content = shell;

        run.Click += async (_, _) => await RunMoveAsync();
        sourceBrowse.Click += async (_, _) => await ChooseFolderAsync(source, "Selecione a pasta de origem");
        destinationBrowse.Click += async (_, _) => await ChooseFolderAsync(destination, "Selecione o local de destino");
    }

    private void ApplyStyles()
    {
        Styles.Add(new Style(x => x.OfType<Button>())
        {
            Setters =
            {
                new Setter(Button.BackgroundProperty, Brush.Parse("#D9FFFFFF")),
                new Setter(Button.ForegroundProperty, Brush.Parse("#3444B8")),
                new Setter(Button.BorderBrushProperty, Brush.Parse("#BFFFFFFF")),
                new Setter(Button.BorderThicknessProperty, new Thickness(1)),
                new Setter(Button.PaddingProperty, new Thickness(16, 10)),
                new Setter(Button.FontSizeProperty, 13.0),
                new Setter(Button.FontWeightProperty, FontWeight.SemiBold)
            }
        });
        Styles.Add(new Style(x => x.OfType<TextBox>())
        {
            Setters =
            {
                new Setter(TextBox.BackgroundProperty, Brush.Parse("#BFFFFFFF")),
                new Setter(TextBox.ForegroundProperty, Ink),
                new Setter(TextBox.BorderBrushProperty, BorderColor),
                new Setter(TextBox.BorderThicknessProperty, new Thickness(1)),
                new Setter(TextBox.PaddingProperty, new Thickness(12, 9)),
                new Setter(TextBox.FontSizeProperty, 12.0),
                new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center)
            }
        });
        Styles.Add(new Style(x => x.OfType<ProgressBar>())
        {
            Setters =
            {
                new Setter(ProgressBar.BackgroundProperty, Brush.Parse("#66FFFFFF")),
                new Setter(ProgressBar.ForegroundProperty, Accent)
            }
        });
        run.Background = Accent;
        run.Foreground = Brushes.White;
        run.BorderBrush = Accent;
        run.BorderThickness = new Thickness(0);
        run.FontSize = 14;
        sourceBrowse.Background = GlassStrong;
        destinationBrowse.Background = GlassStrong;
    }

    private Control BuildHeader()
    {
        var brandIcon = new Border
        {
            Width = 42, Height = 42, CornerRadius = new CornerRadius(13),
            Background = new LinearGradientBrush { StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative), EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative), GradientStops = new GradientStops { new GradientStop(Color.Parse("#7184FF"), 0), new GradientStop(Color.Parse("#4F46E5"), 1) } },
            Child = new TextBlock { Text = "L", FontSize = 24, FontWeight = FontWeight.Bold, Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center }
        };
        var brand = new StackPanel
        {
            Orientation = Orientation.Horizontal, Spacing = 12, VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                brandIcon,
                new StackPanel { Spacing = 1, VerticalAlignment = VerticalAlignment.Center, Children =
                    {
                        new TextBlock { Text = "LinkMover", FontSize = 17, FontWeight = FontWeight.Bold, Foreground = Ink },
                        new TextBlock { Text = "SMART STORAGE", FontSize = 10, FontWeight = FontWeight.SemiBold, Foreground = Muted, LetterSpacing = 1.4 }
                    }
                }
            }
        };
        var platform = new Border
        {
            Background = Brush.Parse("#AFFFFFFF"), BorderBrush = Brush.Parse("#BFFFFFFF"), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(14), Padding = new Thickness(12, 7),
            Child = new TextBlock { Text = "●  Windows · Linux · macOS", FontSize = 11, FontWeight = FontWeight.SemiBold, Foreground = Brush.Parse("#067647") }
        };
        var row = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), VerticalAlignment = VerticalAlignment.Center };
        Grid.SetColumn(brand, 0); row.Children.Add(brand);
        Grid.SetColumn(platform, 1); row.Children.Add(platform);
        var header = new Border { Background = Brush.Parse("#B8FFFFFF"), BorderBrush = BorderColor, BorderThickness = new Thickness(0, 0, 0, 1), Padding = new Thickness(30, 17), Child = row };
        Grid.SetRow(header, 0);
        return header;
    }

    private Control BuildPathCard(string number, string title, string description, TextBox path, Button browse)
    {
        var numberBadge = new Border
        {
            Width = 34, Height = 34, CornerRadius = new CornerRadius(12), Background = Brush.Parse("#BFFFFFFF"),
            Child = new TextBlock { Text = number, FontSize = 12, FontWeight = FontWeight.Bold, Foreground = Accent, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center }
        };
        var labels = new StackPanel { Spacing = 4, VerticalAlignment = VerticalAlignment.Center, Children =
        {
            new TextBlock { Text = title, FontSize = 15, FontWeight = FontWeight.SemiBold, Foreground = Ink },
            new TextBlock { Text = description, FontSize = 12, Foreground = Muted, TextWrapping = TextWrapping.Wrap }
        }};
        var top = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 12, Children = { numberBadge, labels } };
        var pathGrid = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), ColumnSpacing = 10 };
        path.MinHeight = 42;
        Grid.SetColumn(path, 0); pathGrid.Children.Add(path);
        Grid.SetColumn(browse, 1); pathGrid.Children.Add(browse);
        return new Border
        {
            Background = Glass, BorderBrush = BorderColor, BorderThickness = new Thickness(1),
            BoxShadow = BoxShadows.Parse("0 10 28 0 #180D1B45"), CornerRadius = new CornerRadius(22), Padding = new Thickness(22),
            Child = new StackPanel { Spacing = 16, Children = { top, pathGrid } }
        };
    }

    private Control BuildSafetyCard()
    {
        var shield = new Border
        {
            Width = 38, Height = 38, CornerRadius = new CornerRadius(12), Background = Brush.Parse("#ECFDF3"),
            Child = new TextBlock { Text = "✓", FontSize = 20, FontWeight = FontWeight.Bold, Foreground = Brush.Parse("#027A48"), HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center }
        };
        var details = new StackPanel { Spacing = 5, Children =
        {
            new TextBlock { Text = "Validação antes da substituição", FontSize = 13, FontWeight = FontWeight.SemiBold, Foreground = Ink },
            new TextBlock { Text = "O LinkMover compara os arquivos por SHA-256 e só troca o caminho depois de validar a cópia. Se houver falha, tenta restaurar a origem.", FontSize = 12, Foreground = Muted, TextWrapping = TextWrapping.Wrap }
        }};
        var row = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 13, Children = { shield, details } };
        return new Border { Background = Brush.Parse("#A6E9F8EF"), BorderBrush = Brush.Parse("#BFFFFFFF"), BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(20), Padding = new Thickness(19), Child = row };
    }

    private Control BuildStatusCard()
    {
        var heading = new Grid { ColumnDefinitions = new ColumnDefinitions("*,Auto"), VerticalAlignment = VerticalAlignment.Center };
        var title = new TextBlock { Text = "STATUS DA OPERAÇÃO", FontSize = 10, FontWeight = FontWeight.Bold, Foreground = Muted, LetterSpacing = 1.2 };
        var indicator = new Border { Background = Brush.Parse("#F2F4F7"), CornerRadius = new CornerRadius(10), Padding = new Thickness(9, 4), Child = new TextBlock { Text = "AGUARDANDO", FontSize = 9, FontWeight = FontWeight.Bold, Foreground = Muted } };
        Grid.SetColumn(title, 0); heading.Children.Add(title); Grid.SetColumn(indicator, 1); heading.Children.Add(indicator);
        return new Border
        {
            Background = Glass, BorderBrush = BorderColor, BorderThickness = new Thickness(1), BoxShadow = BoxShadows.Parse("0 10 28 0 #180D1B45"), CornerRadius = new CornerRadius(20), Padding = new Thickness(20),
            Child = new StackPanel { Spacing = 12, Children = { heading, status, new Border { Height = 1, Background = BorderColor }, new TextBlock { Text = "DETALHES", FontSize = 10, FontWeight = FontWeight.Bold, Foreground = Muted, LetterSpacing = 1.2 }, log } }
        };
    }

    private static TextBlock Eyebrow(string text) => new() { Text = text, FontSize = 10, FontWeight = FontWeight.Bold, Foreground = Accent, LetterSpacing = 1.5 };

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static bool IsAdministrator() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static bool IsDeveloperModeEnabled()
    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock");
        return Convert.ToInt32(key?.GetValue("AllowDevelopmentWithoutDevLicense", 0) ?? 0) == 1;
    }

    private async Task ChooseFolderAsync(TextBox target, string title)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = title, AllowMultiple = false });
        if (folders.Count > 0 && folders[0].TryGetLocalPath() is { } path) target.Text = path;
    }

    private async Task RunMoveAsync()
    {
        var src = source.Text?.Trim() ?? "";
        var parent = destination.Text?.Trim() ?? "";
        if (src.Length == 0 || parent.Length == 0) { status.Text = "Selecione a pasta de origem e o novo local antes de continuar."; return; }
        var dest = Path.Combine(parent, Path.GetFileName(Path.TrimEndingDirectorySeparator(src)));
        if (!await ConfirmAsync("A operação copiará e validará os arquivos e então substituirá o caminho original.\n\nOrigem: " + src + "\nDestino: " + dest + "\n\nContinuar?")) return;
        if (OperatingSystem.IsWindows() && !IsAdministrator() && !IsDeveloperModeEnabled())
        {
            if (!await ConfirmAsync("O Windows pode exigir permissão administrativa para criar o link simbólico. O aplicativo pedirá confirmação antes de reiniciar via UAC. Nenhuma pasta será alterada antes disso. Continuar?")) return;
            try
            {
                var exe = Environment.ProcessPath ?? throw new InvalidOperationException("Não foi possível localizar o executável para reiniciar.");
                var psi = new ProcessStartInfo(exe) { UseShellExecute = true, Verb = "runas" };
                psi.ArgumentList.Add("--source"); psi.ArgumentList.Add(src); psi.ArgumentList.Add("--destination"); psi.ArgumentList.Add(parent);
                Process.Start(psi); Close(); return;
            }
            catch (Exception ex) { await MessageAsync("Não foi possível reiniciar com permissão administrativa. Ative o Developer Mode ou execute como administrador.\n\n" + ex.Message); return; }
        }

        run.IsEnabled = false; sourceBrowse.IsEnabled = false; destinationBrowse.IsEnabled = false; progress.IsIndeterminate = true; status.Text = "Operação em andamento…"; log.Text = "";
        var reporter = new Progress<string>(s => { status.Text = s; log.Text += "• " + s + Environment.NewLine; });
        try
        {
            var result = await SafeMover.MoveToLinkedFolderAsync(src, parent, reporter);
            status.Text = result.Message;
            log.Text += Environment.NewLine + "Arquivos: " + result.Files.ToString("N0", CultureInfo.InvariantCulture) + Environment.NewLine + "Bytes: " + result.Bytes.ToString("N0", CultureInfo.InvariantCulture) + Environment.NewLine + "Destino: " + result.Destination + Environment.NewLine + "Backup removido: " + (result.BackupRemoved ? "sim" : "não");
            await MessageAsync(result.Message);
        }
        catch (Exception ex) { status.Text = "A operação falhou. Consulte os detalhes abaixo."; log.Text += Environment.NewLine + "ERRO: " + ex; await MessageAsync("Não foi possível concluir.\n\n" + ex.Message); }
        finally { progress.IsIndeterminate = false; progress.Value = 100; run.IsEnabled = true; sourceBrowse.IsEnabled = true; destinationBrowse.IsEnabled = true; }
    }

    private async Task<bool> ConfirmAsync(string message)
    {
        var dlg = new Window { Title = "Confirmar operação", Width = 500, SizeToContent = SizeToContent.Height, WindowStartupLocation = WindowStartupLocation.CenterOwner, CanResize = false, Background = Canvas };
        bool answer = false;
        var yes = new Button { Content = "Continuar", MinWidth = 110, Height = 40, Background = Accent, Foreground = Brushes.White, BorderThickness = new Thickness(0) };
        var no = new Button { Content = "Cancelar", MinWidth = 100, Height = 40 };
        yes.Click += (_, _) => { answer = true; dlg.Close(); }; no.Click += (_, _) => dlg.Close();
        dlg.Content = new StackPanel { Margin = new Thickness(24), Spacing = 18, Children = { new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, FontSize = 13, Foreground = Ink, MaxWidth = 450 }, new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right, Spacing = 10, Children = { no, yes } } } };
        await dlg.ShowDialog(this);
        return answer;
    }

    private async Task MessageAsync(string message)
    {
        var dlg = new Window { Title = "LinkMover", Width = 440, SizeToContent = SizeToContent.Height, WindowStartupLocation = WindowStartupLocation.CenterOwner, CanResize = false, Background = Canvas };
        var close = new Button { Content = "Fechar", HorizontalAlignment = HorizontalAlignment.Right, MinWidth = 100, Height = 40 };
        close.Click += (_, _) => dlg.Close();
        dlg.Content = new StackPanel { Margin = new Thickness(24), Spacing = 16, Children = { new TextBlock { Text = message, TextWrapping = TextWrapping.Wrap, FontSize = 13, Foreground = Ink }, close } };
        await dlg.ShowDialog(this);
    }
}
