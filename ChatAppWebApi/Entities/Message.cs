namespace ChatAppWebApi.Entities;

public class MessageEntity
{
    public Guid Id {get; set;}

    public string User {get; set;}
    
    public string Message {get; set;}

}