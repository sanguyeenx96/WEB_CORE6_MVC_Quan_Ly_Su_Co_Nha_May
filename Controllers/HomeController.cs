using BaoLoi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Dynamic;
using NuGet.Protocol.Plugins;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.Diagnostics.Metrics;
using static Azure.Core.HttpHeader;
using NuGet.Protocol.Core.Types;
using static System.Collections.Specialized.BitVector32;

namespace BaoLoi.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private BaoloiContext db = new BaoloiContext();

        public async Task<IActionResult> Index()
        {
            //DateTime dateStart = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd")).AddDays(-1);

            DateTime dateStart = DateTime.Now.AddDays(-1);
            var dem = db.Dulieus.Where(s => s.Ngaygio > dateStart).Count();
            TempData["dem"] = dem;
            var demIJP = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "D67" || s.Model == "E28" || s.Model == "F0809" || s.Model == "E63")).Count();
            TempData["demIJP"] = demIJP;
            var list = await db.Layouts.Where(n => n.Status == "NG").ToListAsync();
            int soluongsuco = list.Count;
            TempData["soluongsuco"] = soluongsuco;
            TempData["soluongsuco_jigquantrong"] = db.Dulieus.Where(s => (s.Ngaygio > dateStart && s.Quantrong == "Quan trọng")).Count();

            var listnghn = db.Dulieus
                                .Where(x => x.Ngaygio > dateStart)
                               .Select(p => new GiamSat() { Ten = p.Nguoidamnhiem, Soluongdasua = 1 })
                               .GroupBy(p => new { p.Ten })
                               .Select(g => new GiamSat() { Ten = g.Key.Ten, Soluongdasua = g.Count() })
                               .OrderByDescending(g => g.Soluongdasua)
                               .ToList();
            ViewBag.giamsathn = listnghn;

            var listtong = db.Dulieus
                 .Where(x => x.Ngaygio > dateStart)
                 .ToList();
            int totalNGtong = Convert.ToInt32(listtong.Count().ToString());
            string connectionString = "Data Source=192.168.1.254;User Id=sa;Password=123;Initial Catalog=BaoLoi;Trusted_connection=false;TrustServerCertificate=True";
            string query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio > '" + dateStart + "' AND [Model] IN('D67', 'E28', 'F0809','E63','L1170ITB','L1170DRIVER','L1172ITB','L1172DRIVER','L1172','L1197','L1170','T555-565','T765','LD2B','OTHER (BAL,CSD,PANEL,...)','T527', 'L1231', 'T541-T543') GROUP BY[Model] ORDER BY Soluongsuco DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<string> ten_model = new List<string>();
                List<int> soluong = new List<int>();
                List<double> tilephantram = new List<double>();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                int totalsoluong = 0;
                int i = 0;
                while (i < dataTable.Rows.Count)
                {
                    string ten;
                    int slloi;
                    double percentError;
                    if (i == 0)
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["Model"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)slloi / totalNGtong * 100), 1);
                    }
                    else
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["Model"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)totalsoluong / totalNGtong * 100), 1);
                    }
                    ten_model.Add(ten);
                    soluong.Add(slloi);
                    tilephantram.Add(percentError);
                    i++;
                }
                ViewBag.tenmodel = ten_model;
                ViewBag.soluongsuco = soluong;
                ViewBag.tile = tilephantram;
            }
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Select(int id)
        {
            var model = await db.Layouts.FindAsync(id);
            if (model != null)
            {
                TempData["damodel"] = model.Model;
                TempData["dastation"] = model.Station;

                TempData["model"] = model.Model;
                TempData["cell"] = model.Cell;
                TempData["station"] = model.Station;
                TempData["id"] = id;
                return RedirectToAction("Nhapthongtinsuachua");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Nhapthongtinsuachua()
        {
            string datenmodel = TempData["damodel"].ToString();
            string datenstation = TempData["dastation"].ToString();

            var tenjig = db.Listjigs.Where(x => (x.Model == datenmodel && x.Station == datenstation)).GroupBy(x => x.Jigname).Select(x => x.FirstOrDefault()).ToList()
              .Select(c => new SelectListItem { Value = c.Jigname.ToString(), Text = c.Jigname }).ToList();

            var selectList = new SelectList(new[]
            {
                new { Value = "Đỗ Văn Đạt" , Text = " Đỗ Văn Đạt" },
                new { Value = "Lê Quốc Đạt" , Text = "Lê Quốc Đạt" },
                new { Value = "Đào Thị Dung" , Text = "Đào Thị Dung" },
                new { Value = "Phan" , Text = "Phan" },
                new { Value = "Quân" , Text = "Quân" },
                new { Value = "Hiển" , Text = "Hiển" },
                new { Value = "Ý" , Text = "Ý" },
                new { Value = "Thành" , Text = "Thành" },
                new { Value = "Đỗ Văn Phúc" , Text = "Đỗ Văn Phúc" },
                new { Value = "Lê Văn Phúc" , Text = "Lê Văn Phúc" },
                new { Value = "Dũng" , Text = "Dũng" },
                new { Value = "Quỳnh" , Text = "Quỳnh" },
                new { Value = "Hải" , Text = "Hải" },
                new { Value = "Đại" , Text = "Đại" },
                new { Value = "Trang" , Text = "Trang" },
                new { Value = "Tuyền" , Text = "Tuyền" },
                new { Value = "Toàn" , Text = "Toàn" },
            }, "Value", "Text");
            var selectList2 = new SelectList(new[]
            {
                new  { Value = "Leader" , Text = "Leader" },
                new  { Value = "Staff" , Text = "Staff" },
                new  { Value = "Supporter" , Text = "Supporter" }
            }, "Value", "Text");
            ViewBag.dstenjig = tenjig;

            ViewBag.list_nguoidamnhiem = selectList;
            ViewBag.list_xacnhanvoi = selectList2;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Nhapthongtinsuachua(Dulieu dulieu, int id)
        {
            int so_id = id;
            try
            {
                Dulieu dl = new Dulieu();
                dl.Model = dulieu.Model;
                dl.Cell = dulieu.Cell;
                dl.Station = dulieu.Station;
                dl.Nguoidamnhiem = dulieu.Nguoidamnhiem;
                dl.Trangthaihientai = "OK";
                dl.Tenjig = dulieu.Tenjig;
                dl.Hientuongloi = dulieu.Hientuongloi;
                dl.Vandeanhhuong = dulieu.Vandeanhhuong;
                dl.Nguyennhan = dulieu.Nguyennhan;
                dl.Doisach = dulieu.Doisach;
                dl.Ghichuthem = "...";
                dl.Thoigiansuachua = dulieu.Thoigiansuachua;
                dl.XacNhanSuaChua = dulieu.XacNhanSuaChua;
                dl.Ngaygio = DateTime.Now;
                dl.Hinhanh = dulieu.Hinhanh;
                dl.Quantrong = dulieu.Quantrong;
                db.Dulieus.Add(dl);
                db.SaveChanges();
                var model = await db.Layouts.FindAsync(so_id);
                if (model != null)
                {
                    model.Status = "OK";
                    await db.SaveChangesAsync();
                }
                TempData["OK"] = "1";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["NG"] = "1";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Nhapthongtinsuachua_thucong(string model, string cell, string station)
        {
            var selectList = new SelectList(new[]
           {
                new { Value = "D67" , Text = "D67" },
                new { Value = "E28" , Text = "E28" },
                new { Value = "E63" , Text = "E63" },
                new { Value = "F0809" , Text = "F0809" },
                new { Value = "T555-565" , Text = "T555-565" },
                new { Value = "T765" , Text = "T765" },
                new { Value = "LD2B" , Text = "LD2B" },
                new { Value = "L1170ITB" , Text = "L1170-ITB" },
                new { Value = "L1170DRIVER" , Text = "L1170-DRIVER" },
                new { Value = "L1172ITB" , Text = "L1172-ITB" },
                new { Value = "L1172DRIVER" , Text = "L1172-DRIVER" },
                new { Value = "L1172" , Text = "L1172" },
                new { Value = "L1197" , Text = "L1197" },
                new { Value = "L1170" , Text = "L1170" },
                new { Value = "T527" , Text = "T527" },
                new { Value = "L1231" , Text = "L1231" },
                new { Value = "T541-T543" , Text = "T541-T543" },
                new { Value = "OTHER (BAL,CSD,PANEL,...)" , Text = "OTHER (BAL,CSD,PANEL,...)" }
            }, "Value", "Text");
            var selectList_modelITB = new SelectList(new[]
            {
                new { Value = "D67" , Text = "D67" },
                new { Value = "E28" , Text = "E28" },
                new { Value = "E63" , Text = "E63" },
                new { Value = "F0809" , Text = "F0809" },
                new { Value = "T555-565" , Text = "T555-565" },
                new { Value = "T765" , Text = "T765" },
                new { Value = "LD2B" , Text = "LD2B" },
                new { Value = "L1170ITB" , Text = "L1170-ITB" },
                new { Value = "L1170DRIVER" , Text = "L1170-DRIVER" },
                new { Value = "L1172ITB" , Text = "L1172-ITB" },
                new { Value = "L1172DRIVER" , Text = "L1172-DRIVER" },
                new { Value = "L1172" , Text = "L1172" },
                new { Value = "L1197" , Text = "L1197" },
                new { Value = "L1170" , Text = "L1170" },
                new { Value = "T527" , Text = "T527" },
                new { Value = "L1231" , Text = "L1231" },
                new { Value = "T541-T543" , Text = "T541-T543" },
                new { Value = "OTHER (BAL,CSD,PANEL,...)" , Text = "OTHER (BAL,CSD,PANEL,...)" }
            }, "Value", "Text");
            var selectList_nguoidamnhiem = new SelectList(new[]
            {
                new { Value = "Đỗ Văn Đạt" , Text = "Đỗ Văn Đạt" },
                new { Value = "Lê Quốc Đạt" , Text = "Lê Quốc Đạt" },
                new { Value = "Đào Thị Dung" , Text = "Đào Thị Dung" },
                new { Value = "Phan" , Text = "Phan" },
                new { Value = "Quân" , Text = "Quân" },
                new { Value = "Hiển" , Text = "Hiển" },
                new { Value = "Ý" , Text = "Ý" },
                new { Value = "Thành" , Text = "Thành" },
                new { Value = "Đỗ Văn Phúc" , Text = "Đỗ Văn Phúc" },
                new { Value = "Lê Văn Phúc" , Text = "Lê Văn Phúc" },
                new { Value = "Dũng" , Text = "Dũng" },
                new { Value = "Quỳnh" , Text = "Quỳnh" },
                new { Value = "Hải" , Text = "Hải" },
                new { Value = "Đại" , Text = "Đại" },
                new { Value = "Trang" , Text = "Trang" },
                new { Value = "Tuyền" , Text = "Tuyền" },
                new { Value = "Toàn" , Text = "Toàn" },
                new { Value = "Sang" , Text = "Sang" },
            }, "Value", "Text");
            var selectList_xacnhanvoi = new SelectList(new[]
            {
                new  { Value = "Leader" , Text = "Leader" },
                new  { Value = "Staff" , Text = "Staff" },
                new  { Value = "Supporter" , Text = "Supporter" }
            }, "Value", "Text");
            ViewBag.list_model = selectList;
            ViewBag.selectList_modelITB = selectList_modelITB;
            ViewBag.list_nguoidamnhiem = selectList_nguoidamnhiem;
            ViewBag.list_xacnhanvoi = selectList_xacnhanvoi;

            var dsstation = db.Listjigs.GroupBy(x => x.Station).Select(x => x.FirstOrDefault()).ToList()
                .Select(c => new SelectListItem { Value = c.Station.ToString(), Text = c.Station }).ToList();
            ViewBag.selectList_Station = dsstation;

            //var layout = db.Listjigs.GroupBy(x => x.Station).Select(x => x.FirstOrDefault()).ToList();

            //var danhsachstation = new SelectList(layout.OrderBy(x => x.Id), "Station", "Station");
            //ViewBag.selectList_Station = danhsachstation;

            if (!String.IsNullOrEmpty(model) && !String.IsNullOrEmpty(station))
            {
                return View();
            }
            return View();
        }

        [HttpPost]
        public IActionResult Nhapthongtinsuachua_thucong(Dulieu dulieu)
        {
            try
            {
                Dulieu dl = new Dulieu();
                dl.Model = dulieu.Model;
                dl.Cell = dulieu.Cell;
                dl.Station = dulieu.Station;
                dl.Nguoidamnhiem = dulieu.Nguoidamnhiem;
                dl.Trangthaihientai = "OK";
                dl.Tenjig = dulieu.Tenjig;
                dl.Majig = dulieu.Majig;
                dl.Hientuongloi = dulieu.Hientuongloi;
                dl.Vandeanhhuong = dulieu.Vandeanhhuong;
                dl.Nguyennhan = dulieu.Nguyennhan;
                dl.Doisach = dulieu.Doisach;
                dl.Ghichuthem = "...";
                dl.Thoigiansuachua = dulieu.Thoigiansuachua;
                dl.XacNhanSuaChua = dulieu.XacNhanSuaChua;
                dl.Ngaygio = DateTime.Now;
                dl.Hinhanh = dulieu.Hinhanh;
                dl.Quantrong = dulieu.Quantrong;
                db.Dulieus.Add(dl);
                db.SaveChanges();
                TempData["OK"] = "1";
                return RedirectToAction("Index");
            }
            catch
            {
                TempData["NG"] = "1";
                return RedirectToAction("Index");
            }
        }

        public IActionResult Thongke(string model, string ngaybatdau, string ngayketthuc)
        {
            var selectList = new SelectList(new[]
            {
                new { Value = "ALL" , Text = "ALL MODEL" },
                new { Value = "IJP" , Text = "IJP" },
                new { Value = "LD2" , Text = "LD2" },
                new { Value = "LD2B" , Text = "LD2B" },
                new { Value = "ITB" , Text = "ITB" },
                new { Value = "FIXING" , Text = "FIXING" },
                new { Value = "HONTAI" , Text = "HONTAI" },
                new { Value = "OTHER (BAL,CSD,PANEL,...)" , Text = "OTHER (BAL,CSD,PANEL,...)" }
            }, "Value", "Text");
            ViewBag.list_model = selectList;
            DateTime dateStart = DateTime.Now.AddDays(-1);
            ViewBag.quantronghomnay = db.Dulieus.Where(x => (x.Ngaygio > dateStart && x.Quantrong == "Quan trọng")).ToList();
            TempData["soluongquantronghomnay"] = db.Dulieus.Where(x => (x.Ngaygio > dateStart && x.Quantrong == "Quan trọng")).Count();
            var listtong = db.Dulieus
                             .Where(x => x.Ngaygio > dateStart)
                             .ToList();
            int totalNGtong = Convert.ToInt32(listtong.Count().ToString());
            string connectionString = "Data Source=192.168.1.254;User Id=sa;Password=123;Initial Catalog=BaoLoi;Trusted_connection=false;TrustServerCertificate=True";
            string query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio > '" + dateStart + "' AND [Model] IN('D67', 'E28', 'F0809','E63','L1170ITB','L1170DRIVER','L1172ITB','L1172DRIVER','L1172','L1197','L1170','T555-565','T765','LD2B','OTHER (BAL,CSD,PANEL,...)','T527', 'L1231', 'T541-T543') GROUP BY[Model] ORDER BY Soluongsuco DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<string> ten_model = new List<string>();
                List<int> soluong = new List<int>();
                List<double> tilephantram = new List<double>();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                int totalsoluong = 0;
                int i = 0;
                while (i < dataTable.Rows.Count)
                {
                    string ten;
                    int slloi;
                    double percentError;
                    if (i == 0)
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["Model"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)slloi / totalNGtong * 100), 1);
                    }
                    else
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["Model"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)totalsoluong / totalNGtong * 100), 1);
                    }
                    ten_model.Add(ten);
                    soluong.Add(slloi);
                    tilephantram.Add(percentError);
                    i++;
                }
                ViewBag.tenmodel = ten_model;
                ViewBag.soluongsuco = soluong;
                ViewBag.tile = tilephantram;
            }

            string query_theotenjig = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio > '" + dateStart + "' AND [Model] IN('D67', 'E28', 'F0809','E63','L1170ITB','L1170DRIVER','L1172ITB','L1172DRIVER','L1172','L1197','L1170','T555-565','T765','LD2B','OTHER (BAL,CSD,PANEL,...)','T527', 'L1231', 'T541-T543') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query_theotenjig, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                List<string> ten_jig = new List<string>();
                List<int> soluong = new List<int>();
                List<double> tilephantram = new List<double>();
                DataTable dataTable = new DataTable();
                dataTable.Load(reader);
                int totalsoluong = 0;
                int i = 0;
                while (i < dataTable.Rows.Count)
                {
                    string ten;
                    int slloi;
                    double percentError;
                    if (i == 0)
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["tenjig"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)slloi / totalNGtong * 100), 1);
                    }
                    else
                    {
                        DataRow row = dataTable.Rows[i];
                        ten = row["tenjig"].ToString();
                        slloi = Convert.ToInt32(row["Soluongsuco"]);
                        totalsoluong += slloi;
                        percentError = Math.Round(((double)totalsoluong / totalNGtong * 100), 1);
                    }
                    ten_jig.Add(ten);
                    soluong.Add(slloi);
                    tilephantram.Add(percentError);
                    i++;
                }
                ViewBag.bd_tenjig = ten_jig;
                ViewBag.bd_soluongsuco = soluong;
                ViewBag.tilept = tilephantram;
            }

            var dem = db.Dulieus.Where(s => s.Ngaygio > dateStart).Count();
            TempData["dem"] = dem;
            var demIJP = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "D67" || s.Model == "E28" || s.Model == "F0809" || s.Model == "E63")).Count();
            TempData["demIJP"] = demIJP;
            var demITB = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "L1170ITB" || s.Model == "L1170DRIVER" || s.Model == "L1172ITB" || s.Model == "L1172DRIVER")).Count();
            TempData["demITB"] = demITB;
            var demFIXING = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "L1172" || s.Model == "L1197" || s.Model == "L1170")).Count();
            TempData["demFIXING"] = demFIXING;
            var demLD2 = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "T555-565" || s.Model == "T765")).Count();
            TempData["demLD2"] = demLD2;
            var demLD2B = db.Dulieus.Where(s => s.Ngaygio > dateStart && s.Model == "LD2B").Count();
            TempData["demLD2B"] = demLD2B;
            var demHONTAI = db.Dulieus.Where(s => s.Ngaygio > dateStart && (s.Model == "T527" || s.Model == "L1231" || s.Model == "T541-T543")).Count();
            TempData["demHONTAI"] = demHONTAI;
            //After clicked search button:
            if (!String.IsNullOrEmpty(model) && !String.IsNullOrEmpty(ngaybatdau) && !String.IsNullOrEmpty(ngayketthuc)) // kiểm tra chuỗi tìm kiếm có rỗng/null hay không
            {
                DateTime start = DateTime.Parse(ngaybatdau);
                DateTime end = DateTime.Parse(ngayketthuc);
                if (model == "IJP")
                {
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && (x.Model == "D67" || x.Model == "E28" || x.Model == "F0809" || x.Model == "E63"))
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    string querytenjig = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio > '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('D67', 'E28', 'F0809', 'E63') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('D67', 'E28', 'F0809', 'E63') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('D67', 'E28', 'F0809', 'E63') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }

                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;

                    return View(list);
                }
                if (model == "ITB")
                {
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && (x.Model == "L1170ITB" || x.Model == "L1170DRIVER" || x.Model == "L1172ITB" || x.Model == "L1172DRIVER"))
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('L1170ITB', 'L1170DRIVER', 'L1172ITB', 'L1172DRIVER') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('L1170ITB', 'L1170DRIVER', 'L1172ITB', 'L1172DRIVER') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "FIXING")
                {
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && (x.Model == "L1172" || x.Model == "L1197" || x.Model == "L1170"))
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('L1172', 'L1197', 'L1170') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('L1172', 'L1197', 'L1170') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "LD2")
                {
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && (x.Model == "T555-565" || x.Model == "T765"))
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('T555-565', 'T765') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('T555-565', 'T765') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "LD2B")
                {
                    //Table:
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && x.Model == "LD2B")
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('LD2B') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('LD2B') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "HONTAI")
                {
                    //Table:
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && (x.Model == "T527" || x.Model == "L1231" || x.Model == "T541-T543"))
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('T527', 'L1231', 'T541-T543') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('T527', 'L1231', 'T541-T543') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "ALL")
                {
                    //Table:
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end).ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('D67', 'E28', 'F0809','E63','L1170ITB','L1170DRIVER','L1172ITB','L1172DRIVER','L1172','L1197','L1170','T555-565','T765','LD2B','OTHER (BAL,CSD,PANEL,...)','T527', 'L1231', 'T541-T543') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('D67', 'E28', 'F0809','E63','L1170ITB','L1170DRIVER','L1172ITB','L1172DRIVER','L1172','L1197','L1170','T555-565','T765','LD2B','OTHER (BAL,CSD,PANEL,...)','T527', 'L1231', 'T541-T543') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
                if (model == "OTHER (BAL,CSD,PANEL,...)")
                {
                    //Table:
                    var list = db.Dulieus
                             .Where(x => x.Ngaygio >= start && x.Ngaygio <= end && x.Model == "OTHER (BAL,CSD,PANEL,...)")
                             .ToList();
                    int totalNG = Convert.ToInt32(list.Count().ToString());
                    TempData["total_ng"] = totalNG;
                    ViewBag.total = totalNG;
                    //Chart:
                    query = "SELECT[Model], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('OTHER (BAL,CSD,PANEL,...)') GROUP BY[Model] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> ten_model = new List<string>();
                        List<int> soluong = new List<int>();
                        List<double> tilephantram = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["Model"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            ten_model.Add(ten);
                            soluong.Add(slloi);
                            tilephantram.Add(percentError);
                            i++;
                        }
                        ViewBag.tenmodel = ten_model;
                        ViewBag.soluongsuco = soluong;
                        ViewBag.tile = tilephantram;
                    }
                    query = "SELECT[tenjig], COUNT(*) AS Soluongsuco FROM dbo.Dulieu WHERE Ngaygio >= '" + start + "' AND Ngaygio <= '" + end + "' AND [Model] IN('OTHER (BAL,CSD,PANEL,...)') GROUP BY[tenjig] ORDER BY Soluongsuco DESC";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<string> jigname = new List<string>();
                        List<int> countnumber = new List<int>();
                        List<double> percent = new List<double>();
                        DataTable dataTable = new DataTable();
                        dataTable.Load(reader);
                        int totalsoluong = 0;
                        int i = 0;
                        while (i < dataTable.Rows.Count)
                        {
                            string ten;
                            int slloi;
                            double percentError;
                            if (i == 0)
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)slloi / totalNG * 100), 1);
                            }
                            else
                            {
                                DataRow row = dataTable.Rows[i];
                                ten = row["tenjig"].ToString();
                                slloi = Convert.ToInt32(row["Soluongsuco"]);
                                totalsoluong += slloi;
                                percentError = Math.Round(((double)totalsoluong / totalNG * 100), 1);
                            }
                            jigname.Add(ten);
                            countnumber.Add(slloi);
                            percent.Add(percentError);
                            i++;
                        }
                        ViewBag.bdj_tenjig = jigname;
                        ViewBag.bdj_soluongsuco = countnumber;
                        ViewBag.tileptj = percent;
                    }
                    TempData["model"] = model;
                    TempData["ngaybatdau"] = ngaybatdau;
                    TempData["ngayketthuc"] = ngayketthuc;
                    return View(list);
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetStates(string model)
        {
            var tenstation = db.Listjigs.Where(x => x.Model == model).GroupBy(x => x.Station).Select(x => x.FirstOrDefault()).ToList()
                .Select(c => new SelectListItem { Value = c.Station, Text = c.Station }).ToList();
            return Json(tenstation);
        }

        public IActionResult GetJigname(string model, string station)
        {
            try
            {
                var tenjig = db.Listjigs.Where(x => (x.Model == model && x.Station == station)).GroupBy(x => x.Jigname).Select(x => x.FirstOrDefault()).ToList()
                .Select(c => new SelectListItem { Value = c.Jigname.ToString(), Text = c.Jigname }).ToList();
                return Json(tenjig);
            }
            catch
            {
                return Json(null);
            }
        }

        public IActionResult GetJigno(string model, string station, string tenjig)
        {
            try
            {
                var majig = db.Listjigs.Where(x => (x.Model == model && x.Station == station && x.Jigname == tenjig)).ToList()
                .Select(c => new SelectListItem { Value = c.Jigno.ToString(), Text = c.Jigno }).ToList();
                return Json(majig);
            }
            catch
            {
                return Json(null);
            }
        }

        public IActionResult GetQuantrong(string model, string station, string tenjig)
        {
            try
            {
                var quantrongko = db.Listjigs.Where(x => (x.Model == model && x.Station == station && x.Jigname == tenjig)).ToList();
                var giatri = quantrongko[0].Quantrong.ToString();
                return Json(giatri);
            }
            catch (Exception)
            {
                return Json(null);
            }
        }
    }
}