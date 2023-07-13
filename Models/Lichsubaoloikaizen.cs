using System;
using System.Collections.Generic;

namespace BaoLoi.Models;

public partial class Lichsubaoloikaizen
{
    public int Id { get; set; }

    public string? Model { get; set; }

    public string? Cell { get; set; }

    public string? Station { get; set; }

    public string? Thoigian { get; set; }
}
