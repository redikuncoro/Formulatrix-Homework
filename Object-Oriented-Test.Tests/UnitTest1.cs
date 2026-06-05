using System.Collections.Generic;
using RepositoryManager;

namespace Object_Oriented_Test.Tests;

public class RepositoryManagerTests
{
    private RepositoryManager.RepositoryManager CreateRepo() => new();

    // test the repo only able to initialize once, and that operations work without initialization
    [Fact]
    public void Initialize_OnlyOnce()
    {
        var repo = CreateRepo();
        repo.Initialize();
        Assert.Throws<InvalidOperationException>(() => repo.Initialize());
    }

    // -------------------------------------------------------------------------
    // 1. Register
    // -------------------------------------------------------------------------

    [Fact]
    public void Register_ValidJson_StoresItem()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        Assert.Equal("{\"key\":\"value\"}", repo.Retrieve("config"));
    }

    [Fact]
    public void Register_ValidXml_StoresItem()
    {
        var repo = CreateRepo();
        repo.Register("doc", "<root><node/></root>", 2);
        Assert.Equal("<root><node/></root>", repo.Retrieve("doc"));
    }

    [Fact]
    public void Register_DuplicateName_ThrowsInvalidOperationException()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        Assert.Throws<InvalidOperationException>(() =>
            repo.Register("config", "{\"other\":\"value\"}", 1));
    }

    [Fact]
    public void Register_InvalidJsonContent_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register("config", "not json", 1));
    }

    [Fact]
    public void Register_InvalidXmlContent_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register("doc", "<unclosed>", 2));
    }

    [Fact]
    public void Register_JsonContentWithXmlType_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register("item", "{}", 2));
    }

    [Fact]
    public void Register_NullItemName_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register(null!, "{\"key\":\"value\"}", 1));
    }

    [Fact]
    public void Register_EmptyItemName_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register("", "{\"key\":\"value\"}", 1));
    }

    [Fact]
    public void Register_NullContent_ThrowsArgumentNullException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentNullException>(() =>
            repo.Register("config", null!, 1));
    }

    [Fact]
    public void Register_UnknownItemType_ThrowsArgumentException()
    {
        var repo = CreateRepo();
        Assert.Throws<ArgumentException>(() =>
            repo.Register("config", "{\"key\":\"value\"}", 99));
    }

    // -------------------------------------------------------------------------
    // 2. Retrieve
    // -------------------------------------------------------------------------

    [Fact]
    public void Retrieve_RegisteredJsonItem_ReturnsOriginalContent()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        Assert.Equal("{\"key\":\"value\"}", repo.Retrieve("config"));
    }

    [Fact]
    public void Retrieve_RegisteredXmlItem_ReturnsOriginalContent()
    {
        var repo = CreateRepo();
        repo.Register("doc", "<root><node/></root>", 2);
        Assert.Equal("<root><node/></root>", repo.Retrieve("doc"));
    }

    [Fact]
    public void Retrieve_NonExistentItem_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.Retrieve("ghost"));
    }

    [Fact]
    public void Retrieve_AfterDeregister_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        repo.Deregister("config");
        Assert.Throws<KeyNotFoundException>(() => repo.Retrieve("config"));
    }

    // -------------------------------------------------------------------------
    // 3. GetType
    // -------------------------------------------------------------------------

    [Fact]
    public void GetType_JsonItem_Returns1()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        Assert.Equal(1, repo.GetType("config"));
    }

    [Fact]
    public void GetType_XmlItem_Returns2()
    {
        var repo = CreateRepo();
        repo.Register("doc", "<root/>", 2);
        Assert.Equal(2, repo.GetType("doc"));
    }

    [Fact]
    public void GetType_NonExistentItem_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.GetType("ghost"));
    }

    // -------------------------------------------------------------------------
    // 4. Deregister
    // -------------------------------------------------------------------------

    [Fact]
    public void Deregister_ExistingItem_RemovesIt()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        repo.Deregister("config");
        Assert.Throws<KeyNotFoundException>(() => repo.Retrieve("config"));
    }

    [Fact]
    public void Deregister_NonExistentItem_ThrowsKeyNotFoundException()
    {
        var repo = CreateRepo();
        Assert.Throws<KeyNotFoundException>(() => repo.Deregister("ghost"));
    }

    [Fact]
    public void Deregister_ThenReRegister_Succeeds()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        repo.Deregister("config");
        repo.Register("config", "{\"new\":\"value\"}", 1);
        Assert.Equal("{\"new\":\"value\"}", repo.Retrieve("config"));
    }

    // -------------------------------------------------------------------------
    // 5. Initialize
    // -------------------------------------------------------------------------

    [Fact]
    public void Initialize_CalledOnce_DoesNotThrow()
    {
        var repo = CreateRepo();
        repo.Initialize();
    }

    [Fact]
    public void Initialize_CalledTwice_ThrowsInvalidOperationException()
    {
        var repo = CreateRepo();
        repo.Initialize();
        Assert.Throws<InvalidOperationException>(() => repo.Initialize());
    }

    [Fact]
    public void Operations_WithoutInitialize_WorkNormally()
    {
        var repo = CreateRepo();
        repo.Register("config", "{\"key\":\"value\"}", 1);
        Assert.Equal("{\"key\":\"value\"}", repo.Retrieve("config"));
        Assert.Equal(1, repo.GetType("config"));
        repo.Deregister("config");
    }

    // -------------------------------------------------------------------------
    // 6. Thread Safety
    // -------------------------------------------------------------------------

    [Fact]
    public void ConcurrentRegister_DifferentNames_AllStored()
    {
        var repo = CreateRepo();
        int count = 100;
        var tasks = Enumerable.Range(0, count)
            .Select(i => Task.Run(() => repo.Register($"item{i}", $"{{\"id\":{i}}}", 1)))
            .ToArray();
        Task.WaitAll(tasks);

        for (int i = 0; i < count; i++)
            Assert.Equal($"{{\"id\":{i}}}", repo.Retrieve($"item{i}"));
    }

    [Fact]
    public void ConcurrentRegister_SameName_ExactlyOneSucceeds()
    {
        var repo = CreateRepo();
        int successCount = 0;
        int failCount = 0;
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() =>
            {
                try
                {
                    repo.Register("shared", "{\"key\":\"value\"}", 1);
                    Interlocked.Increment(ref successCount);
                }
                catch (InvalidOperationException)
                {
                    Interlocked.Increment(ref failCount);
                }
            }))
            .ToArray();
        Task.WaitAll(tasks);

        Assert.Equal(1, successCount);
        Assert.Equal(19, failCount);
    }

    [Fact]
    public void ConcurrentRetrieve_WhileRegisterInProgress_NoCorruption()
    {
        var repo = CreateRepo();
        repo.Register("existing", "{\"ready\":true}", 1);

        var tasks = Enumerable.Range(0, 50)
            .Select(i => Task.Run(() =>
            {
                if (i % 2 == 0)
                    repo.Register($"new{i}", $"{{\"id\":{i}}}", 1);
                else
                {
                    try { repo.Retrieve("existing"); }
                    catch (KeyNotFoundException) { /* acceptable */ }
                }
            }))
            .ToArray();

        // Should complete without exceptions related to data corruption
        Task.WaitAll(tasks);
    }

    [Fact]
    public void ConcurrentDeregister_SameItem_ExactlyOneSucceeds()
    {
        var repo = CreateRepo();
        repo.Register("shared", "{\"key\":\"value\"}", 1);

        int successCount = 0;
        int failCount = 0;
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() =>
            {
                try
                {
                    repo.Deregister("shared");
                    Interlocked.Increment(ref successCount);
                }
                catch (KeyNotFoundException)
                {
                    Interlocked.Increment(ref failCount);
                }
            }))
            .ToArray();
        Task.WaitAll(tasks);

        Assert.Equal(1, successCount);
        Assert.Equal(19, failCount);
    }

    [Fact]
    public void ConcurrentInitialize_ExactlyOneSucceeds()
    {
        var repo = CreateRepo();
        int successCount = 0;
        int failCount = 0;
        var tasks = Enumerable.Range(0, 20)
            .Select(_ => Task.Run(() =>
            {
                try
                {
                    repo.Initialize();
                    Interlocked.Increment(ref successCount);
                }
                catch (InvalidOperationException)
                {
                    Interlocked.Increment(ref failCount);
                }
            }))
            .ToArray();
        Task.WaitAll(tasks);

        Assert.Equal(1, successCount);
        Assert.Equal(19, failCount);
    }
}
