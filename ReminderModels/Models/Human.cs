using System;

namespace ReminderBot.ReminderModels.Models;

public class Human 
{
    public long Id { get; set; }
    public long TgId { get; set; }

    public override string ToString() 
    { 
        return Id.ToString(); 
    }

    public override bool Equals(object? obj)
    {  
        return obj is Human human && human.Id == Id; 
    }

    public override int GetHashCode() 
    { 
        return HashCode.Combine(Id); 
    }
}