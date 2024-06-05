using System.Text.Json.Serialization;

namespace Sup.Common.Models.DTO;

public class ScheduleDto
{
    [JsonPropertyName("sun")]
    public bool WorkingInSunday { get; set; } = false;
    [JsonPropertyName("sun_start")]
    public int SundayStartHour { get; set; } = 0;
    [JsonPropertyName("sun_end")]
    public int SundayEndHour { get; set; } = 0;
    [JsonPropertyName("mon")]
    public bool WorkingInMonday { get; set; } = true;
    [JsonPropertyName("mon_start")]    
    public int MondayStartHour { get; set; } = 6;
    [JsonPropertyName("mon_end")]
    public int MondayEndHour { get; set; } = 17;
    [JsonPropertyName("tue")]
    public bool WorkingInTuesday { get; set; } = true;
    [JsonPropertyName("tue_start")]
    public int TuesdayStartHour { get; set; } = 6;
    [JsonPropertyName("tue_end")]
    public int TuesdayEndHour { get; set; } = 17;
    [JsonPropertyName("wed")]
    public bool WorkingInWednesday { get; set; } = true;
    [JsonPropertyName("wed_start")]
    public int WednesdayStartHour { get; set; } = 6;
    [JsonPropertyName("wed_end")]
    public int WednesdayEndHour { get; set; } = 17;
    [JsonPropertyName("thu")]
    public bool WorkingInThursday { get; set; } = true;
    [JsonPropertyName("thu_start")]
    public int ThursdayStartHour { get; set; } = 6;
    [JsonPropertyName("thu_end")]
    public int ThursdayEndHour { get; set; } = 17;
    [JsonPropertyName("fri")]
    public bool WorkingInFriday { get; set; } = true;
    [JsonPropertyName("fri_start")]
    public int FridayStartHour { get; set; } = 6;
    [JsonPropertyName("fri_end")]
    public int FridayEndHour { get; set; } = 17;
    [JsonPropertyName("sat")]
    public bool WorkingInSaturday { get; set; } = false;
    [JsonPropertyName("sat_start")]
    public int SaturdayStartHour { get; set; } = 0;
    [JsonPropertyName("sat_end")]
    public int SaturdayEndHour { get; set; } = 0;
}