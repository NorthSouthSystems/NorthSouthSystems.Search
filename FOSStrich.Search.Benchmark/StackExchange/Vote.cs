namespace FOSStrich.Search;

using MemoryPack;
using System.Xml.Linq;

[MemoryPackable]
public partial class Vote
{
    [MemoryPackConstructor]
    private Vote() { }

    internal Vote(XElement xe)
    {
        Id = (int)xe.Attribute(nameof(Id));
        PostId = (int)xe.Attribute(nameof(PostId));
        VoteTypeId = (byte)(uint)xe.Attribute(nameof(VoteTypeId));
        CreationDate = (DateTime)xe.Attribute(nameof(CreationDate));
        UserId = (int?)xe.Attribute(nameof(UserId));
        BountyAmount = (int?)xe.Attribute(nameof(BountyAmount));
    }

    public int Id { get; init; }
    public int PostId { get; init; }
    public byte VoteTypeId { get; init; }
    public DateTime CreationDate { get; init; }
    public int? UserId { get; init; }
    public int? BountyAmount { get; init; }
}