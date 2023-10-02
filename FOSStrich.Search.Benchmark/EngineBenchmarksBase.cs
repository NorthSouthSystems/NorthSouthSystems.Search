namespace FOSStrich.Search;

using FOSStrich.StackExchange;

public abstract class EngineBenchmarksBase
{
    private static readonly int[] _powersOfTen = new[] { 1, 10, 100, 1_000, 10_000, 100_000, 1_000_000 };

    protected Engine<Post, int> ConstructEngine(bool allowUnsafe, VectorCompression compression)
    {
        Engine = new Engine<Post, int>(allowUnsafe, post => post.Id);

        PostTypeCatalog = Engine.CreateCatalog(nameof(Post.PostTypeId), compression, post => post.PostTypeId);
        CreationDateCatalog = Engine.CreateCatalog(nameof(Post.CreationDate), compression, post => YearMonth(post.CreationDate));
        LastActivityDateCatalog = Engine.CreateCatalog(nameof(Post.LastActivityDate), compression, post => YearMonth(post.LastActivityDate));
        ScoreCatalog = Engine.CreateCatalog(nameof(Post.Score), compression, post => post.Score);
        ViewCountCatalog = Engine.CreateCatalog(nameof(Post.ViewCount), compression, post => OneSigFig(post.ViewCount));
        OwnerUserIdCatalog = Engine.CreateCatalog(nameof(Post.OwnerUserId), compression, post => post.OwnerUserId);
        TagsCatalog = Engine.CreateCatalog(nameof(Post.Tags), compression, post => post.Tags);
        AnswerCountCatalog = Engine.CreateCatalog(nameof(Post.AnswerCount), compression, post => OneSigFig(post.AnswerCount));
        CommentCountCatalog = Engine.CreateCatalog(nameof(Post.CommentCount), compression, post => OneSigFig(post.CommentCount));
        FavoriteCountCatalog = Engine.CreateCatalog(nameof(Post.FavoriteCount), compression, post => OneSigFig(post.FavoriteCount));

        return Engine;

        DateTime YearMonth(DateTime date) => new(date.Year, date.Month, 1);

        int OneSigFig(int count)
        {
            for (int i = 1; i < _powersOfTen.Length; i++)
                if (count < _powersOfTen[i])
                    return FloorToFactor(_powersOfTen[i - 1]);

            return FloorToFactor(_powersOfTen.Last());

            int FloorToFactor(int factor) => (count / factor) * factor;
        }
    }

    public Engine<Post, int> Engine { get; private set; }

    public ICatalogHandle<byte> PostTypeCatalog { get; private set; }
    public ICatalogHandle<DateTime> CreationDateCatalog { get; private set; }
    public ICatalogHandle<DateTime> LastActivityDateCatalog { get; private set; }
    public ICatalogHandle<int> ScoreCatalog { get; private set; }
    public ICatalogHandle<int> ViewCountCatalog { get; private set; }
    public ICatalogHandle<int> OwnerUserIdCatalog { get; private set; }
    public ICatalogHandle<string> TagsCatalog { get; private set; }
    public ICatalogHandle<int> AnswerCountCatalog { get; private set; }
    public ICatalogHandle<int> CommentCountCatalog { get; private set; }
    public ICatalogHandle<int> FavoriteCountCatalog { get; private set; }
}