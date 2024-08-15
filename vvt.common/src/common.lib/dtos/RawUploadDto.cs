namespace vvt.common.lib.dtos;

public class RawUploadDto
{
    public string RawLine { get; set; } = "";

    public RawUploadDto(string rawLine)
    {
        RawLine = rawLine;
    }
}