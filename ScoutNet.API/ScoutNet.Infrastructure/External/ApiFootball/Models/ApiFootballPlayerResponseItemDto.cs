using System.Text.Json.Serialization;

namespace ScoutNet.Infrastructure.External.ApiFootball.Models;

public class ApiFootballPlayerResponseItemDto
{
    [JsonPropertyName("player")]
    public ApiFootballPlayerProfileDto Player { get; set; } = new();

    [JsonPropertyName("statistics")]
    public List<ApiFootballPlayerStatisticsDto> Statistics { get; set; } = [];
}

public class ApiFootballPlayerProfileDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("firstname")]
    public string? Firstname { get; set; }

    [JsonPropertyName("lastname")]
    public string? Lastname { get; set; }

    [JsonPropertyName("age")]
    public int? Age { get; set; }

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }

    [JsonPropertyName("height")]
    public string? Height { get; set; }

    [JsonPropertyName("weight")]
    public string? Weight { get; set; }

    [JsonPropertyName("injured")]
    public bool Injured { get; set; }

    [JsonPropertyName("photo")]
    public string? Photo { get; set; }

    [JsonPropertyName("birth")]
    public ApiFootballBirthDto? Birth { get; set; }
}

public class ApiFootballBirthDto
{
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("place")]
    public string? Place { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }
}

public class ApiFootballPlayerStatisticsDto
{
    [JsonPropertyName("team")]
    public ApiFootballTeamDto Team { get; set; } = new();

    [JsonPropertyName("league")]
    public ApiFootballLeagueDto League { get; set; } = new();

    [JsonPropertyName("games")]
    public ApiFootballGamesDto Games { get; set; } = new();

    [JsonPropertyName("substitutes")]
    public ApiFootballSubstitutesDto? Substitutes { get; set; }

    [JsonPropertyName("shots")]
    public ApiFootballShotsDto? Shots { get; set; }

    [JsonPropertyName("goals")]
    public ApiFootballGoalsDto? Goals { get; set; }

    [JsonPropertyName("passes")]
    public ApiFootballPassesDto? Passes { get; set; }

    [JsonPropertyName("tackles")]
    public ApiFootballTacklesDto? Tackles { get; set; }

    [JsonPropertyName("duels")]
    public ApiFootballDuelsDto? Duels { get; set; }

    [JsonPropertyName("dribbles")]
    public ApiFootballDribblesDto? Dribbles { get; set; }

    [JsonPropertyName("fouls")]
    public ApiFootballFoulsDto? Fouls { get; set; }

    [JsonPropertyName("cards")]
    public ApiFootballCardsDto? Cards { get; set; }

    [JsonPropertyName("penalty")]
    public ApiFootballPenaltyDto? Penalty { get; set; }
}

public class ApiFootballTeamDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }
}

public class ApiFootballLeagueDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("logo")]
    public string? Logo { get; set; }

    [JsonPropertyName("flag")]
    public string? Flag { get; set; }

    [JsonPropertyName("season")]
    public int Season { get; set; }
}

public class ApiFootballGamesDto
{
    [JsonPropertyName("appearences")]
    public int? Appearences { get; set; }

    [JsonPropertyName("lineups")]
    public int? Lineups { get; set; }

    [JsonPropertyName("minutes")]
    public int? Minutes { get; set; }

    [JsonPropertyName("number")]
    public int? Number { get; set; }

    [JsonPropertyName("position")]
    public string? Position { get; set; }

    [JsonPropertyName("rating")]
    public string? Rating { get; set; }

    [JsonPropertyName("captain")]
    public bool Captain { get; set; }
}

public class ApiFootballSubstitutesDto
{
    [JsonPropertyName("in")]
    public int? In { get; set; }

    [JsonPropertyName("out")]
    public int? Out { get; set; }

    [JsonPropertyName("bench")]
    public int? Bench { get; set; }
}

public class ApiFootballShotsDto
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("on")]
    public int? On { get; set; }
}

public class ApiFootballGoalsDto
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("conceded")]
    public int? Conceded { get; set; }

    [JsonPropertyName("assists")]
    public int? Assists { get; set; }

    [JsonPropertyName("saves")]
    public int? Saves { get; set; }
}

public class ApiFootballPassesDto
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("key")]
    public int? Key { get; set; }

    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; set; }
}

public class ApiFootballTacklesDto
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("blocks")]
    public int? Blocks { get; set; }

    [JsonPropertyName("interceptions")]
    public int? Interceptions { get; set; }
}

public class ApiFootballDuelsDto
{
    [JsonPropertyName("total")]
    public int? Total { get; set; }

    [JsonPropertyName("won")]
    public int? Won { get; set; }
}

public class ApiFootballDribblesDto
{
    [JsonPropertyName("attempts")]
    public int? Attempts { get; set; }

    [JsonPropertyName("success")]
    public int? Success { get; set; }

    [JsonPropertyName("past")]
    public int? Past { get; set; }
}

public class ApiFootballFoulsDto
{
    [JsonPropertyName("drawn")]
    public int? Drawn { get; set; }

    [JsonPropertyName("committed")]
    public int? Committed { get; set; }
}

public class ApiFootballCardsDto
{
    [JsonPropertyName("yellow")]
    public int? Yellow { get; set; }

    [JsonPropertyName("red")]
    public int? Red { get; set; }
}

public class ApiFootballPenaltyDto
{
    [JsonPropertyName("won")]
    public int? Won { get; set; }

    [JsonPropertyName("commited")]
    public int? Commited { get; set; }

    [JsonPropertyName("scored")]
    public int? Scored { get; set; }

    [JsonPropertyName("missed")]
    public int? Missed { get; set; }

    [JsonPropertyName("saved")]
    public int? Saved { get; set; }
}
