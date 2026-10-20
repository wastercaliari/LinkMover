# LinkMover - interface moderna para mover uma pasta e criar link simbolico
Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing
[System.Windows.Forms.Application]::EnableVisualStyles()
[System.Windows.Forms.Application]::SetCompatibleTextRenderingDefault($false)

$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    $answer = [System.Windows.Forms.MessageBox]::Show("O LinkMover precisa de permissao administrativa para criar o link simbolico. Deseja reiniciar como administrador?","LinkMover",[System.Windows.Forms.MessageBoxButtons]::YesNo,[System.Windows.Forms.MessageBoxIcon]::Question)
    if ($answer -ne [System.Windows.Forms.DialogResult]::Yes) { exit }
    Start-Process powershell.exe -Verb RunAs -ArgumentList @("-NoProfile","-ExecutionPolicy","Bypass","-File",$PSCommandPath)
    exit
}

# Paleta visual
$navy = [Drawing.Color]::FromArgb(17,24,39)
$panel = [Drawing.Color]::FromArgb(31,41,55)
$panelLight = [Drawing.Color]::FromArgb(245,247,250)
$blue = [Drawing.Color]::FromArgb(63,81,181)
$blueHover = [Drawing.Color]::FromArgb(48,63,159)
$white = [Drawing.Color]::White
$ink = [Drawing.Color]::FromArgb(31,41,55)
$muted = [Drawing.Color]::FromArgb(107,114,128)
$green = [Drawing.Color]::FromArgb(0,137,123)

$form = New-Object Windows.Forms.Form
$form.Text = "LinkMover | Material Edition"
$form.ClientSize = New-Object Drawing.Size(760,570)
$form.MinimumSize = New-Object Drawing.Size(776,609)
$form.StartPosition = "CenterScreen"
$form.FormBorderStyle = "Sizable"
$form.MaximizeBox = $true
$form.MinimizeBox = $true
$form.BackColor = $panelLight
$form.Font = New-Object Drawing.Font("Segoe UI",9)
$form.AutoScaleMode = [Windows.Forms.AutoScaleMode]::Dpi

function New-Label([string]$text,[int]$x,[int]$y,[int]$w,[int]$h,[int]$size=9,[Drawing.Color]$color=$ink,[bool]$bold=$false) {
    $l = New-Object Windows.Forms.Label
    $l.Text=$text; $l.Location=New-Object Drawing.Point($x,$y); $l.Size=New-Object Drawing.Size($w,$h)
    $style = [Drawing.FontStyle]::Regular
    if($bold){$style=[Drawing.FontStyle]::Bold}
    $l.Font=New-Object Drawing.Font("Segoe UI",$size,$style)
    $l.ForeColor=$color; $l.BackColor=[Drawing.Color]::Transparent
    return $l
}
function New-Button([string]$text,[int]$x,[int]$y,[int]$w,[int]$h,[bool]$primary=$false) {
    $b=New-Object Windows.Forms.Button
    $b.Text=$text; $b.Location=New-Object Drawing.Point($x,$y); $b.Size=New-Object Drawing.Size($w,$h)
    $b.FlatStyle=[Windows.Forms.FlatStyle]::Flat
    $b.FlatAppearance.BorderSize=0
    $b.FlatAppearance.MouseOverBackColor=if($primary){$blueHover}else{[Drawing.Color]::FromArgb(229,234,242)}
    $b.BackColor=if($primary){$blue}else{[Drawing.Color]::FromArgb(229,234,242)}
    $b.ForeColor=if($primary){$white}else{$ink}
    $b.Font=New-Object Drawing.Font("Segoe UI",9,[Drawing.FontStyle]::Bold)
    $b.Cursor=[Windows.Forms.Cursors]::Hand
    return $b
}
function New-Card([int]$x,[int]$y,[int]$w,[int]$h) {
    $p=New-Object Windows.Forms.Panel
    $p.Location=New-Object Drawing.Point($x,$y); $p.Size=New-Object Drawing.Size($w,$h)
    $p.BackColor=$white
    $p.BorderStyle=[Windows.Forms.BorderStyle]::None
    return $p
}

# Inventario iterativo: nao segue junctions/links internos nem cria arrays gigantes.
function Copy-SafeDirectory([string]$source,[string]$destination) {
    $stack = New-Object 'System.Collections.Generic.Stack[object]'
    $stack.Push([pscustomobject]@{ Source=$source; Destination=$destination })
    while ($stack.Count -gt 0) {
        $pair = $stack.Pop()
        foreach ($entry in (Get-ChildItem -LiteralPath $pair.Source -Force -ErrorAction Stop)) {
            if (($entry.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw ("A origem contem link/junction interno que precisa ser tratado manualmente: " + $entry.FullName)
            }
            $to = Join-Path $pair.Destination $entry.Name
            if ($entry.PSIsContainer) {
                New-Item -ItemType Directory -Path $to -ErrorAction Stop | Out-Null
                $stack.Push([pscustomobject]@{ Source=$entry.FullName; Destination=$to })
            } else {
                Copy-Item -LiteralPath $entry.FullName -Destination $to -Force -ErrorAction Stop
            }
        }
    }
}

function Get-SafeFileInventory([string]$root) {
    $stack = New-Object 'System.Collections.Generic.Stack[string]'
    $stack.Push($root)
    [long]$count = 0
    [long]$bytes = 0
    while ($stack.Count -gt 0) {
        $dir = $stack.Pop()
        foreach ($entry in (Get-ChildItem -LiteralPath $dir -Force -ErrorAction Stop)) {
            if (($entry.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) {
                throw ("Foi encontrado link/junction durante a verificacao: " + $entry.FullName)
            }
            if ($entry.PSIsContainer) { $stack.Push($entry.FullName); continue }
            $count++
            $bytes += [long]$entry.Length
        }
    }
    return [pscustomobject]@{ Count = $count; Bytes = $bytes }
}

# Cabecalho
$header=New-Object Windows.Forms.Panel
$header.Location=New-Object Drawing.Point(0,0); $header.Size=New-Object Drawing.Size(760,125)
$header.BackColor=$blue; $header.Anchor="Top,Left,Right"; $form.Controls.Add($header)
$mark=New-Object Windows.Forms.Panel
$mark.Location=New-Object Drawing.Point(28,27); $mark.Size=New-Object Drawing.Size(46,46); $mark.BackColor=$blue
$header.Controls.Add($mark)
$mark.Controls.Add((New-Label "↗" 9 1 32 40 23 $white $true))
$header.Controls.Add((New-Label "LinkMover" 88 25 300 34 20 $white $true))
$header.Controls.Add((New-Label "GERENCIADOR DE CAMINHOS VIRTUAIS" 90 62 420 20 8 ([Drawing.Color]::FromArgb(156,163,175)) $true))
$header.Controls.Add((New-Label "MOVE UMA VEZ. MANTENHA O CAMINHO ORIGINAL." 28 94 690 20 9 ([Drawing.Color]::FromArgb(209,213,219)) $false))

$form.Controls.Add((New-Label "Organize o armazenamento do seu programa" 26 143 700 29 15 $ink $true))
$form.Controls.Add((New-Label "Escolha a pasta atual e onde os arquivos devem ficar." 27 173 700 22 9 $muted $false))

# Cartao origem
$sourceCard=New-Card 24 210 712 104
$sourceCard.Anchor="Top,Left,Right"; $form.Controls.Add($sourceCard)
$sourceCard.Controls.Add((New-Label "01" 16 16 30 25 10 $blue $true))
$sourceCard.Controls.Add((New-Label "PASTA DE ORIGEM" 50 15 250 22 9 $ink $true))
$sourceCard.Controls.Add((New-Label "Selecione a pasta real atualmente instalada." 50 37 600 20 9 $muted $false))
$sourceBox=New-Object Windows.Forms.TextBox
$sourceBox.Location=New-Object Drawing.Point(18,64); $sourceBox.Size=New-Object Drawing.Size(555,27)
$sourceBox.Anchor="Top,Left,Right"; $sourceBox.BorderStyle=[Windows.Forms.BorderStyle]::FixedSingle
$sourceBox.Font=New-Object Drawing.Font("Segoe UI",9); $sourceCard.Controls.Add($sourceBox)
$sourceButton=New-Button "Procurar..." 584 62 110 30 $false
$sourceButton.Anchor="Top,Right"; $sourceCard.Controls.Add($sourceButton)

# Cartao destino
$destCard=New-Card 24 326 712 104
$destCard.Anchor="Top,Left,Right"; $form.Controls.Add($destCard)
$destCard.Controls.Add((New-Label "02" 16 16 30 25 10 $green $true))
$destCard.Controls.Add((New-Label "LOCAL DE DESTINO" 50 15 250 22 9 $ink $true))
$destCard.Controls.Add((New-Label "A pasta do programa sera criada aqui. Digite tambem um caminho que ainda nao exista." 50 37 620 20 9 $muted $false))
$destBox=New-Object Windows.Forms.TextBox
$destBox.Location=New-Object Drawing.Point(18,64); $destBox.Size=New-Object Drawing.Size(555,27)
$destBox.Anchor="Top,Left,Right"; $destBox.BorderStyle=[Windows.Forms.BorderStyle]::FixedSingle
$destBox.Font=New-Object Drawing.Font("Segoe UI",9); $destCard.Controls.Add($destBox)
$destButton=New-Button "Procurar..." 584 62 110 30 $false
$destButton.Anchor="Top,Right"; $destCard.Controls.Add($destButton)

# Seguranca, progresso e acoes
$safety=New-Object Windows.Forms.Panel
$safety.Location=New-Object Drawing.Point(24,442); $safety.Size=New-Object Drawing.Size(712,48)
$safety.BackColor=[Drawing.Color]::FromArgb(232,242,255); $safety.Anchor="Top,Left,Right"; $form.Controls.Add($safety)
$safety.Controls.Add((New-Label "i" 14 11 20 25 13 $blue $true))
$safety.Controls.Add((New-Label "O LinkMover cria os diretorios ausentes e verifica os arquivos antes da troca." 39 8 655 32 9 ([Drawing.Color]::FromArgb(30,64,120)) $false))
$progress=New-Object Windows.Forms.ProgressBar
$progress.Location=New-Object Drawing.Point(25,506); $progress.Size=New-Object Drawing.Size(710,7)
$progress.Minimum=0; $progress.Maximum=100; $progress.Style=[Windows.Forms.ProgressBarStyle]::Continuous
$progress.Anchor="Top,Left,Right"; $form.Controls.Add($progress)
$status=New-Label "Pronto para selecionar as pastas." 26 518 420 24 9 $muted $false
$status.Anchor="Top,Left,Right"; $form.Controls.Add($status)
$runButton=New-Button "Mover e criar link  →" 485 514 251 38 $true
$runButton.Anchor="Top,Right"; $form.Controls.Add($runButton)

$sd=New-Object Windows.Forms.FolderBrowserDialog
$sd.Description="Selecione a pasta real do programa instalado"
$dd=New-Object Windows.Forms.FolderBrowserDialog
$dd.Description="Selecione o diretorio onde os arquivos serao armazenados"
$sourceButton.Add_Click({if($sd.ShowDialog($form) -eq "OK"){$sourceBox.Text=$sd.SelectedPath}})
$destButton.Add_Click({if($dd.ShowDialog($form) -eq "OK"){$destBox.Text=$dd.SelectedPath}})

$runButton.Add_Click({
 if([string]::IsNullOrWhiteSpace($sourceBox.Text) -or [string]::IsNullOrWhiteSpace($destBox.Text)){
  [Windows.Forms.MessageBox]::Show($form,"Selecione a pasta de origem e o local de destino.","Campos obrigatorios",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Warning) | Out-Null; return
 }
 try {
  $src=[IO.Path]::GetFullPath($sourceBox.Text.Trim())
  $srcRoot=[IO.Path]::GetPathRoot($src); if($src.Length -gt $srcRoot.Length){$src=$src.TrimEnd('\')}
  if($src.Equals($srcRoot,[StringComparison]::OrdinalIgnoreCase)){[Windows.Forms.MessageBox]::Show($form,"Nao selecione a raiz do disco como origem.","Origem invalida",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Warning) | Out-Null; return}
  $parent=[IO.Path]::GetFullPath($destBox.Text.Trim())
  $parentRoot=[IO.Path]::GetPathRoot($parent); if($parent.Length -gt $parentRoot.Length){$parent=$parent.TrimEnd('\')}
  if(!(Test-Path -LiteralPath $src -PathType Container)){
   [Windows.Forms.MessageBox]::Show($form,"A pasta de origem nao existe ou nao e uma pasta.","Origem invalida",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Error) | Out-Null; return
  }
  if((Test-Path -LiteralPath $parent) -and !(Test-Path -LiteralPath $parent -PathType Container)){
   [Windows.Forms.MessageBox]::Show($form,"O caminho de destino existe, mas nao e uma pasta. Escolha outro caminho.","Destino invalido",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Error) | Out-Null; return
  }
  $item=Get-Item -LiteralPath $src -Force
  if(($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0){
   [Windows.Forms.MessageBox]::Show($form,"A origem ja e um link simbolico. Selecione a pasta real.","Origem invalida",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Warning) | Out-Null; return
  }
  $leaf=Split-Path $src -Leaf; $target=Join-Path $parent $leaf
  if($src.Equals($target,[StringComparison]::OrdinalIgnoreCase) -or $target.StartsWith($src+'\',[StringComparison]::OrdinalIgnoreCase) -or $src.StartsWith($target+'\',[StringComparison]::OrdinalIgnoreCase)){
   [Windows.Forms.MessageBox]::Show($form,"Origem e destino nao podem ser iguais nem estar um dentro do outro.","Caminhos incompativeis",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Warning) | Out-Null; return
  }
  if(Test-Path -LiteralPath $target){[Windows.Forms.MessageBox]::Show($form,"O destino ja existe. Escolha outro diretorio vazio.","Destino ocupado",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Warning) | Out-Null; return}
  $nl=[Environment]::NewLine
  $msg="ORIGEM"+$nl+$src+$nl+$nl+"DESTINO REAL"+$nl+$target+$nl+$nl+"Feche o programa antes de continuar. A pasta original sera substituida por um link simbolico. Deseja prosseguir?"
  if([Windows.Forms.MessageBox]::Show($form,$msg,"Confirmar operacao",[Windows.Forms.MessageBoxButtons]::YesNo,[Windows.Forms.MessageBoxIcon]::Warning) -ne [Windows.Forms.DialogResult]::Yes){return}
  $runButton.Enabled=$false; $sourceButton.Enabled=$false; $destButton.Enabled=$false
  $backup=$src+".linkmover-backup-"+(Get-Date -Format "yyyyMMddHHmmss")
  $copied=$false; $renamed=$false
  try {
   $status.Text="Copiando arquivos..."; $status.ForeColor=$blue; $progress.Value=10; [Windows.Forms.Application]::DoEvents()
   if(!(Test-Path -LiteralPath $parent -PathType Container)){
    $status.Text="Criando diretorios de destino..."; [Windows.Forms.Application]::DoEvents()
    New-Item -ItemType Directory -Path $parent -Force -ErrorAction Stop | Out-Null
   }
   New-Item -ItemType Directory -Path $target -ErrorAction Stop | Out-Null
   $copied=$true
   Copy-SafeDirectory -source $src -destination $target
   $progress.Value=55; $status.Text="Verificando quantidade e tamanho dos arquivos..."; [Windows.Forms.Application]::DoEvents()
   $sourceInventory=Get-SafeFileInventory -root $src
   $targetInventory=Get-SafeFileInventory -root $target
   if($sourceInventory.Count -ne $targetInventory.Count -or $sourceInventory.Bytes -ne $targetInventory.Bytes){throw ("A verificacao falhou. Origem: "+$sourceInventory.Count+" arquivos / "+$sourceInventory.Bytes+" bytes; destino: "+$targetInventory.Count+" arquivos / "+$targetInventory.Bytes+" bytes. A origem foi mantida.")}
   $status.Text="Preservando a origem temporariamente..."; $progress.Value=70; [Windows.Forms.Application]::DoEvents()
   Rename-Item -LiteralPath $src -NewName (Split-Path $backup -Leaf) -ErrorAction Stop; $renamed=$true
   $status.Text="Criando link simbolico..."; $progress.Value=85; [Windows.Forms.Application]::DoEvents()
   New-Item -ItemType SymbolicLink -Path $src -Target $target -ErrorAction Stop | Out-Null
   $status.Text="Link criado. Limpando copia temporaria..."; $progress.Value=95; [Windows.Forms.Application]::DoEvents()
   $cleanupWarning=$null; try { Remove-Item -LiteralPath $backup -Recurse -Force -ErrorAction Stop } catch { $cleanupWarning="O link foi criado, mas a copia temporaria nao pode ser removida. Ela foi preservada em: "+$backup+". Detalhes: "+$_.Exception.Message }
   $progress.Value=100; $status.Text="Concluido com sucesso."; $status.ForeColor=$green
   if($cleanupWarning){$doneText="O link foi criado com sucesso."+$nl+$nl+"Caminho virtual:"+$nl+$src+$nl+$nl+"Arquivos armazenados em:"+$nl+$target+$nl+$nl+$cleanupWarning; $status.Text="Concluido; copia temporaria preservada."; $status.ForeColor=[Drawing.Color]::FromArgb(180,83,9)} else {$doneText="Operacao concluida com sucesso!"+$nl+$nl+"Caminho virtual:"+$nl+$src+$nl+$nl+"Arquivos armazenados em:"+$nl+$target}; [Windows.Forms.MessageBox]::Show($form,$doneText,"LinkMover - Concluido",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Information) | Out-Null
  } catch {
   $err=$_.Exception.Message
   if($renamed -and (Test-Path -LiteralPath $backup)){
    try {
     if(Test-Path -LiteralPath $src){
      $current=Get-Item -LiteralPath $src -Force -ErrorAction SilentlyContinue
      if($null -ne $current -and (($current.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0)){Remove-Item -LiteralPath $src -Force -ErrorAction Stop}
     }
     if(!(Test-Path -LiteralPath $src)){Rename-Item -LiteralPath $backup -NewName (Split-Path $src -Leaf) -ErrorAction Stop}
     else {$err+="; backup preservado em "+$backup}
    } catch {$err+="; restauracao manual necessaria. Backup preservado em "+$backup}
   }
   if($copied -and (Test-Path -LiteralPath $target) -and !(Test-Path -LiteralPath $backup)){$err+="; confira a copia no destino: "+$target}
   $status.Text="A operacao encontrou um problema."; $status.ForeColor=[Drawing.Color]::FromArgb(185,28,28)
   [Windows.Forms.MessageBox]::Show($form,"A operacao nao foi concluida:"+$nl+$nl+$err,"LinkMover - Falha",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Error) | Out-Null
  } finally {$runButton.Enabled=$true; $sourceButton.Enabled=$true; $destButton.Enabled=$true}
 } catch {
  [Windows.Forms.MessageBox]::Show($form,"Nao foi possivel validar os caminhos:"+[Environment]::NewLine+$_.Exception.Message,"Erro de caminho",[Windows.Forms.MessageBoxButtons]::OK,[Windows.Forms.MessageBoxIcon]::Error) | Out-Null
 }
})
[void]$form.ShowDialog()
