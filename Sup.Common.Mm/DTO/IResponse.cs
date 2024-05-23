namespace Sup.Mm.Common.DTO;

public interface IResponse
{
    public bool Success { get; set; }
    
    public int ErrorCode { get; set; }
    
    public string Message { get; set; }
}