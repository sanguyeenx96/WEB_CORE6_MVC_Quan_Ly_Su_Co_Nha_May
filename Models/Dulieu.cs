using System;
using System.Collections.Generic;

namespace BaoLoi.Models;

public partial class Dulieu
{
    public int Id { get; set; }

    public string Model { get; set; } = null!;

    public string Cell { get; set; } = null!;

    public string Station { get; set; } = null!;

    public string Nguoidamnhiem { get; set; } = null!;

    public string Trangthaihientai { get; set; } = null!;

    public string Tenjig { get; set; } = null!;

    public string? Majig { get; set; }

    public string Hientuongloi { get; set; } = null!;

    public string Vandeanhhuong { get; set; } = null!;

    public string Nguyennhan { get; set; } = null!;

    public string Doisach { get; set; } = null!;

    public string? Ghichuthem { get; set; }

    public int Thoigiansuachua { get; set; }

    public string? XacNhanSuaChua { get; set; }

    public DateTime Ngaygio { get; set; }

    public string? Hinhanh { get; set; }

    public string? Quantrong { get; set; }
}
