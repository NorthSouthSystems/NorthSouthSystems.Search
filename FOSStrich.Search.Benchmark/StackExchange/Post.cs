namespace FOSStrich.Search;

using MemoryPack;
using System.Xml.Linq;

[MemoryPackable]
public partial class Post
{
    [MemoryPackConstructor]
    private Post() { }

    internal Post(XElement xe)
    {
        Id = (int)xe.Attribute(nameof(Id));
        PostTypeId = (byte)(uint)xe.Attribute(nameof(PostTypeId));
        CreationDate = (DateTime)xe.Attribute(nameof(CreationDate));
        LastActivityDate = (DateTime)xe.Attribute(nameof(LastActivityDate));
        ViewCount = (int)xe.Attribute(nameof(ViewCount));
        OwnerUserId = ((int?)xe.Attribute(nameof(OwnerUserId))).GetValueOrDefault(-1);
        Title = (string)xe.Attribute(nameof(Title));
        Tags = ((string)xe.Attribute(nameof(Tags)) ?? string.Empty).TrimStart('<').TrimEnd('>').Split(new[] { "><" }, StringSplitOptions.RemoveEmptyEntries);
        AnswerCount = ((int?)xe.Attribute(nameof(AnswerCount))).GetValueOrDefault();
        CommentCount = ((int?)xe.Attribute(nameof(CommentCount))).GetValueOrDefault();
        FavoriteCount = ((int?)xe.Attribute(nameof(FavoriteCount))).GetValueOrDefault();
    }

    public int Id { get; }
    public byte PostTypeId { get; }
    public DateTime CreationDate { get; }
    public DateTime LastActivityDate { get; }
    public int Score { get; }
    public int ViewCount { get; }
    public int OwnerUserId { get; }
    public string Title { get; }
    public IReadOnlyCollection<string> Tags { get; }
    public int AnswerCount { get; }
    public int CommentCount { get; }
    public int FavoriteCount { get; }
}