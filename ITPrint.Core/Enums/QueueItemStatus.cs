namespace ITPrint.Core.Enums;

public enum QueueItemStatus
{
    Queued = 0,           
    Printing = 1,         
    Completed = 2,        
    Failed = 3,           
    Cancelled = 4,        
    Retry = 5             
}