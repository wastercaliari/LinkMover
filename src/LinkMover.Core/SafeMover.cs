using System.Security.Cryptography;
namespace LinkMover.Core;
public sealed record MoveResult(string Source,string Destination,string Backup,int Files,long Bytes,bool BackupRemoved,string Message);
public sealed record Inventory(int Files,long Bytes,IReadOnlyDictionary<string,string> Hashes);
public static class SafeMover {
 public static async Task<MoveResult> MoveToLinkedFolderAsync(string source,string destinationParent,IProgress<string>? progress=null,CancellationToken cancellationToken=default) {
  source=Path.GetFullPath(source.Trim()); destinationParent=Path.GetFullPath(destinationParent.Trim());
  if(!Directory.Exists(source)) throw new DirectoryNotFoundException("A pasta de origem não existe: "+source);
  if(!Directory.Exists(destinationParent)) throw new DirectoryNotFoundException("A pasta pai de destino não existe: "+destinationParent);
  var si=new DirectoryInfo(source); if((si.Attributes&FileAttributes.ReparsePoint)!=0) throw new IOException("A origem já é um link ou ponto de reanálise.");
  var root=Path.GetPathRoot(source); if(string.Equals(Path.TrimEndingDirectorySeparator(source),Path.TrimEndingDirectorySeparator(root??""),Cmp)) throw new IOException("Não é permitido mover a raiz de uma unidade.");
  var dest=Path.Combine(destinationParent,si.Name); if(SamePath(source,dest)) throw new IOException("Origem e destino são iguais.");
  if(Inside(dest,source)||Inside(source,dest)) throw new IOException("Origem e destino não podem conter um ao outro.");
  if(Directory.Exists(dest)||File.Exists(dest)) throw new IOException("O destino já existe: "+dest);
  var backup=source+".linkmover-backup-"+DateTime.UtcNow.ToString("yyyyMMddHHmmss")+"-"+Guid.NewGuid().ToString("N")[..8];
  progress?.Report("Verificando arquivos e links..."); var before=await Task.Run(()=>BuildInventory(source,cancellationToken),cancellationToken); cancellationToken.ThrowIfCancellationRequested();
  bool dc=false,sr=false,lc=false;
  try {
   progress?.Report("Copiando arquivos..."); Directory.CreateDirectory(dest); dc=true; await Task.Run(()=>CopyTree(source,dest,progress,cancellationToken),cancellationToken);
   progress?.Report("Validando cópia por SHA-256..."); var after=await Task.Run(()=>BuildInventory(dest,cancellationToken),cancellationToken);
   if(before.Files!=after.Files||before.Bytes!=after.Bytes||!SameHashes(before.Hashes,after.Hashes)) throw new IOException("A cópia não passou na validação SHA-256. A origem não será substituída.");
   cancellationToken.ThrowIfCancellationRequested(); progress?.Report("Criando substituição reversível..."); Directory.Move(source,backup); sr=true; Directory.CreateSymbolicLink(source,dest); lc=true;
   var li=new DirectoryInfo(source); li.Refresh(); if((li.Attributes&FileAttributes.ReparsePoint)==0||!Directory.Exists(source)) throw new IOException("O sistema não confirmou o link simbólico.");
   var resolved=li.ResolveLinkTarget(true); if(resolved is null||!SamePath(resolved.FullName,dest)) throw new IOException("O link não aponta para o destino esperado.");
   progress?.Report("Link verificado. Finalizando..."); bool removed=false; try { Directory.Delete(backup,true); removed=true; } catch(Exception ex) { progress?.Report("AVISO: backup mantido em "+backup+". "+ex.Message); }
   return new MoveResult(source,dest,backup,before.Files,before.Bytes,removed,removed?"Concluído. Cópia validada e link verificado.":"Link validado, mas o backup foi mantido por segurança.");
  } catch(Exception failure) {
   progress?.Report("Falha. Tentando restaurar a origem..."); var errors=new List<string>();
   if(lc||(Directory.Exists(source)&&new DirectoryInfo(source).LinkTarget is not null)) try { Directory.Delete(source); } catch(Exception ex) { errors.Add("Não foi possível remover link: "+ex.Message); }
   if(sr&&Directory.Exists(backup)&&!Directory.Exists(source)) try { Directory.Move(backup,source); } catch(Exception ex) { errors.Add("Backup preservado em "+backup+". Restauração falhou: "+ex.Message); }
   if(dc&&Directory.Exists(dest)) try { Directory.Delete(dest,true); } catch(Exception ex) { errors.Add("Cópia parcial preservada em "+dest+". "+ex.Message); }
   throw new IOException(failure.Message+Environment.NewLine+(errors.Count==0?"A recuperação foi concluída.":string.Join(Environment.NewLine,errors)),failure);
  }
 }
 public static Inventory BuildInventory(string root,CancellationToken token=default) {
  var hashes=new SortedDictionary<string,string>(StringComparer.Ordinal); long bytes=0; var dirs=new Stack<string>(); dirs.Push(root);
  while(dirs.Count>0) { token.ThrowIfCancellationRequested(); foreach(var entry in Directory.EnumerateFileSystemEntries(dirs.Pop())) { token.ThrowIfCancellationRequested(); var attr=File.GetAttributes(entry);
   if((attr&FileAttributes.ReparsePoint)!=0) throw new IOException("Link/junção encontrado; operação bloqueada: "+entry);
   if((attr&FileAttributes.Directory)!=0) dirs.Push(entry); else { var rel=Path.GetRelativePath(root,entry).Replace('\\','/'); using var fs=new FileStream(entry,FileMode.Open,FileAccess.Read,FileShare.Read,1024*1024,FileOptions.SequentialScan); hashes.Add(rel,Convert.ToHexString(SHA256.HashData(fs))); bytes+=fs.Length; }
  } }
  return new Inventory(hashes.Count,bytes,hashes);
 }
 private static void CopyTree(string source,string dest,IProgress<string>? progress,CancellationToken token) {
  var stack=new Stack<(string S,string D)>(); stack.Push((source,dest)); int count=0;
  while(stack.Count>0) { token.ThrowIfCancellationRequested(); var cur=stack.Pop(); foreach(var e in Directory.EnumerateFileSystemEntries(cur.S)) { token.ThrowIfCancellationRequested(); var a=File.GetAttributes(e); if((a&FileAttributes.ReparsePoint)!=0) throw new IOException("Link/junção encontrado durante cópia: "+e); var t=Path.Combine(cur.D,Path.GetFileName(e));
   if((a&FileAttributes.Directory)!=0) { Directory.CreateDirectory(t); stack.Push((e,t)); } else { using var input=new FileStream(e,FileMode.Open,FileAccess.Read,FileShare.Read,1024*1024,FileOptions.SequentialScan); using var output=new FileStream(t,FileMode.CreateNew,FileAccess.Write,FileShare.None,1024*1024,FileOptions.SequentialScan); input.CopyTo(output); output.Flush(true); count++; if(count%100==0) progress?.Report("Arquivos copiados: "+count); }
  } }
 }
 private static bool SameHashes(IReadOnlyDictionary<string,string> a,IReadOnlyDictionary<string,string> b)=>a.Count==b.Count&&a.All(x=>b.TryGetValue(x.Key,out var v)&&string.Equals(x.Value,v,StringComparison.Ordinal));
 private static bool Inside(string path,string parent) { var r=Path.GetRelativePath(parent,path); return r=="."||(!Path.IsPathRooted(r)&&r!=".."&&!r.StartsWith(".."+Path.DirectorySeparatorChar,StringComparison.Ordinal)); }
 private static bool SamePath(string a,string b)=>string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(a)),Path.TrimEndingDirectorySeparator(Path.GetFullPath(b)),Cmp);
 private static StringComparison Cmp=>OperatingSystem.IsWindows()?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal;
}