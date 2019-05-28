using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gases.Data;
using Gases.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Gases.Controllers
{
    public class GDatasController : Controller
    {
        IHostingEnvironment _appEnvironment;
        private readonly ApplicationDbContext _context;

        public GDatasController(IHostingEnvironment appEnvironment, ApplicationDbContext context)
        {
            _appEnvironment = appEnvironment;
            _context = context;
        }

        // GET: GDatas
        public async Task<IActionResult> Index(string SortOrder,
            int? GDataTypeIdFilter,
            int? GaseIdFilter,
            int? RegionIdFilter,
            decimal? VerticalSliceFilter,
            int? YearFilter,
            int? Page)
        {
            //var applicationDbContext = _context.GData.Include(g => g.GDataType).Include(g => g.Gase).Include(g => g.Region);
            //return View(await applicationDbContext.ToListAsync());

            var gData = _context.GData
                .Include(g => g.GDataType)
                .Include(g => g.Gase)
                .Include(g => g.Region)
                .Where(g => true);

            ViewBag.GDataTypeIdFilter = GDataTypeIdFilter;
            ViewBag.GaseIdFilter = GaseIdFilter;
            ViewBag.RegionIdFilter = RegionIdFilter;
            ViewBag.VerticalSliceFilter = VerticalSliceFilter;
            ViewBag.YearFilter = YearFilter;

            ViewBag.VerticalSliceSort = SortOrder == "VerticalSlice" ? "VerticalSliceDesc" : "VerticalSlice";
            ViewBag.YearSort = SortOrder == "Year" ? "YearDesc" : "Year";

            if (GDataTypeIdFilter != null)
            {
                gData = gData.Where(c => c.GDataTypeId == GDataTypeIdFilter);
            }
            if (GaseIdFilter != null)
            {
                gData = gData.Where(c => c.GaseId == GaseIdFilter);
            }
            if (RegionIdFilter != null)
            {
                gData = gData.Where(c => c.RegionId == RegionIdFilter);
            }
            if (VerticalSliceFilter != null)
            {
                gData = gData.Where(c => c.VerticalSlice == VerticalSliceFilter);
            }
            if (YearFilter != null)
            {
                gData = gData.Where(c => c.Year == YearFilter);
            }

            switch (SortOrder)
            {
                case "VerticalSlice":
                    gData = gData.OrderBy(c => c.VerticalSlice);
                    break;
                case "VerticalSliceDesc":
                    gData = gData.OrderByDescending(c => c.VerticalSlice);
                    break;
                case "Year":
                    gData = gData.OrderBy(c => c.Year);
                    break;
                case "YearDesc":
                    gData = gData.OrderByDescending(c => c.Year);
                    break;
                default:
                    gData = gData.OrderBy(c => c.Id);
                    break;
            }

            ViewBag.SortOrder = SortOrder;

            var pager = new Pager(gData.Count(), Page);

            var viewModel = new GDataIndexPageViewModel
            {
                Items = gData.Skip((pager.CurrentPage - 1) * pager.PageSize).Take(pager.PageSize),
                Pager = pager
            };

            ViewBag.GDataType = new SelectList(_context.GDataType.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.Gase = new SelectList(_context.Gase.OrderBy(c => c.Name), "Id", "Name");
            ViewBag.Region = new SelectList(_context.Region.OrderBy(c => c.Name), "Id", "Name");

            return View(viewModel);
        }

        // GET: GDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gData = await _context.GData
                .Include(g => g.GDataType)
                .Include(g => g.Gase)
                .Include(g => g.Region)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gData == null)
            {
                return NotFound();
            }

            return View(gData);
        }

        // GET: GDatas/Create
        //public IActionResult Create()
        //{
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Id");
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id");
        //    ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Id");
        //    return View();
        //}

        // POST: GDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,GDataTypeId,GaseId,VerticalSlice,RegionId,Longtitude,Latitude,Value,Year,Month,Season")] GData gData)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(gData);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Id", gData.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", gData.GaseId);
        //    ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Id", gData.RegionId);
        //    return View(gData);
        //}

        // GET: GDatas/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var gData = await _context.GData.SingleOrDefaultAsync(m => m.Id == id);
        //    if (gData == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Id", gData.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", gData.GaseId);
        //    ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Id", gData.RegionId);
        //    return View(gData);
        //}

        // POST: GDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,GDataTypeId,GaseId,VerticalSlice,RegionId,Longtitude,Latitude,Value,Year,Month,Season")] GData gData)
        //{
        //    if (id != gData.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(gData);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!GDataExists(gData.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Id", gData.GDataTypeId);
        //    ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Id", gData.GaseId);
        //    ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Id", gData.RegionId);
        //    return View(gData);
        //}

        // GET: GDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gData = await _context.GData
                .Include(g => g.GDataType)
                .Include(g => g.Gase)
                .Include(g => g.Region)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (gData == null)
            {
                return NotFound();
            }

            return View(gData);
        }

        // POST: GDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gData = await _context.GData.SingleOrDefaultAsync(m => m.Id == id);
            _context.GData.Remove(gData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GDataExists(int id)
        {
            return _context.GData.Any(e => e.Id == id);
        }

        public void GData(int GDataTypeId,
            int GaseId,
            decimal VerticalSlice,
            int? RegionId,
            decimal? Longtitude,
            decimal? Latitude,
            decimal? Value,
            int Year,
            int? Month,
            Season? Season)
        {
            GData gData = new GData
            {
                GDataTypeId = GDataTypeId,
                GaseId = GaseId,
                VerticalSlice = VerticalSlice,
                RegionId = RegionId,
                Longtitude = Longtitude,
                Latitude = Latitude,
                Value = Value,
                Year = Year,
                Month = Month,
                Season = Season
            };

            _context.GData.Add(gData);
            _context.SaveChanges();
        }

        // GET: GDatas/Upload
        public IActionResult Upload()
        {
            ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name");
            ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name");
            ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Name");
            return View();
        }

        // POST: GDatas/Upload
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int GDataTypeId, int GaseId, decimal VerticalSlice, int RegionId, int Year, IFormFile uploadedFile)
        {
            if (uploadedFile != null)
            {
                // путь к папке Uploaded
                string path = Path.Combine(_appEnvironment.WebRootPath, "Uploaded", Path.GetFileName(uploadedFile.FileName.Replace(',', '.')));
                // сохраняем файл в папку Uploaded в каталоге wwwroot
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                if (Path.GetExtension(path) == ".nc")
                {
                    string folder = Path.Combine(_appEnvironment.WebRootPath, "Uploaded");
                    string batfile = Path.Combine(folder, "bat.bat");
                    string filename = Path.GetFileNameWithoutExtension(uploadedFile.FileName);
                    filename = filename.Replace(',', '.');

                    using (var sw = new StreamWriter(batfile))
                    {
                        sw.WriteLine("ncdump -f c " + filename + ".nc > " + filename + ".txt");
                    }

                    Process process = new Process();
                    try
                    {
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.RedirectStandardError = true;


                        process.StartInfo.WorkingDirectory = folder;
                        process.StartInfo.FileName = batfile;
                        process.Start();

                        string output = "",
                            error = "";
                        while (!process.StandardOutput.EndOfStream)
                        {
                            output += process.StandardOutput.ReadLine();
                        }
                        while (!process.StandardError.EndOfStream)
                        {
                            error += process.StandardError.ReadLine();
                        }

                        process.WaitForExit();
                        System.IO.File.Delete(batfile);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(exception.ToString(), exception.InnerException);
                    }

                    //Получение необходимой информации из созданного файла *.txt
                    path = Path.Combine(_appEnvironment.WebRootPath, "Uploaded", filename + ".txt");
                    using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                    {
                        int indexOfSubstring;
                        string subString;
                        string line;
                        if (GDataTypeId == 2)
                        {
                            List<decimal> value = new List<decimal>();
                            List<decimal> lon = new List<decimal>();
                            List<decimal> lat = new List<decimal>();
                            while ((line = sr.ReadLine()) != "data:")
                            {
                            }
                            line = sr.ReadLine() + Environment.NewLine;
                            line = sr.ReadLine();
                            List<int> x = new List<int>();
                            List<int> y = new List<int>();
                            while ((line = sr.ReadLine().Trim(' ')) != "")
                            {
                                subString = "_";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring > 10)
                                {
                                    line = line.Replace(" ", "").Replace("\t", "");
                                    indexOfSubstring = line.IndexOf("(");
                                    subString = line.Remove(0, line.IndexOf("(") + 1);

                                    x.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(","), subString.Length - subString.IndexOf(","))));
                                    y.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(")"), subString.Length - subString.IndexOf(")")).Remove(0, subString.IndexOf(",") + 1)));

                                    string val;
                                    if (line.IndexOf(';') != -1)
                                    {
                                        val = line.Remove(line.IndexOf(';'));
                                    }
                                    else
                                    {
                                        val = line.Remove(line.IndexOf(','));
                                    }

                                    try
                                    {
                                        value.Add(Decimal.Parse(val, CultureInfo.InvariantCulture));
                                    }
                                    catch
                                    {
                                        value.Add(Decimal.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture));
                                    }
                                }
                            }

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.IndexOf("lat =") != -1 || line.IndexOf("lon =") != -1)
                                {
                                    do
                                    {
                                        string numb = null;
                                        if (line.IndexOf(",") != -1)
                                        {
                                            line = line.Remove(line.IndexOf(","), line.Length - line.IndexOf(","));
                                        }
                                        else
                                        {
                                            line = line.Remove(line.IndexOf(";"), line.Length - line.IndexOf(";"));
                                        }
                                        foreach (char s in line)
                                        {
                                            if (char.IsDigit(s) || (s == '.'))
                                            {
                                                numb = numb + s;
                                            }
                                        }
                                        lat.Add(Decimal.Parse(numb, CultureInfo.InvariantCulture));
                                    } while ((line = sr.ReadLine().Trim(' ')) != "");
                                    break;
                                }
                            }

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.IndexOf("lon =") != -1 || line.IndexOf("lat =") != -1)
                                {
                                    do
                                    {
                                        string numb = null;
                                        if (line.IndexOf(",") != -1)
                                        {
                                            line = line.Remove(line.IndexOf(","), line.Length - line.IndexOf(","));
                                        }
                                        else
                                        {
                                            line = line.Remove(line.IndexOf(";"), line.Length - line.IndexOf(";"));
                                        }
                                        foreach (char s in line)
                                        {
                                            if (char.IsDigit(s) || char.IsPunctuation(s))
                                            {
                                                numb = numb + s;
                                            }
                                        }
                                        lon.Add(Decimal.Parse(numb, CultureInfo.InvariantCulture));
                                    } while ((line = sr.ReadLine().Trim(' ')) != "");
                                    break;
                                }
                            }
                            List<decimal> latValue = new List<decimal>();
                            List<decimal> lonValue = new List<decimal>();
                            for (int i = 0; i < x.Count; i++)
                            {
                                for (int j = 0; j < lat.Count; j++)
                                {
                                    if (x[i] == j)
                                    {
                                        latValue.Add(lat[j]);
                                    }
                                }
                            }
                            for (int i = 0; i < y.Count; i++)
                            {
                                for (int j = 0; j < lon.Count; j++)
                                {
                                    if (y[i] == j)
                                    {
                                        lonValue.Add(lon[j]);
                                    }
                                }
                            }

                            for (int i = 0; i < value.Count; i++)
                            {
                                GData(GDataTypeId, GaseId, VerticalSlice, null, lonValue[i], latValue[i], value[i], Year, null, null);
                            }
                        }

                        if (GDataTypeId == 3)
                        {
                            List<decimal> value = new List<decimal>();
                            List<decimal> vSlice = new List<decimal>();
                            while ((line = sr.ReadLine()) != "data:")
                            {
                            }
                            line = sr.ReadLine() + Environment.NewLine;
                            //line = sr.ReadLine();
                            List<int> y = new List<int>();
                            while ((line = sr.ReadLine().Trim(' ')) != "")
                            {
                                subString = "_";
                                indexOfSubstring = line.IndexOf(subString);
                                if (indexOfSubstring > 10)
                                {
                                    line = line.Replace(" ", "").Replace("\t", "");
                                    indexOfSubstring = line.IndexOf("(");
                                    subString = line.Remove(0, line.IndexOf("(") + 1);

                                    y.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(")"), subString.Length - subString.IndexOf(")"))));

                                    string val;
                                    if (line.IndexOf(';') != -1)
                                    {
                                        val = line.Remove(line.IndexOf(';'));
                                    }
                                    else
                                    {
                                        val = line.Remove(line.IndexOf(','));
                                    }

                                    try
                                    {
                                        value.Add(Decimal.Parse(val, CultureInfo.InvariantCulture));
                                    }
                                    catch
                                    {
                                        value.Add(Decimal.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture));
                                    }
                                }
                                if (indexOfSubstring == 8)
                                {
                                    line = line.Replace(" ", "").Replace("\t", "");
                                    indexOfSubstring = line.IndexOf("(");
                                    subString = line.Remove(0, line.IndexOf("(") + 1);

                                    y.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(")"), subString.Length - subString.IndexOf(")"))));

                                    string val;
                                    val = line.Remove(line.IndexOf(','));
                                    val = val.Remove(0, line.IndexOf("=") + 1);

                                    try
                                    {
                                        value.Add(Decimal.Parse(val, CultureInfo.InvariantCulture));
                                    }
                                    catch
                                    {
                                        value.Add(Decimal.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture));
                                    }
                                }
                            }

                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.IndexOf("TempPrsLvls_A =") != -1)
                                {
                                    do
                                    {
                                        if (String.Compare(line, "}") == 0)
                                        {
                                            break;
                                        }
                                        string numb = null;
                                        if (line.IndexOf(",") != -1)
                                        {
                                            line = line.Remove(line.IndexOf(","), line.Length - line.IndexOf(","));
                                        }
                                        else
                                        {
                                            line = line.Remove(line.IndexOf(";"), line.Length - line.IndexOf(";"));
                                        }
                                        foreach (char s in line)
                                        {
                                            if (char.IsDigit(s) || (s == '.'))
                                            {
                                                numb = numb + s;
                                            }
                                        }
                                        vSlice.Add(Decimal.Parse(numb, CultureInfo.InvariantCulture));
                                    } while ((line = sr.ReadLine().Trim(' ')) != "");
                                    break;
                                }
                            }

                            List<decimal> vSliceValue = new List<decimal>();
                            for (int i = 0; i < y.Count; i++)
                            {
                                for (int j = 0; j < vSlice.Count; j++)
                                {
                                    if (y[i] == j)
                                    {
                                        vSliceValue.Add(vSlice[j]);
                                    }
                                }
                            }

                            for (int i = 0; i < value.Count; i++)
                            {
                                GData(GDataTypeId, GaseId, vSliceValue[i], RegionId, null, null, value[i], Year, null, null);
                            }
                        }

                        if (GDataTypeId == 5)
                        {
                            var lines = System.IO.File.ReadAllLines(path, Encoding.Default);
                            string months = "";
                            Season season = new Season();
                            List<int> year = new List<int>();
                            List<decimal> value = new List<decimal>();
                            subString = "";

                            subString = lines[0].Remove(0, lines[0].IndexOf("SEASON_") + 7);
                            months = subString.Remove(subString.IndexOf('.'));
                            if (String.Compare(months, "DJF") == 0)
                            {
                                season = Season.Winter;
                            }
                            if (String.Compare(months, "MAM") == 0)
                            {
                                season = Season.Spring;
                            }
                            if (String.Compare(months, "JJA") == 0)
                            {
                                season = Season.Summer;
                            }
                            if (String.Compare(months, "SON") == 0)
                            {
                                season = Season.Autumn;
                            }

                            for (int i = 70; i < lines.Length; i++)
                            {
                                if (String.Compare(lines[i].Replace(" ", "").Replace("\t", ""), "") == 0)
                                {
                                    break;
                                }

                                if (lines[i].IndexOf("datayear =") != -1)
                                {
                                    subString = lines[i].Remove(0, lines[i].IndexOf("=") + 2);
                                }
                                else
                                {
                                    subString = lines[i].Replace(" ", "").Replace("\t", "");
                                }

                                if (lines[i].IndexOf(';') != -1)
                                {
                                    year.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(';'))));
                                }
                                else
                                {
                                    year.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(','))));
                                }
                            }
                            int equally = 0;
                            for (int i = 70; i < lines.Length; i++)
                            {
                                if (lines[i].IndexOf("=") != -1)
                                {
                                    equally++;
                                    if (equally == 3)
                                    {
                                        for (int j = i+1; j < lines.Length; j++)
                                        {
                                            if (String.Compare(lines[j].Replace(" ", "").Replace("\t", ""), "") == 0)
                                            {
                                                break;
                                            }
                                            subString = lines[j].Replace(" ", "").Replace("\t", "");
                                            if (lines[j].IndexOf(';') != -1)
                                            {
                                                value.Add(Decimal.Parse(subString.Remove(subString.IndexOf(';')), CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                value.Add(Decimal.Parse(subString.Remove(subString.IndexOf(',')), CultureInfo.InvariantCulture));
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < value.Count; i++)
                            {
                                GData(GDataTypeId, GaseId, VerticalSlice, null, null, null, value[i], year[i], null, season);
                            }
                        }

                        if (GDataTypeId == 4)
                        {
                            var lines = System.IO.File.ReadAllLines(path, Encoding.Default);
                            int month = 0;
                            List<int> year = new List<int>();
                            List<decimal> value = new List<decimal>();
                            subString = "";

                            subString = lines[0].Remove(0, lines[0].IndexOf("MONTH_") + 6);
                            month = Convert.ToInt32(subString.Remove(subString.IndexOf('.')));

                            for (int i = 68; i < lines.Length; i++)
                            {
                                if (String.Compare(lines[i].Replace(" ", "").Replace("\t", ""), "") == 0)
                                {
                                    break;
                                }

                                if (lines[i].IndexOf("datayear =") != -1)
                                {
                                    subString = lines[i].Remove(0, lines[i].IndexOf("=") + 2);
                                }
                                else
                                {
                                    subString = lines[i].Replace(" ", "").Replace("\t", "");
                                }

                                if (lines[i].IndexOf(';') != -1)
                                {
                                    year.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(';'))));
                                }
                                else
                                {
                                    year.Add(Convert.ToInt32(subString.Remove(subString.IndexOf(','))));
                                }
                            }
                            int equally = 0;
                            for (int i = 68; i < lines.Length; i++)
                            {
                                if (lines[i].IndexOf("=") != -1)
                                {
                                    equally++;
                                    if (equally == 2)
                                    {
                                        for (int j = i + 1; j < lines.Length; j++)
                                        {
                                            if (String.Compare(lines[j].Replace(" ", "").Replace("\t", ""), "") == 0)
                                            {
                                                break;
                                            }
                                            subString = lines[j].Replace(" ", "").Replace("\t", "");
                                            if (lines[j].IndexOf(';') != -1)
                                            {
                                                value.Add(Decimal.Parse(subString.Remove(subString.IndexOf(';')), CultureInfo.InvariantCulture));
                                            }
                                            else
                                            {
                                                value.Add(Decimal.Parse(subString.Remove(subString.IndexOf(',')), CultureInfo.InvariantCulture));
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < value.Count; i++)
                            {
                                GData(GDataTypeId, GaseId, VerticalSlice, null, null, null, value[i], year[i], month, null);
                            }
                        }
                    }
                }

                if (Path.GetExtension(path) == ".csv")
                {
                    var lines = System.IO.File.ReadAllLines(path, Encoding.Default);

                    List<int> month = new List<int>();
                    List<decimal> value = new List<decimal>();
                    string subString;
                    for (int i = 9; i < lines.Length; i++)
                    {
                        subString = lines[i].Remove(0, lines[i].IndexOf("-") + 1);
                        month.Add(Convert.ToInt32(subString.Remove(subString.IndexOf("-"), subString.Length - subString.IndexOf("-"))));
                        value.Add(Decimal.Parse(lines[i].Remove(0, lines[i].IndexOf(",") + 1), CultureInfo.InvariantCulture));
                    }

                    for (int i = 0; i < value.Count; i++)
                    {
                        GData(GDataTypeId, GaseId, VerticalSlice, RegionId, null, null, value[i], Year, month[i], null);
                    }
                }
            }
            ViewData["GDataTypeId"] = new SelectList(_context.GDataType, "Id", "Name", GDataTypeId);
            ViewData["GaseId"] = new SelectList(_context.Gase, "Id", "Name", GaseId);
            ViewData["RegionId"] = new SelectList(_context.Region, "Id", "Name", RegionId);
            //return View();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> View(int? GDataTypeId, int? GaseId, decimal? VerticalSlice, int? RegionId, int? Year)
        {
            return View();
        }
    }
}
