namespace ITPrint.Core.Enums;

public enum PrintJobStatus
{
    Uploaded = 0,         
    Processing = 1,       
    Splitting = 2,        
    Routing = 3,          
    InQueue = 4,          
    Printing = 5,         
    Completed = 6,        
    PartiallyCompleted = 7,
    Failed = 8,          
    Cancelled = 9        
}