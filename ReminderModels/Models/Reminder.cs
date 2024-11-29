using System;

namespace ReminderBot.ReminderModels.Models;

public class Reminder 
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public DateTime CreationDateTime { get; set; }
    public DateTime DateTime { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }

    public Reminder(string title, long userId) 
    {
        Title = title;
        UserId = userId;
    }

    public override string ToString() 
    { 
        return Title; 
    }

    public override bool Equals(object? obj)
    {  
        return obj is Reminder reminder && reminder.Id == Id; 
    }

    public override int GetHashCode() 
    { 
        return HashCode.Combine(Id); 
    }
}