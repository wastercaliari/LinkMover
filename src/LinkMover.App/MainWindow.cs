using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;
using LinkMover.Core;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Win32;

namespace LinkMover.App;

public sealed class MainWindow : Window
{
    // =========================================================================
    // DESIGN SYSTEM: SYNTHETIC HYPERDRIVE (Dark Mode First / Telemetry Console)
    // =========================================================================
    private static readonly IBrush BgCanvas = Brush.Parse("#0B1326");
    private static readonly IBrush SurfaceLow = Brush.Parse("#131B2E");
    private static readonly IBrush SurfaceContainer = Brush.Parse("#171F33");
    private static readonly IBrush SurfaceHigh = Brush.Parse("#222A3D");
    private static readonly IBrush SurfaceHighest = Brush.Parse("#2D3449");
    private static readonly IBrush SurfaceTerminal = Brush.Parse("#060E20");

    private static readonly IBrush PrimaryEmerald = Brush.Parse("#4EDEA3");
    private static readonly IBrush PrimaryContainer = Brush.Parse("#10B981");
    private static readonly IBrush OnPrimaryDark = Brush.Parse("#003824");
    private static readonly IBrush SecondaryCyan = Brush.Parse("#4CD7F6");
    private static readonly IBrush TertiaryBlue = Brush.Parse("#ADC6FF");

    private static readonly IBrush ErrorRed = Brush.Parse("#FFB4AB");
    private static readonly IBrush ErrorBg = Brush.Parse("#381318");
    private static readonly IBrush ErrorBorder = Brush.Parse("#EF4444");

    private static readonly IBrush TextHighContrast = Brush.Parse("#DAE2FD");
    private static readonly IBrush TextMuted = Brush.Parse("#94A3B8");
    private static readonly IBrush TextVariant = Brush.Parse("#BBCABF");
    private static readonly IBrush BorderGhost = Brush.Parse("#263248");
    private static readonly IBrush BorderActive = Brush.Parse("#3C4A42");

    private static readonly FontFamily MonoFont = new("Consolas, Cascadia Code, JetBrains Mono, monospace");
    private static readonly FontFamily SansFont = new("Inter, Geist, Segoe UI, sans-serif");

    // =========================================================================
    // UI CONTROLS & STATE
    // =========================================================================
    private readonly TextBox source = new()
    {
        PlaceholderText = "Selecione a pasta de origem para migração...",
        IsReadOnly = true,
        FontFamily = MonoFont,
        FontSize = 13
    };

    private readonly TextBox destination = new()
    {
        PlaceholderText = "Selecione o diretório raiz de destino...",
        IsReadOnly = true,
        FontFamily = MonoFont,
        FontSize = 13
    };

    private readonly TextBlock sourceDriveInfo = new()
    {
        Text = "Origem: Selecione um diretório",
        FontSize = 11,
        Foreground = TextMuted,
        FontFamily = MonoFont
    };

    private readonly TextBlock destinationDriveInfo = new()
    {
        Text = "Destino: Selecione um diretório",
        FontSize = 11,
        Foreground = TextMuted,
        FontFamily = MonoFont
    };

    private readonly TextBlock status = new()
    {
        Text = "Sistema inicializado. Pronto para analisar diretórios.",
        TextWrapping = TextWrapping.Wrap,
        Foreground = TextHighContrast,
        FontSize = 13,
        FontWeight = FontWeight.Medium
    };

    private readonly TextBlock telemetryFilesCount = new()
    {
        Text = "0 ARQUIVOS",
        FontSize = 15,
        FontWeight = FontWeight.Bold,
        Foreground = TextHighContrast,
        FontFamily = MonoFont
    };

    private readonly TextBlock telemetryPhaseText = new()
    {
        Text = "AGUARDANDO",
        FontSize = 15,
        FontWeight = FontWeight.Bold,
        Foreground = SecondaryCyan,
        FontFamily = MonoFont
    };

    private readonly TextBlock telemetrySafetyText = new()
    {
        Text = "SHA-256 ATIVO",
        FontSize = 15,
        FontWeight = FontWeight.Bold,
        Foreground = PrimaryEmerald,
        FontFamily = MonoFont
    };

    private readonly TextBox log = new()
    {
        IsReadOnly = true,
        AcceptsReturn = true,
        TextWrapping = TextWrapping.Wrap,
        MinHeight = 130,
        MaxHeight = 220,
        FontFamily = MonoFont,
        FontSize = 12,
        Foreground = Brush.Parse("#A5F3FC"),
        Background = SurfaceTerminal,
        BorderThickness = new Thickness(0),
        Padding = new Thickness(12)
    };

    private readonly Button run = new()
    {
        Content = "⚡ VALIDAR E EXECUTAR MIGRAÇÃO",
        HorizontalAlignment = HorizontalAlignment.Stretch,
        Height = 48,
        FontSize = 14,
        FontWeight = FontWeight.Bold,
        Background = PrimaryContainer,
        Foreground = OnPrimaryDark,
        CornerRadius = new CornerRadius(8),
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
    };

    private readonly Button cancel = new()
    {
        Content = "CANCELAR (ROLLBACK)",
        MinWidth = 160,
        Height = 48,
        FontSize = 13,
        FontWeight = FontWeight.SemiBold,
        Background = ErrorBg,
        Foreground = ErrorRed,
        BorderBrush = ErrorBorder,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(8),
        IsEnabled = false,
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
    };

    private readonly Button sourceBrowse = new()
    {
        Content = "Procurar...",
        MinWidth = 100,
        Height = 42,
        FontSize = 13,
        FontWeight = FontWeight.Medium,
        Background = SurfaceHigh,
        Foreground = SecondaryCyan,
        BorderBrush = BorderGhost,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(8),
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
    };

    private readonly Button destinationBrowse = new()
    {
        Content = "Procurar...",
        MinWidth = 100,
        Height = 42,
        FontSize = 13,
        FontWeight = FontWeight.Medium,
        Background = SurfaceHigh,
        Foreground = SecondaryCyan,
        BorderBrush = BorderGhost,
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(8),
        Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
    };

    private readonly ProgressBar progress = new()
    {
        IsIndeterminate = false,
        Minimum = 0,
        Maximum = 100,
        Height = 5,
        CornerRadius = new CornerRadius(3),
        Background = SurfaceHigh,
        Foreground = PrimaryEmerald
    };

    private readonly TextBlock indicatorText = new()
    {
        Text = "READY",
        FontSize = 10,
        FontWeight = FontWeight.Bold,
        Foreground = PrimaryEmerald,
        FontFamily = MonoFont
    };

    private readonly Border indicatorBadge = new()
    {
        Background = Brush.Parse("#1A4EDEA3"),
        BorderBrush = Brush.Parse("#404EDEA3"),
        BorderThickness = new Thickness(1),
        CornerRadius = new CornerRadius(6),
        Padding = new Thickness(8, 3)
    };

    // Step pipeline items
    private readonly Border step1Badge = CreateStepCircle("1", true);
    private readonly Border step2Badge = CreateStepCircle("2", false);
    private readonly Border step3Badge = CreateStepCircle("3", false);

    private CancellationTokenSource? currentCts;

    public MainWindow(string[]? args = null)
    {
        Title = "LinkMover // Symlink Studio";
        Width = 960;
        Height = 880;
        MinWidth = 760;
        MinHeight = 700;
        Background = BgCanvas;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        FontFamily = SansFont;
        RequestedThemeVariant = ThemeVariant.Dark;

        indicatorBadge.Child = indicatorText;

        ApplyAccessibilityProperties();
        ApplyStyles();

        var parsed = args ?? Array.Empty<string>();
        for (int i = 0; i + 1 < parsed.Length; i++)
        {
            if (parsed[i] == "--source") source.Text = parsed[++i];
            else if (parsed[i] == "--destination") destination.Text = parsed[++i];
        }

        UpdateDriveInfo();

        var shell = new Grid { RowDefinitions = new RowDefinitions("Auto,*") };
        shell.Children.Add(BuildHeader());

        var content = new StackPanel
        {
            Spacing = 16,
            Margin = new Thickness(28, 20, 28, 24),
            MaxWidth = 880,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        // Step Tracker Pipeline
        content.Children.Add(BuildStepPipeline());

        // Dual Relocator Configuration Cards
        content.Children.Add(BuildSourceCard());
        content.Children.Add(BuildDestinationCard());

        // 3-Metric Telemetry Dashboard
        content.Children.Add(BuildTelemetryDashboard());

        // Action Row: Run & Cancel
        var actionGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 12
        };
        Grid.SetColumn(run, 0); actionGrid.Children.Add(run);
        Grid.SetColumn(cancel, 1); actionGrid.Children.Add(cancel);
        content.Children.Add(actionGrid);

        // Progress Bar
        content.Children.Add(new Border
        {
            Background = SurfaceHigh,
            Height = 5,
            CornerRadius = new CornerRadius(3),
            Child = progress
        });

        // Terminal Telemetry Console Card
        content.Children.Add(BuildTerminalCard());

        // Footer Safety Note
        content.Children.Add(new TextBlock
        {
            Text = "Garantia Transacional: O LinkMover valida hashes SHA-256 pré e pós transferência. Em caso de falha ou cancelamento, a origem é mantida intacta.",
            FontSize = 11,
            Foreground = TextMuted,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextAlignment = TextAlignment.Center,
            MaxWidth = 780
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

        // Wire Events
        source.PropertyChanged += (_, e) => { if (e.Property == TextBox.TextProperty) UpdateDriveInfo(); };
        destination.PropertyChanged += (_, e) => { if (e.Property == TextBox.TextProperty) UpdateDriveInfo(); };

        run.Click += async (_, _) => await RunMoveAsync();
        cancel.Click += (_, _) => CancelMove();
        sourceBrowse.Click += async (_, _) => await ChooseFolderAsync(source, "Selecione a pasta de origem");
        destinationBrowse.Click += async (_, _) => await ChooseFolderAsync(destination, "Selecione o local de destino");
    }

    private void ApplyAccessibilityProperties()
    {
        AutomationProperties.SetName(source, "Caminho da pasta de origem");
        AutomationProperties.SetHelpText(source, "Exibe a pasta selecionada que será movida.");

        AutomationProperties.SetName(sourceBrowse, "Procurar pasta de origem");
        AutomationProperties.SetHelpText(sourceBrowse, "Abre o seletor para escolher o diretório a ser movido.");

        AutomationProperties.SetName(destination, "Caminho do diretório de destino");
        AutomationProperties.SetHelpText(destination, "Exibe a pasta pai onde os dados físicos serão hospedados.");

        AutomationProperties.SetName(destinationBrowse, "Procurar diretório de destino");
        AutomationProperties.SetHelpText(destinationBrowse, "Abre o seletor para escolher a unidade de destino.");

        AutomationProperties.SetName(run, "Validar e executar migração");
        AutomationProperties.SetHelpText(run, "Inicia o ciclo seguro com inventário SHA-256, cópia de dados e criação de link simbólico.");

        AutomationProperties.SetName(cancel, "Cancelar e restaurar origem");
        AutomationProperties.SetHelpText(cancel, "Interrompe a operação em andamento e restaura o diretório de origem.");

        AutomationProperties.SetName(log, "Console de telemetria e diagnósticos de execução");
        AutomationProperties.SetHelpText(log, "Exibe os logs em tempo real do pipeline transacional.");
    }

    private void ApplyStyles()
    {
        Styles.Add(new Style(x => x.OfType<TextBox>())
        {
            Setters =
            {
                new Setter(TextBox.BackgroundProperty, SurfaceContainer),
                new Setter(TextBox.ForegroundProperty, TextHighContrast),
                new Setter(TextBox.BorderBrushProperty, BorderGhost),
                new Setter(TextBox.BorderThicknessProperty, new Thickness(1)),
                new Setter(TextBox.CornerRadiusProperty, new CornerRadius(8)),
                new Setter(TextBox.PaddingProperty, new Thickness(12, 10)),
                new Setter(TextBox.VerticalContentAlignmentProperty, VerticalAlignment.Center)
            }
        });
    }

    private Control BuildHeader()
    {
        var brandIcon = new Border
        {
            Width = 38,
            Height = 38,
            CornerRadius = new CornerRadius(10),
            Background = SurfaceHigh,
            BorderBrush = Brush.Parse("#334155"),
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = "⚡",
                FontSize = 20,
                Foreground = PrimaryEmerald,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var titleBlock = new StackPanel
        {
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 8,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children =
                    {
                        new TextBlock
                        {
                            Text = "LINKMOVER // SYMLINK STUDIO",
                            FontSize = 12,
                            FontWeight = FontWeight.Bold,
                            Foreground = PrimaryEmerald,
                            FontFamily = MonoFont,
                            LetterSpacing = 1.2
                        },
                        indicatorBadge
                    }
                },
                new TextBlock
                {
                    Text = "Universal Directory Relocator & NTFS Symlink Engine",
                    FontSize = 14,
                    FontWeight = FontWeight.SemiBold,
                    Foreground = TextHighContrast
                }
            }
        };

        var brand = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 12,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { brandIcon, titleBlock }
        };

        // Environment & Privilege Chips
        bool isElevated = OperatingSystem.IsWindows() && IsAdministrator();
        bool isDevMode = OperatingSystem.IsWindows() && IsDeveloperModeEnabled();

        var privilegeChip = new Border
        {
            Background = isElevated ? Brush.Parse("#1A10B981") : (isDevMode ? Brush.Parse("#1A38BDF8") : Brush.Parse("#1AF59E0B")),
            BorderBrush = isElevated ? PrimaryContainer : (isDevMode ? SecondaryCyan : Brush.Parse("#F59E0B")),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(10, 5),
            Child = new TextBlock
            {
                Text = isElevated ? "🛡️ ELEVADO (ADMIN)" : (isDevMode ? "⚙️ DEV MODE ATIVO" : "⚠️ PADRÃO (REQUER UAC)"),
                FontSize = 10,
                FontWeight = FontWeight.Bold,
                Foreground = isElevated ? PrimaryEmerald : (isDevMode ? SecondaryCyan : Brush.Parse("#F59E0B")),
                FontFamily = MonoFont
            }
        };

        var osChip = new Border
        {
            Background = SurfaceHigh,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(10, 5),
            Child = new TextBlock
            {
                Text = "● NTFS · APFS · EXT4",
                FontSize = 10,
                FontWeight = FontWeight.SemiBold,
                Foreground = SecondaryCyan,
                FontFamily = MonoFont
            }
        };

        var chips = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center,
            Children = { privilegeChip, osChip }
        };

        var row = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetColumn(brand, 0); row.Children.Add(brand);
        Grid.SetColumn(chips, 1); row.Children.Add(chips);

        var header = new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(28, 14),
            Child = row
        };
        Grid.SetRow(header, 0);
        return header;
    }

    private Control BuildStepPipeline()
    {
        var step1 = CreateStepItem(step1Badge, "ORIGEM", "Pasta Atual", PrimaryEmerald);
        var step2 = CreateStepItem(step2Badge, "DESTINO", "Armazenamento", SecondaryCyan);
        var step3 = CreateStepItem(step3Badge, "LINK VIRTUAL", "Transparência NTFS", TertiaryBlue);

        var line1 = new Border { Background = BorderGhost, Height = 1, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0) };
        var line2 = new Border { Background = BorderGhost, Height = 1, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0) };

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto,*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };

        Grid.SetColumn(step1, 0); grid.Children.Add(step1);
        Grid.SetColumn(line1, 1); grid.Children.Add(line1);
        Grid.SetColumn(step2, 2); grid.Children.Add(step2);
        Grid.SetColumn(line2, 3); grid.Children.Add(line2);
        Grid.SetColumn(step3, 4); grid.Children.Add(step3);

        return new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(12),
            Padding = new Thickness(18, 12),
            Child = grid
        };
    }

    private static StackPanel CreateStepItem(Border badge, string title, string sub, IBrush accent)
    {
        return new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 10,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                badge,
                new StackPanel
                {
                    Spacing = 1,
                    VerticalAlignment = VerticalAlignment.Center,
                    Children =
                    {
                        new TextBlock { Text = title, FontSize = 11, FontWeight = FontWeight.Bold, Foreground = accent, FontFamily = MonoFont },
                        new TextBlock { Text = sub, FontSize = 10, Foreground = TextMuted }
                    }
                }
            }
        };
    }

    private static Border CreateStepCircle(string number, bool active)
    {
        return new Border
        {
            Width = 28,
            Height = 28,
            CornerRadius = new CornerRadius(14),
            Background = active ? Brush.Parse("#1A4EDEA3") : SurfaceHigh,
            BorderBrush = active ? PrimaryEmerald : BorderGhost,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = number,
                FontSize = 11,
                FontWeight = FontWeight.Bold,
                Foreground = active ? PrimaryEmerald : TextMuted,
                FontFamily = MonoFont,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };
    }

    private Control BuildSourceCard()
    {
        var badge = new Border
        {
            Width = 32,
            Height = 32,
            CornerRadius = new CornerRadius(8),
            Background = SurfaceHigh,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = "01",
                FontSize = 12,
                FontWeight = FontWeight.Bold,
                Foreground = PrimaryEmerald,
                FontFamily = MonoFont,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var labels = new StackPanel
        {
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock { Text = "Diretório de Origem (Source)", FontSize = 14, FontWeight = FontWeight.SemiBold, Foreground = TextHighContrast },
                new TextBlock { Text = "A pasta cujos arquivos serão transferidos fisicamente. Um link simbólico ocupará este local exato.", FontSize = 12, Foreground = TextMuted }
            }
        };

        var top = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };
        var left = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { badge, labels } };
        Grid.SetColumn(left, 0); top.Children.Add(left);
        Grid.SetColumn(sourceDriveInfo, 2); top.Children.Add(sourceDriveInfo);

        var pathGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8
        };
        source.MinHeight = 42;
        Grid.SetColumn(source, 0); pathGrid.Children.Add(source);
        Grid.SetColumn(sourceBrowse, 1); pathGrid.Children.Add(sourceBrowse);

        return new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(18, 16),
            Child = new StackPanel { Spacing = 12, Children = { top, pathGrid } }
        };
    }

    private Control BuildDestinationCard()
    {
        var badge = new Border
        {
            Width = 32,
            Height = 32,
            CornerRadius = new CornerRadius(8),
            Background = SurfaceHigh,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            Child = new TextBlock
            {
                Text = "02",
                FontSize = 12,
                FontWeight = FontWeight.Bold,
                Foreground = SecondaryCyan,
                FontFamily = MonoFont,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        var labels = new StackPanel
        {
            Spacing = 2,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock { Text = "Novo Local Físico de Destino (Target)", FontSize = 14, FontWeight = FontWeight.SemiBold, Foreground = TextHighContrast },
                new TextBlock { Text = "O diretório no disco secundário onde os dados reais residirão após a verificação de hash.", FontSize = 12, Foreground = TextMuted }
            }
        };

        var top = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("Auto,*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };
        var left = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10, Children = { badge, labels } };
        Grid.SetColumn(left, 0); top.Children.Add(left);
        Grid.SetColumn(destinationDriveInfo, 2); top.Children.Add(destinationDriveInfo);

        var pathGrid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            ColumnSpacing = 8
        };
        destination.MinHeight = 42;
        Grid.SetColumn(destination, 0); pathGrid.Children.Add(destination);
        Grid.SetColumn(destinationBrowse, 1); pathGrid.Children.Add(destinationBrowse);

        return new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(18, 16),
            Child = new StackPanel { Spacing = 12, Children = { top, pathGrid } }
        };
    }

    private Control BuildTelemetryDashboard()
    {
        var card1 = BuildTelemetryTile("INTEGRIDADE SHA-256", "🛡️ BIT-A-BIT", telemetrySafetyText, "Pré & pós cópia garantida");
        var card2 = BuildTelemetryTile("VOLUME TRANSFERIDO", "⚡ I/O STREAM", telemetryFilesCount, "Validação de inventário");
        var card3 = BuildTelemetryTile("ETAPA DO PIPELINE", "🔄 STATUS", telemetryPhaseText, "Monitoramento em tempo real");

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,*,*"),
            ColumnSpacing = 12
        };
        Grid.SetColumn(card1, 0); grid.Children.Add(card1);
        Grid.SetColumn(card2, 1); grid.Children.Add(card2);
        Grid.SetColumn(card3, 2); grid.Children.Add(card3);

        return grid;
    }

    private static Control BuildTelemetryTile(string title, string tag, TextBlock valueControl, string sub)
    {
        var top = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };
        var label = new TextBlock
        {
            Text = title,
            FontSize = 9,
            FontWeight = FontWeight.Bold,
            Foreground = TextMuted,
            FontFamily = MonoFont,
            LetterSpacing = 1.0
        };
        var badge = new TextBlock
        {
            Text = tag,
            FontSize = 9,
            FontWeight = FontWeight.SemiBold,
            Foreground = SecondaryCyan,
            FontFamily = MonoFont
        };
        Grid.SetColumn(label, 0); top.Children.Add(label);
        Grid.SetColumn(badge, 1); top.Children.Add(badge);

        var subLabel = new TextBlock { Text = sub, FontSize = 10, Foreground = TextMuted, TextWrapping = TextWrapping.NoWrap };

        return new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(10),
            Padding = new Thickness(14, 10),
            Child = new StackPanel { Spacing = 6, Children = { top, valueControl, subLabel } }
        };
    }

    private Control BuildTerminalCard()
    {
        var top = new Grid
        {
            ColumnDefinitions = new ColumnDefinitions("*,Auto"),
            VerticalAlignment = VerticalAlignment.Center
        };

        var titleBlock = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new Border
                {
                    Width = 8,
                    Height = 8,
                    CornerRadius = new CornerRadius(4),
                    Background = PrimaryEmerald
                },
                new TextBlock
                {
                    Text = "TERMINAL TELEMETRY STREAM // KERNEL I/O",
                    FontSize = 10,
                    FontWeight = FontWeight.Bold,
                    Foreground = PrimaryEmerald,
                    FontFamily = MonoFont,
                    LetterSpacing = 1.2
                }
            }
        };

        Grid.SetColumn(titleBlock, 0); top.Children.Add(titleBlock);
        Grid.SetColumn(status, 1); top.Children.Add(status);

        var terminalBox = new Border
        {
            Background = SurfaceTerminal,
            BorderBrush = Brush.Parse("#1E293B"),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(8),
            Child = log
        };

        return new Border
        {
            Background = SurfaceLow,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(14),
            Padding = new Thickness(18, 14),
            Child = new StackPanel { Spacing = 10, Children = { top, terminalBox } }
        };
    }

    private void UpdateDriveInfo()
    {
        sourceDriveInfo.Text = FormatDriveTelemetry(source.Text);
        destinationDriveInfo.Text = FormatDriveTelemetry(destination.Text);
    }

    private static string FormatDriveTelemetry(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return "Unidade: Aguardando seleção";
        try
        {
            var root = Path.GetPathRoot(Path.GetFullPath(path));
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root)) return "Unidade: Não inicializada";
            var drive = new DriveInfo(root);
            var freeGb = drive.AvailableFreeSpace / (1024.0 * 1024 * 1024);
            var totalGb = drive.TotalSize / (1024.0 * 1024 * 1024);
            return $"{drive.Name} [{drive.DriveFormat}] · {freeGb:F1} GB livres de {totalGb:F1} GB";
        }
        catch
        {
            return "Unidade: Informações indisponíveis";
        }
    }

    private void SetStatusBadge(string label, IBrush bg, IBrush text)
    {
        indicatorText.Text = label;
        indicatorText.Foreground = text;
        indicatorBadge.Background = bg;
        indicatorBadge.BorderBrush = text;
    }

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static bool IsAdministrator() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    private static bool IsDeveloperModeEnabled()
    {
        using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock");
        return Convert.ToInt32(key?.GetValue("AllowDevelopmentWithoutDevLicense", 0) ?? 0) == 1;
    }

    private async Task ChooseFolderAsync(TextBox target, string dialogTitle)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = dialogTitle, AllowMultiple = false });
        if (folders.Count > 0 && folders[0].TryGetLocalPath() is { } path)
        {
            target.Text = path;
            AppendLog($"[CONFIG] Diretório configurado: {path}");
        }
    }

    private void CancelMove()
    {
        if (currentCts is { IsCancellationRequested: false })
        {
            cancel.IsEnabled = false;
            status.Text = "Cancelamento solicitado. Interrompendo com segurança...";
            telemetryPhaseText.Text = "ROLLBACK";
            telemetryPhaseText.Foreground = ErrorRed;
            AppendLog("[CANCELAMENTO] Operação interrompida pelo operador. Disparando rollback atômico...");
            currentCts.Cancel();
        }
    }

    private void AppendLog(string message)
    {
        var time = DateTime.Now.ToString("HH:mm:ss.fff");
        log.Text += $"[{time}] {message}{Environment.NewLine}";
        log.CaretIndex = log.Text.Length;
    }

    private async Task RunMoveAsync()
    {
        var src = source.Text?.Trim() ?? "";
        var parent = destination.Text?.Trim() ?? "";
        if (src.Length == 0 || parent.Length == 0)
        {
            status.Text = "Selecione a pasta de origem e o diretório de destino antes de iniciar.";
            AppendLog("[ERRO] Origem ou destino não preenchidos.");
            return;
        }

        var dest = Path.Combine(parent, Path.GetFileName(Path.TrimEndingDirectorySeparator(src)));
        if (!await ConfirmAsync($"A operação copiará e validará todos os arquivos via SHA-256 e criará o link simbólico.\n\nOrigem: {src}\nDestino Físico: {dest}\n\nDeseja iniciar a migração?"))
        {
            AppendLog("[ABORT] Migração cancelada pelo usuário no prompt de confirmação.");
            return;
        }

        if (OperatingSystem.IsWindows() && !IsAdministrator() && !IsDeveloperModeEnabled())
        {
            if (!await ConfirmAsync("O Windows exige permissão de Administrador para criar links simbólicos (a menos que o Modo Desenvolvedor esteja ativo).\n\nO aplicativo solicitará elevação via UAC e será reiniciado. Nenhuma alteração foi feita ainda.\n\nContinuar com elevação UAC?"))
                return;

            try
            {
                var exe = Environment.ProcessPath ?? throw new InvalidOperationException("Não foi possível localizar o executável para reiniciar.");
                var psi = new ProcessStartInfo(exe) { UseShellExecute = true, Verb = "runas" };
                psi.ArgumentList.Add("--source"); psi.ArgumentList.Add(src); psi.ArgumentList.Add("--destination"); psi.ArgumentList.Add(parent);
                Process.Start(psi);
                Close();
                return;
            }
            catch (Exception ex)
            {
                await MessageAsync("Não foi possível reiniciar com permissão administrativa.\n\nAtive o Developer Mode no Windows ou execute o LinkMover como Administrador.\n\n" + ex.Message);
                return;
            }
        }

        currentCts = new CancellationTokenSource();
        run.IsEnabled = false;
        cancel.IsEnabled = true;
        sourceBrowse.IsEnabled = false;
        destinationBrowse.IsEnabled = false;
        progress.IsIndeterminate = true;
        status.Text = "Executando migração...";
        log.Text = "";
        SetStatusBadge("MIGRATING", Brush.Parse("#1A03B5D3"), SecondaryCyan);
        telemetryPhaseText.Text = "INICIADO";
        telemetryPhaseText.Foreground = SecondaryCyan;

        AppendLog($"[START] Iniciando pipeline de migração transacional.");
        AppendLog($"[ORIGEM]  {src}");
        AppendLog($"[DESTINO] {dest}");

        var reporter = new Progress<string>(s =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                status.Text = s;
                AppendLog(s);

                if (s.Contains("Copiando", StringComparison.OrdinalIgnoreCase))
                {
                    telemetryPhaseText.Text = "COPIANDO";
                    telemetryPhaseText.Foreground = PrimaryEmerald;
                }
                else if (s.Contains("SHA-256", StringComparison.OrdinalIgnoreCase))
                {
                    telemetryPhaseText.Text = "VALIDANDO HASH";
                    telemetryPhaseText.Foreground = SecondaryCyan;
                }
                else if (s.Contains("substituição", StringComparison.OrdinalIgnoreCase) || s.Contains("link", StringComparison.OrdinalIgnoreCase))
                {
                    telemetryPhaseText.Text = "CRIANDO LINK";
                    telemetryPhaseText.Foreground = TertiaryBlue;
                }
                else if (s.Contains("Arquivos copiados:", StringComparison.OrdinalIgnoreCase))
                {
                    telemetryFilesCount.Text = s.Replace("Arquivos copiados:", "").Trim() + " ARQS";
                }
            });
        });

        try
        {
            var result = await SafeMover.MoveToLinkedFolderAsync(src, parent, reporter, currentCts.Token);
            status.Text = result.Message;
            SetStatusBadge("VERIFIED", Brush.Parse("#1A10B981"), PrimaryEmerald);
            telemetryPhaseText.Text = "CONCLUÍDO";
            telemetryPhaseText.Foreground = PrimaryEmerald;
            telemetryFilesCount.Text = result.Files.ToString("N0", CultureInfo.InvariantCulture) + " ARQS";

            AppendLog("--------------------------------------------------");
            AppendLog($"[SUCESSO] {result.Message}");
            AppendLog($"[TOTAL ARQUIVOS] {result.Files:N0}");
            AppendLog($"[TOTAL BYTES]    {result.Bytes:N0} bytes ({result.Bytes / (1024.0 * 1024.0):F2} MB)");
            AppendLog($"[DESTINO FINAL]  {result.Destination}");
            AppendLog($"[BACKUP LIMPO]   {(result.BackupRemoved ? "Sim (Origem removida com segurança)" : "Não (Backup preservado)")}");
            AppendLog("--------------------------------------------------");

            await MessageAsync(result.Message + $"\n\nArquivos validados: {result.Files:N0}\nBytes: {result.Bytes:N0}\nDestino físico: {result.Destination}");
        }
        catch (OperationCanceledException)
        {
            status.Text = "Operação cancelada pelo usuário. Origem restaurada com integridade.";
            SetStatusBadge("CANCELED", Brush.Parse("#1AEF4444"), ErrorRed);
            telemetryPhaseText.Text = "REVERTIDO";
            telemetryPhaseText.Foreground = ErrorRed;
            AppendLog("[ROLLBACK CONCLUÍDO] Todos os arquivos originais foram preservados intactos.");
            await MessageAsync("A operação foi cancelada. Seus arquivos originais foram preservados intactos.");
        }
        catch (Exception ex)
        {
            status.Text = "A operação falhou. Rollback disparado.";
            SetStatusBadge("FAILED", Brush.Parse("#1AEF4444"), ErrorRed);
            telemetryPhaseText.Text = "FALHA";
            telemetryPhaseText.Foreground = ErrorRed;
            AppendLog($"[FATAL] Exceção capturada: {ex.Message}");
            await MessageAsync("Não foi possível concluir a migração.\n\n" + ex.Message);
        }
        finally
        {
            progress.IsIndeterminate = false;
            progress.Value = 100;
            run.IsEnabled = true;
            cancel.IsEnabled = false;
            sourceBrowse.IsEnabled = true;
            destinationBrowse.IsEnabled = true;
            currentCts?.Dispose();
            currentCts = null;
            UpdateDriveInfo();
        }
    }

    private async Task<bool> ConfirmAsync(string message)
    {
        var dlg = new Window
        {
            Title = "Confirmação de Migração // LinkMover",
            Width = 520,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = BgCanvas,
            RequestedThemeVariant = ThemeVariant.Dark
        };
        bool answer = false;
        var yes = new Button
        {
            Content = "Confirmar e Continuar",
            MinWidth = 160,
            Height = 40,
            Background = PrimaryContainer,
            Foreground = OnPrimaryDark,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(6)
        };
        var no = new Button
        {
            Content = "Cancelar",
            MinWidth = 100,
            Height = 40,
            Background = SurfaceHigh,
            Foreground = TextHighContrast,
            BorderBrush = BorderGhost,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(6)
        };
        yes.Click += (_, _) => { answer = true; dlg.Close(); };
        no.Click += (_, _) => dlg.Close();

        dlg.Content = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 18,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Foreground = TextHighContrast,
                    MaxWidth = 470
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Spacing = 10,
                    Children = { no, yes }
                }
            }
        };
        await dlg.ShowDialog(this);
        return answer;
    }

    private async Task MessageAsync(string message)
    {
        var dlg = new Window
        {
            Title = "Status // LinkMover",
            Width = 480,
            SizeToContent = SizeToContent.Height,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Background = BgCanvas,
            RequestedThemeVariant = ThemeVariant.Dark
        };
        var close = new Button
        {
            Content = "Fechar",
            HorizontalAlignment = HorizontalAlignment.Right,
            MinWidth = 110,
            Height = 40,
            Background = PrimaryContainer,
            Foreground = OnPrimaryDark,
            FontWeight = FontWeight.Bold,
            CornerRadius = new CornerRadius(6)
        };
        close.Click += (_, _) => dlg.Close();
        dlg.Content = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 16,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    Foreground = TextHighContrast
                },
                close
            }
        };
        await dlg.ShowDialog(this);
    }
}
