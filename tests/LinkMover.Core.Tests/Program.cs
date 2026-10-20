using LinkMover.Core;
using System.Text;
var temp=Path.Combine(Path.GetTempPath(),"LinkMover-Test-"+Guid.NewGuid().ToString("N"));
var source=Path.Combine(temp,"source","DemoApp");
var targetParent=Path.Combine(temp,"storage");
Directory.CreateDirectory(Path.Combine(source,"nested","æ·±åº¦"));
Directory.CreateDirectory(targetParent);
File.WriteAllText(Path.Combine(source,"alpha.txt"),"alpha-test");
File.WriteAllText(Path.Combine(source,"nested","data.bin"),"0123456789");
File.WriteAllBytes(Path.Combine(source,"nested","zero.bin"),Array.Empty<byte>());
File.WriteAllText(Path.Combine(source,"nested","æ·±åº¦","unicode-aÃ§Ã£o.txt"),"OlÃ¡, mundo!");
try {
 var inventory=SafeMover.BuildInventory(source);
 var expectedBytes=Encoding.UTF8.GetByteCount("alpha-test")+10+Encoding.UTF8.GetByteCount("OlÃ¡, mundo!");
 if(inventory.Files!=4) throw new Exception("Inventory count mismatch: "+inventory.Files);
 if(inventory.Bytes!=expectedBytes) throw new Exception("Inventory byte count mismatch: "+inventory.Bytes+" expected "+expectedBytes);
 Console.WriteLine($"INVENTORY_OK files={inventory.Files} bytes={inventory.Bytes}");
 var result=await SafeMover.MoveToLinkedFolderAsync(source,targetParent,new Progress<string>(s=>Console.WriteLine("STEP "+s)));
 if(!Directory.Exists(source)||new DirectoryInfo(source).LinkTarget is null) throw new Exception("The source path is not a symlink after move.");
 if(!File.Exists(Path.Combine(targetParent,"DemoApp","nested","æ·±åº¦","unicode-aÃ§Ã£o.txt"))) throw new Exception("Destination file missing.");
 if(!result.BackupRemoved) throw new Exception("Backup was retained unexpectedly: "+result.Backup);
 Console.WriteLine($"MOVE_E2E_OK files={result.Files} bytes={result.Bytes} backupRemoved={result.BackupRemoved}");
} catch(Exception ex) {
 if(ex.Message.Contains("privilégio necessário",StringComparison.OrdinalIgnoreCase) && Directory.Exists(source) && new DirectoryInfo(source).LinkTarget is null && !Directory.Exists(Path.Combine(targetParent,"DemoApp"))) { Console.WriteLine("ROLLBACK_OK_LINK_PRIVILEGE_DENIED: origem restaurada e cópia removida; habilite Developer Mode ou execute com permissão adequada para testar sucesso."); } else { Console.Error.WriteLine("TEST_FAILURE: "+ex); if(!Directory.Exists(source)) Console.Error.WriteLine("SAFETY_CHECK_FAILED: original source is missing."); Environment.ExitCode=1; }
} finally { try { Directory.Delete(temp,true); } catch(Exception ex) { Console.Error.WriteLine("TEMP_CLEANUP_WARNING: "+ex.Message); } }


