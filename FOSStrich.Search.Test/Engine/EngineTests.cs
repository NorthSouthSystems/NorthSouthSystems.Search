namespace FOSStrich.Search;

[TestClass]
public class EngineTests
{
    [TestMethod]
    public void Construction()
    {
        using var engine = new Engine<SimpleItem, int>(false, item => item.Id);

        engine.AllowUnsafe.Should().BeFalse();
    }

    private class SimpleItem
    {
        public int Id;
        public int SomeInt;
    }

    [TestMethod]
    public void AmongstPrimaryKeyOutOfRange() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            using var engine1 = new Engine<SimpleItem, int>(safetyAndCompression.AllowUnsafe, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", safetyAndCompression.Compression, item => item.SomeInt);

            engine1.Add(new SimpleItem { Id = 43, SomeInt = 0 });

            var query = engine1.CreateQuery();
            query.Amongst(new[] { 43, 44 });

            query.Execute(0, 10);

            query.ResultTotalCount.Should().Be(1);
            query.ResultPrimaryKeys.Length.Should().Be(1);
            query.ResultPrimaryKeys[0].Should().Be(43);
        });

    [TestMethod]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            using var engine1 = new Engine<EngineItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            engine1.Add(EngineItem.CreateItems(id => id, id => DateTime.Now, id => id.ToString(), id => Array.Empty<string>(), 1).Single());

            var catalog2 = engine1.CreateCatalog("SomeString", VectorCompression.None, item => item.SomeString);
        };
        act.Should().ThrowExactly<NotSupportedException>(because: "CreateCatalogNotInitializing");

        act = () =>
        {
            using var engine1 = new Engine<EngineItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeInt);
            var catalog2 = engine1.CreateCatalog("Name", VectorCompression.None, item => item.SomeString);
        };
        act.Should().ThrowExactly<ArgumentException>(because: "CreateCatalogDuplicateName");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Add(new SimpleItem { Id = 0, SomeInt = 1 });
        };
        act.Should().ThrowExactly<ArgumentException>(because: "AddDuplicatePrimaryKey");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
            engine1.Add((SimpleItem[])null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "AddRangeNull");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Update(new SimpleItem { Id = 1, SomeInt = 1 });
        };
        act.Should().ThrowExactly<ArgumentException>(because: "UpdateNoPrimaryKey");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
            engine1.Update((SimpleItem[])null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "UpdateRangeNull");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Remove(new SimpleItem { Id = 1, SomeInt = 1 });
        };
        act.Should().ThrowExactly<ArgumentException>(because: "RemoveNoPrimaryKey");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);

            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Remove(new SimpleItem { Id = 0, SomeInt = 0 });
            engine1.Add(new SimpleItem { Id = 0, SomeInt = 0 });
        };
        act.Should().NotThrow(because: "RemoveReAddPrimaryKey");

        act = () =>
        {
            using var engine1 = new Engine<SimpleItem, int>(false, item => item.Id);

            var catalog1 = engine1.CreateCatalog("SomeInt", VectorCompression.None, item => item.SomeInt);
            engine1.Remove((SimpleItem[])null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "RemoveRangeNull");
    }
}