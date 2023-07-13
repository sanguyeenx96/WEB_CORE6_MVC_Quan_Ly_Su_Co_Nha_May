using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BaoLoi.Models;

namespace BaoLoi.Controllers
{
    public class ListjigsController : Controller
    {
        private readonly BaoloiContext _context;

        public ListjigsController(BaoloiContext context)
        {
            _context = context;
        }

        // GET: Listjigs
        public async Task<IActionResult> Index(string model)
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
            ViewBag.list_model = selectList;
            if (!String.IsNullOrEmpty(model))
            {
                TempData["modelduocchon"] = model;
                TempData["select_model"] = model;
                var listjig = await _context.Listjigs.Where(x => x.Model == model).OrderByDescending(x => x.Id).ToListAsync();
                return View(listjig);
            }
            return View();
        }

        // GET: Listjigs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listjigs == null)
            {
                return NotFound();
            }

            var listjig = await _context.Listjigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listjig == null)
            {
                return NotFound();
            }

            return View(listjig);
        }

        // GET: Listjigs/Create
        public IActionResult Create()
        {
            var selectList_model = new SelectList(new[]
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
            ViewBag.list_model2 = selectList_model;
            var sl = new SelectList(new[]
            {
                new { Value = "Quan trọng" , Text = "Quan trọng" },
                new { Value = "Không quan trọng" , Text = "Không quan trọng" }
            }, "Value", "Text");
            ViewBag.list_quantrong = sl;

            return View();
        }

        [HttpPost]
        public IActionResult Create(Listjig listjig)
        {
            try
            {
                Listjig dl = new Listjig();
                dl.Model = listjig.Model;
                if (!String.IsNullOrEmpty(listjig.Cell))
                {
                    dl.Cell = listjig.Cell.Trim();
                }
                if (!String.IsNullOrEmpty(listjig.Station))
                {
                    dl.Station = listjig.Station.Trim();
                }
                if (!String.IsNullOrEmpty(listjig.Jigname))
                {
                    dl.Jigname = listjig.Jigname.Trim();
                }
                if (!String.IsNullOrEmpty(listjig.Jigno))
                {
                    dl.Jigno = listjig.Jigno.Trim();
                }
                dl.Quantrong = listjig.Quantrong;
                _context.Add(dl);
                _context.SaveChanges();
                TempData["OK"] = "1";
                return RedirectToRoute(new { action = "Index", controller = "Listjigs", model = listjig.Model });
            }
            catch
            {
                TempData["NG"] = 1;
                return RedirectToAction("Create");
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var sl = new SelectList(new[]
           {
                new { Value = "Quan trọng" , Text = "Quan trọng" },
                new { Value = "Không quan trọng" , Text = "Không quan trọng" }
            }, "Value", "Text");
            ViewBag.list_quantrong = sl;
            if (id == null || _context.Listjigs == null)
            {
                return NotFound();
            }

            var listjig = await _context.Listjigs.FindAsync(id);
            if (listjig == null)
            {
                return NotFound();
            }
            return View(listjig);
        }

        // POST: Listjigs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,Cell,Station,Jigname,Jigno,Quantrong")] Listjig listjig)
        {
            if (id != listjig.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listjig);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListjigExists(listjig.Id))
                    {
                        TempData["NG"] = 1;
                        return NotFound();
                    }
                    else
                    {
                        TempData["NG"] = 1;
                        throw;
                    }
                }
                TempData["OK"] = 1;
                return RedirectToRoute(new { action = "Index", controller = "Listjigs", model = TempData["select_model"] });
            }
            return View(listjig);
        }

        // GET: Listjigs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Listjigs == null)
            {
                return NotFound();
            }

            var listjig = await _context.Listjigs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (listjig == null)
            {
                return NotFound();
            }

            return View(listjig);
        }

        // POST: Listjigs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Listjigs == null)
            {
                return Problem("Entity set 'BaoLoiContext.Listjigs'  is null.");
            }
            var listjig = await _context.Listjigs.FindAsync(id);
            if (listjig != null)
            {
                _context.Listjigs.Remove(listjig);
            }

            await _context.SaveChangesAsync();
            TempData["OK"] = 1;
            return RedirectToRoute(new { action = "Index", controller = "Listjigs", model = TempData["select_model"] });
        }

        private bool ListjigExists(int id)
        {
            return (_context.Listjigs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}