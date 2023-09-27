namespace FOSStrich.Search;

using MemoryPack;
using System.Xml.Linq;

[MemoryPackable]
public partial class User
{
    [MemoryPackConstructor]
    private User() { }

    internal User(XElement xe)
    {
        Id = (int)xe.Attribute(nameof(Id));
        Reputation = (int)xe.Attribute(nameof(Reputation));
        CreationDate = (DateTime)xe.Attribute(nameof(CreationDate));
        DisplayName = (string)xe.Attribute(nameof(DisplayName));
        LastAccessDate = (DateTime)xe.Attribute(nameof(LastAccessDate));
        Views = (int)xe.Attribute(nameof(Views));
        UpVotes = (int)xe.Attribute(nameof(UpVotes));
        DownVotes = (int)xe.Attribute(nameof(DownVotes));
    }

    public int Id { get; init; }
    public int Reputation { get; init; }
    public DateTime CreationDate { get; init; }
    public string DisplayName { get; init; }
    public DateTime LastAccessDate { get; init; }
    public int Views { get; init; }
    public int UpVotes { get; init; }
    public int DownVotes { get; init; }
}