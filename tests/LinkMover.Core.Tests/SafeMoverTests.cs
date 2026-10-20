using System.Security.Cryptography;
using System.Text;
using LinkMover.Core;
using Xunit;

namespace LinkMover.Core.Tests;

public class SafeMoverTests : IDisposable
{
    private readonly string _testRoot;

    public SafeMoverTests()
    {
        _testRoot = Path.Combine(Path.GetTempPath(), "LinkMover-xUnit-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_testRoot);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_testRoot))
            {
                Directory.Delete(_testRoot, true);
            }
        }
        catch
        {
            // Best effort temp cleanup
        }
    }

    [Fact]
    public void BuildInventory_ValidStructure_ReturnsCorrectFilesAndBytes()
    {
        var source = Path.Combine(_testRoot, "src-valid");
        var nested = Path.Combine(source, "nested", "sub");
        Directory.CreateDirectory(nested);

        File.WriteAllText(Path.Combine(source, "file1.txt"), "hello", Encoding.UTF8);
        File.WriteAllBytes(Path.Combine(nested, "file2.bin"), new byte[] { 1, 2, 3, 4, 5 });
        File.WriteAllText(Path.Combine(nested, "unicode-ação.txt"), "conteúdo com acentuação e emoji 🚀", Encoding.UTF8);

        var inventory = SafeMover.BuildInventory(source);

        Assert.Equal(3, inventory.Files);
        var expectedBytes = new FileInfo(Path.Combine(source, "file1.txt")).Length
                          + new FileInfo(Path.Combine(nested, "file2.bin")).Length
                          + new FileInfo(Path.Combine(nested, "unicode-ação.txt")).Length;
        Assert.Equal(expectedBytes, inventory.Bytes);
        Assert.Equal(3, inventory.Hashes.Count);
        Assert.True(inventory.Hashes.ContainsKey("file1.txt"));
        Assert.True(inventory.Hashes.ContainsKey("nested/sub/file2.bin"));
        Assert.True(inventory.Hashes.ContainsKey("nested/sub/unicode-ação.txt"));
    }

    [Fact]
    public void BuildInventory_EmptyDirectory_ReturnsZeroCounts()
    {
        var source = Path.Combine(_testRoot, "src-empty");
        Directory.CreateDirectory(source);

        var inventory = SafeMover.BuildInventory(source);

        Assert.Equal(0, inventory.Files);
        Assert.Equal(0, inventory.Bytes);
        Assert.Empty(inventory.Hashes);
    }

    [Fact]
    public async Task MoveToLinkedFolderAsync_NonExistentSource_ThrowsDirectoryNotFoundException()
    {
        var nonExistentSource = Path.Combine(_testRoot, "non-existent-" + Guid.NewGuid().ToString("N"));
        var targetParent = Path.Combine(_testRoot, "target");
        Directory.CreateDirectory(targetParent);

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            SafeMover.MoveToLinkedFolderAsync(nonExistentSource, targetParent));
    }

    [Fact]
    public async Task MoveToLinkedFolderAsync_NonExistentTargetParent_ThrowsDirectoryNotFoundException()
    {
        var source = Path.Combine(_testRoot, "source-dir");
        Directory.CreateDirectory(source);
        var nonExistentTargetParent = Path.Combine(_testRoot, "missing-parent", "nested");

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() =>
            SafeMover.MoveToLinkedFolderAsync(source, nonExistentTargetParent));
    }

    [Fact]
    public async Task MoveToLinkedFolderAsync_TargetAlreadyExists_ThrowsIOException()
    {
        var source = Path.Combine(_testRoot, "source-dir");
        Directory.CreateDirectory(source);
        var targetParent = Path.Combine(_testRoot, "target-parent");
        Directory.CreateDirectory(targetParent);

        // Pre-create destination with same folder name
        var existingDest = Path.Combine(targetParent, "source-dir");
        Directory.CreateDirectory(existingDest);

        var ex = await Assert.ThrowsAsync<IOException>(() =>
            SafeMover.MoveToLinkedFolderAsync(source, targetParent));

        Assert.Contains("O destino já existe", ex.Message);
    }

    [Fact]
    public async Task MoveToLinkedFolderAsync_NestedPaths_ThrowsIOException()
    {
        var parent = Path.Combine(_testRoot, "parent");
        var child = Path.Combine(parent, "child");
        Directory.CreateDirectory(child);

        var ex = await Assert.ThrowsAsync<IOException>(() =>
            SafeMover.MoveToLinkedFolderAsync(parent, child));

        Assert.Contains("não podem conter um ao outro", ex.Message);
    }

    [Fact]
    public async Task MoveToLinkedFolderAsync_FullCycle_EitherSucceedsOrRollsBackSafely()
    {
        var source = Path.Combine(_testRoot, "SafeApp");
        var targetParent = Path.Combine(_testRoot, "Storage");
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(targetParent);

        File.WriteAllText(Path.Combine(source, "data.txt"), "Important Content");
        var initialHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("Important Content")));

        try
        {
            var result = await SafeMover.MoveToLinkedFolderAsync(source, targetParent);

            // Se o ambiente tem privilégios para symlink (Developer Mode ou UAC):
            Assert.True(Directory.Exists(source));
            var di = new DirectoryInfo(source);
            Assert.NotNull(di.LinkTarget);
            Assert.Equal(1, result.Files);
            Assert.True(File.Exists(Path.Combine(targetParent, "SafeApp", "data.txt")));
        }
        catch (IOException ex) when (ex.Message.Contains("privilégio necessário") || ex.InnerException?.Message.Contains("privilégio necessário") == true)
        {
            // Se falhou por privilégio de symlink, o rollback DEVE ter preservado a origem intacta!
            Assert.True(Directory.Exists(source), "Rollback falhou: a pasta de origem não existe.");
            Assert.Null(new DirectoryInfo(source).LinkTarget);
            Assert.True(File.Exists(Path.Combine(source, "data.txt")), "Rollback falhou: o arquivo original foi perdido.");
            var content = await File.ReadAllTextAsync(Path.Combine(source, "data.txt"));
            Assert.Equal("Important Content", content);
            Assert.False(Directory.Exists(Path.Combine(targetParent, "SafeApp")), "A pasta de destino parcial deveria ter sido removida no rollback.");
        }
    }
}
