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

    public int Id { get; }
    public int Reputation { get; }
    public DateTime CreationDate { get; }
    public string DisplayName { get; }
    public DateTime LastAccessDate { get; }
    public int Views { get; }
    public int UpVotes { get; }
    public int DownVotes { get; }
}