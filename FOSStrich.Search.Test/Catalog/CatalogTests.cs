namespace FOSStrich.Search;

public class CatalogTests
{
    [Fact]
    public void SortBitPositions() =>
        SafetyAndCompression.RunAll(safetyAndCompression =>
        {
            var catalog = new Catalog<int>("SomeInt", true, safetyAndCompression.AllowUnsafe, safetyAndCompression.Compression);
            catalog.Set(0, 5, true);
            catalog.Set(1, 6, true);
            catalog.Set(2, 7, true);
            catalog.Set(3, 8, true);
            catalog.Set(4, 9, true);

            var vector = new Vector(safetyAndCompression.AllowUnsafe, VectorCompression.None);
            vector[4] = true;
            vector[5] = true;
            vector[6] = true;
            vector[7] = true;
            vector[8] = true;
            vector[9] = true;
            vector[10] = true;

            foreach (bool disableParallel in new[] { false, true })
            {
                var result = catalog.Sort(vector, true, false, disableParallel);
                int[] bitPositions = result.PartialSorts.SelectMany(partial => partial.GetBitPositions(true)).ToArray();
                bitPositions.Should().Equal(9, 8, 7, 6, 5);

                var partialSorts = result.PartialSorts.ToArray();
                partialSorts.Length.Should().Be(5);

                for (int i = 0; i < 5; i++)
                    partialSorts[i].GetBitPositions(true).Single().Should().Be(9 - i);
            }
        });

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Set((string)null, 777, true);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "SetNull");

        act = () =>
        {
            var catalog = new Catalog<int>("SomeInt", true, false, VectorCompression.None);
            catalog.Set((int[])null, 777, true);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "SetEnumerableNull");

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A");
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterExactVectorNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterExactKeyNull");

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, new[] { "A", "B" });
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterEnumerableVectorNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string[])null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterEnumerableKeysNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, new[] { "A", null });
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterEnumerableKeysKeyNull");

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(null, "A", "B");
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterRangeVectorNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, "A");
            catalog.Filter(vector, "A", (string)null);
        };
        act.Should().NotThrow(because: "FilterRangeKeyMinMaxOK");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, (string)null, (string)null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FilterRangeKeyMinMaxNull");

        act = () =>
        {
            var vector = new Vector(false, VectorCompression.Compressed);

            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Filter(vector, "B", "A");
        };
        act.Should().ThrowExactly<ArgumentOutOfRangeException>(because: "FilterRangeKeyMinMaxOutOfRange");

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Facet(null);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "FacetVectorNull");

        act = () =>
        {
            var catalog = new Catalog<string>("SomeString", true, false, VectorCompression.None);
            catalog.Sort(null, true, true, false);
        };
        act.Should().ThrowExactly<ArgumentNullException>(because: "SortBitPositionsVectorNull");
    }
}