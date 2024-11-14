using System.ComponentModel.DataAnnotations;

namespace Typeracer.Models;

// values of a player can change, so struct is used instead of record
// extension methods can be called to calculate the average WPM, average accuracy, best WPM and performance score
public class Player : IComparable<Player>
{
    [Key]
    public Guid PlayerID { get; private set; }  // changed from readonly for deserialization (private set for unreachable modification)
    
    [Required]
    public string Username { get; set; }
    public List<WPM> WPMs { get; set; } // each game average WPM should be added here
    public List<Accuracy> Accuracies { get; set; } // each game average accuracy should be added here
    
    public Guid? BestWPMID { get; set; }
    public Guid? BestAccuracyID { get; set; }
    
    
    // parameterless constructor for deserialization
    public Player()
    {
        WPMs = new List<WPM>();
        Accuracies = new List<Accuracy>();
    }


    public Player(string username, double initialWPM, double initialAccuracy)
    {
        PlayerID = Guid.NewGuid();
        Username = username;
        WPMs = new List<WPM>();
        Accuracies = new List<Accuracy>();
        AddGameResult(initialWPM, initialAccuracy);
        Console.WriteLine($"Player created: {PlayerID} - {username}");
    }
    
    public void AddGameResult(double wpm, double accuracy)
    {
        WPM newWPM = new WPM
        {
            Value = wpm, PlayerId = PlayerID
        };
        
        Accuracy newAccuracy = new Accuracy
        {
            Value = accuracy, PlayerId = PlayerID
        };
        
        WPMs.Add(newWPM);
        Accuracies.Add(newAccuracy);

        if (BestWPMID == null || wpm > WPMs.Find(w => w.WPMId == BestWPMID)?.Value)
        {
            BestWPMID = newWPM.WPMId;
        }

        if (BestAccuracyID == null || accuracy > Accuracies.Find(a => a.AccuracyId == BestAccuracyID)?.Value)
        {
            BestAccuracyID = newAccuracy.AccuracyId;
        }
        
        
    }

    // comparing players by their best WPM
    public int CompareTo(Player otherPlayer)
    {
        return (WPMs.Find(w => w.WPMId == BestWPMID)?.Value ?? 0)
            .CompareTo(otherPlayer.WPMs.Find(w => w.WPMId == otherPlayer.BestWPMID)?.Value ?? 0);
    }
}


public class WPM
{
    [Key]
    public Guid WPMId { get; set; } = Guid.NewGuid(); // Ensure a new Guid is generated by default
    public Guid PlayerId { get; set; } // Foreign key to link to the Player
    public double Value { get; set; } // Store the WPM value 
    
    
}


public class Accuracy
{
    [Key]
    public Guid AccuracyId { get; set; } = Guid.NewGuid(); // Ensure a new Guid is generated by default
    public Guid PlayerId { get; set; } // Foreign key to link to the Player
    public double Value { get; set; } // Store the WPM value
        
}