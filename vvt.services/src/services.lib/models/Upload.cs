using vvt.common.lib.models;

namespace vvt.services.lib.models;

public class Upload : IEntity
{
    public int Id { get; set; }

    public Guid RunId { get; set; }

    public IEnumerable<string> RawLines { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Upload(Guid runId, IEnumerable<string> rawLines)
    {
        RunId = runId;
        RawLines = rawLines;
        CreatedAt = DateTimeOffset.Now;
    }
}