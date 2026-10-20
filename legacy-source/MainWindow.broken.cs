using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using LinkMover.Core;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Win32;
namespace LinkMover.App;
public sealed class MainWindow : Window
{
 private readonly TextBox source=new(){PlaceholderText="Selecione a pasta instalada",IsReadOnly=true};
 private readonly TextBox destination=new(){PlaceholderText="Selecione a pasta pai de destino",IsReadOnly=true};
 private readonly TextBlock status=new(){Text="Pronto para verificar os caminhos.",TextWrapping=TextWrapping.Wrap};
 private readonly TextBox log=new(){IsReadOnly=true,AcceptsReturn=true,TextWrapping=TextWrapping.Wrap,MinHeight=110};
 private readonly Button run=new(){Content="Validar e mover com segurança",HorizontalAlignment=HorizontalAlignment.Stretch,Height=48};
 private readonly Button sourceBrowse=new(){Content="Escolher pasta…",MinWidth=130};
 private readonly Button destinationBrowse=new(){Content="Escolher destino…",MinWidth=130};
 private readonly ProgressBar progress=new(){IsIndeterminate=false,Minimum=0,Maximum=100,Height=6};
 public MainWindow(string[]? args=null)
 {
  Title="LinkMover · Gerenciador de armazenamento"; Width=820; Height=760; MinWidth=620; MinHeight=650; Background=Brush.Parse("#F4F6FB"); WindowStartupLocation=WindowStartupLocation.CenterScreen;
  var parsed=args??Array.Empty<string>(); for(int i=0;i+1<parsed.Length;i++) { if(parsed[i]=="--source") source.Text=parsed[++i]; else if(parsed[i]=="--destination") destination.Text=parsed[++i]; }
  var root=new Grid{RowDefinitions=new RowDefinitions("Auto,*")};
  var header=new Border{Background=Brush.Parse("#233876"),Padding=new Thickness(30,25),Child=new StackPanel{Spacing=8,Children={
   new TextBlock{Text="LinkMover",FontSize=28,FontWeight=FontWeight.Bold,Foreground=Brushes.White},
   new TextBlock{Text="Mova pastas. Preserve o caminho original.",FontSize=14,Foreground=Brush.Parse("#DDE5FF")},
   new TextBlock{Text="Windows  ·  Linux  ·  macOS",FontSize=12,Foreground=Brush.Parse("#B9C8FF")}
  }}};
  Grid.SetRow(header,0); root.Children.Add(header);
  var body=new StackPanel{Spacing=16,Margin=new Thickness(24)};
  body.Children.Add(Card("01 · Pasta atual","A pasta que contém os arquivos reais.",source,sourceBrowse));
  body.Children.Add(Card("02 · Local de armazenamento","O LinkMover criará uma pasta com o mesmo nome neste local.",destination,destinationBrowse));
  body.Children.Add(new Border{Background=Brushes.White,BorderBrush=Brush.Parse("#DCE3F0"),BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(14),Padding=new Thickness(18),Child=new StackPanel{Spacing=8,Children={
   new TextBlock{Text="Proteção de dados",FontSize=15,FontWeight=FontWeight.SemiBold,Foreground=Brush.Parse("#172554")},
   new TextBlock{Text="A cópia é validada por SHA-256 antes de substituir o caminho original. Se a criação do link falhar, o aplicativo tenta restaurar a pasta. Links e junções dentro da origem são bloqueados.",TextWrapping=TextWrapping.Wrap,FontSize=13,Foreground=Brush.Parse("#475569")}
  }}});
  run.Click+=async(_,_)=>await RunMoveAsync();
  sourceBrowse.Click+=async(_,_)=>await ChooseFolderAsync(source,"Selecione a pasta de origem");
  destinationBrowse.Click+=async(_,_)=>await ChooseFolderAsync(destination,"Selecione a pasta pai de destino");
  body.Children.Add(run); body.Children.Add(progress);
  body.Children.Add(new TextBlock{Text="STATUS DA OPERAÇÃO",FontSize=11,FontWeight=FontWeight.Bold,Foreground=Brush.Parse("#64748B")});
  body.Children.Add(status);
  body.Children.Add(new TextBlock{Text="REGISTRO",FontSize=11,FontWeight=FontWeight.Bold,Foreground=Brush.Parse("#64748B")});
  body.Children.Add(log);
  body.Children.Add(new TextBlock{Text="Feche os aplicativos que usam a pasta antes de movê-la. Faça backup dos dados importantes. A criação de links pode exigir permissão do sistema.",FontSize=11,Foreground=Brush.Parse("#64748B"),TextWrapping=TextWrapping.Wrap});
  var scroll=new ScrollViewer{Content=body,VerticalScrollBarVisibility=Avalonia.Controls.Primitives.ScrollBarVisibility.Auto};
  Grid.SetRow(scroll,1); root.Children.Add(scroll); Content=root;
 }
 private static Border Card(string title,string subtitle,TextBox field,Button button)
 {
  var grid=new Grid{ColumnDefinitions=new ColumnDefinitions("*,Auto"),RowDefinitions=new RowDefinitions("Auto,Auto,Auto"),ColumnSpacing=12,RowSpacing=9};
  var heading=new TextBlock{Text=title,FontSize=15,FontWeight=FontWeight.SemiBold,Foreground=Brush.Parse("#172554")};
  var sub=new TextBlock{Text=subtitle,FontSize=12,Foreground=Brush.Parse("#64748B"),TextWrapping=TextWrapping.Wrap};
  Grid.SetRow(heading,0); Grid.SetColumnSpan(heading,2); grid.Children.Add(heading);
  Grid.SetRow(sub,1); Grid.SetColumnSpan(sub,2); grid.Children.Add(sub);
  field.MinHeight=38; Grid.SetRow(field,2); Grid.SetColumn(field,0); grid.Children.Add(field);
  Grid.SetRow(button,2); Grid.SetColumn(button,1); grid.Children.Add(button);
  return new Border{Background=Brushes.White,BorderBrush=Brush.Parse("#DCE3F0"),BorderThickness=new Thickness(1),CornerRadius=new CornerRadius(14),Padding=new Thickness(18),Child=grid};
 }
 private static bool IsAdministrator() => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
 private static bool IsDeveloperModeEnabled() { using var key=Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock"); return Convert.ToInt32(key?.GetValue("AllowDevelopmentWithoutDevLicense",0)??0)==1; }
 private async Task ChooseFolderAsync(TextBox target,string title)
 {
  var folders=await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions{Title=title,AllowMultiple=false});
  if(folders.Count>0&&folders[0].TryGetLocalPath() is { } path) target.Text=path;
 }
 private async Task RunMoveAsync()
 {
  var src=source.Text?.Trim()??""; var parent=destination.Text?.Trim()??"";
  if(src.Length==0||parent.Length==0){status.Text="Selecione a pasta atual e o destino.";return;}
  var dest=Path.Combine(parent,Path.GetFileName(Path.TrimEndingDirectorySeparator(src)));
  if(!await ConfirmAsync("A operação copiará e validará os arquivos e então substituirá o caminho original.\n\nOrigem: "+src+"\nDestino: "+dest+"\n\nContinuar?"))return;
  if(OperatingSystem.IsWindows() && !IsAdministrator() && !IsDeveloperModeEnabled()) {
   if(!await ConfirmAsync("O Windows está bloqueando links simbólicos para este usuário. O LinkMover pode reiniciar com permissão administrativa; o Windows exibirá o pedido de UAC. Nenhuma pasta será alterada antes disso. Continuar?")) return;
   try { var exe=Environment.ProcessPath ?? throw new InvalidOperationException("Não foi possível localizar o executável para reiniciar."); var psi=new ProcessStartInfo(exe){UseShellExecute=true,Verb="runas"}; psi.ArgumentList.Add("--source"); psi.ArgumentList.Add(src); psi.ArgumentList.Add("--destination"); psi.ArgumentList.Add(parent); Process.Start(psi); Close(); return; }
   catch(Exception ex) { await MessageAsync("Não foi possível reiniciar com permissão administrativa. Ative o Developer Mode do Windows ou execute o aplicativo como administrador.\n\n"+ex.Message); return; }
  }
  run.IsEnabled=false; sourceBrowse.IsEnabled=false; destinationBrowse.IsEnabled=false; progress.IsIndeterminate=true; status.Text="Operação em andamento…"; log.Text="";
  var reporter=new Progress<string>(s=>{status.Text=s;log.Text+=("• "+s+Environment.NewLine);});
  try{
   var result=await SafeMover.MoveToLinkedFolderAsync(src,parent,reporter);
   status.Text=result.Message; log.Text+=Environment.NewLine+"Arquivos: "+result.Files.ToString("N0",CultureInfo.InvariantCulture)+Environment.NewLine+"Bytes: "+result.Bytes.ToString("N0",CultureInfo.InvariantCulture)+Environment.NewLine+"Destino: "+result.Destination+Environment.NewLine+"Backup removido: "+(result.BackupRemoved?"sim":"não");
   await MessageAsync(result.Message);
  }catch(Exception ex){status.Text="A operação falhou. Consulte o registro.";log.Text+=Environment.NewLine+"ERRO: "+ex;await MessageAsync("Não foi possível concluir.\n\n"+ex.Message);}
  finally{progress.IsIndeterminate=false;progress.Value=100;run.IsEnabled=true;sourceBrowse.IsEnabled=true;destinationBrowse.IsEnabled=true;}
 }
 private async Task<bool> ConfirmAsync(string message)
 {
  var dlg=new Window{Title="Confirmar operação",Width=500,SizeToContent=SizeToContent.Height,WindowStartupLocation=WindowStartupLocation.CenterOwner,CanResize=false}; bool answer=false;
  var yes=new Button{Content="Continuar",MinWidth=110}; var no=new Button{Content="Cancelar",MinWidth=110};
  yes.Click+=(_,_)=>{answer=true;dlg.Close();}; no.Click+=(_,_)=>dlg.Close();
  dlg.Content=new StackPanel{Margin=new Thickness(22),Spacing=16,Children={new TextBlock{Text=message,TextWrapping=TextWrapping.Wrap},new StackPanel{Orientation=Orientation.Horizontal,HorizontalAlignment=HorizontalAlignment.Right,Spacing=10,Children={no,yes}}}};
  await dlg.ShowDialog(this); return answer;
 }
 private async Task MessageAsync(string message)
 {
  var dlg=new Window{Title="LinkMover",Width=440,SizeToContent=SizeToContent.Height,WindowStartupLocation=WindowStartupLocation.CenterOwner,CanResize=false};
  var close=new Button{Content="Fechar",HorizontalAlignment=HorizontalAlignment.Right,MinWidth=100}; close.Click+=(_,_)=>dlg.Close();
  dlg.Content=new StackPanel{Margin=new Thickness(22),Spacing=16,Children={new TextBlock{Text=message,TextWrapping=TextWrapping.Wrap},close}};
  await dlg.ShowDialog(this);
 }
}