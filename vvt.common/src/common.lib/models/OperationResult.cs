using vvt.common.lib.enums;
namespace vvt.common.lib.models;

public class OperationResult
{
    public OperationResults Result { get; set; }
    public string Message {get; set; } = "";

    public OperationResult(OperationResults result, string message)
    {
        Result = result;
        Message = message;
    }
    
    public OperationResult(OperationResults result)
    {
        Result = result;
        Message = string.Empty;
    }
}