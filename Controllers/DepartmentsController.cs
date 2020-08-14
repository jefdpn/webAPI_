using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using webAPI.Models;

namespace webAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly ContosoUniversityContext _context;

        public DepartmentsController(ContosoUniversityContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment()
        {
            return await _context.Department.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Department>> GetDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);

            if (department == null)
            {
                return NotFound();
            }

            return department;
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, Department department)
        {
            if (id != department.DepartmentId)
            {
                return BadRequest();
            }

            //_context.Entry(department).State = EntityState.Modified;

            try
            {
                //await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlInterpolatedAsync(
                $" EXEC dbo.Department_Update {department.InstructorId},{department.Name},{department.Budget},{department.StartDate},{department.InstructorId},{department.RowVersion}");

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Departments
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Department> PostDepartment(Department department)
        {
            //_context.Department.Add(department);
            //_context.SaveChanges();
            //return CreatedAtAction("GetDepartment", new { id = department.DepartmentId }, department);
            
            var IsOk = _context.Department.FromSqlInterpolated(
                $" EXEC dbo.Department_Insert {department.Name},{department.Budget},{department.StartDate},{department.InstructorId}")
                .Select(o => new { o.DepartmentId, o.RowVersion }).AsEnumerable().FirstOrDefault();

            return CreatedAtAction("GetDepartment", new { id = IsOk.DepartmentId }, department);        
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment(int id)
        {
            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            //_context.Department.Remove(department);
            //await _context.SaveChangesAsync();

            _context.Database.ExecuteSqlInterpolated(
               $"EXEC dbo.Department_Delete  {department.DepartmentId} ,{department.RowVersion} ");
            
            return department;
        }

        //public ActionResult<Department> DeleteDepartment(int id)
        //{
        //    var department = _context.Department.Find(id);
        //    if (department == null)
        //    {
        //        return NotFound();
        //    }            
        //    _context.Database.ExecuteSqlInterpolated(
        //        $"EXEC dbo.Department_Delete  {id} ,{department.RowVersion} ");
        //    return department;
        //}



        // GET: api/Courses/1/Counts
        [HttpGet("/{Coursesid:int}/Counts")]
        public async Task<ActionResult<IEnumerable<VwCourseStudentCount>>> vwCourseStudentCount(int Coursesid)
        {
            var VwCourseStudentCount = 
                await _context.VwCourseStudentCount                
                .Select(p => p)
                .Where(p => p.CourseId == Coursesid)
                .ToListAsync();

            if (VwCourseStudentCount == null)
            {
                return NotFound();
            }

            return VwCourseStudentCount;
        }

        // GET: api/Courses/Students/1
        [HttpGet("/Counts/Students/{Coursesid?}")]
        public async Task<ActionResult<IEnumerable<VwCourseStudents>>> VwCourseStudents(int? Coursesid)
        {            
            if (Coursesid == null)
            {
                return await (
                from i in _context.VwCourseStudents
                where i.CourseId == Coursesid
                select i).ToListAsync();
            }
            else
            {
               return await (
               from i in _context.VwCourseStudents               
               select i).ToListAsync();
            }                        
        }


        // GET: api/Courses/Students/1
        [HttpGet("/Counts/Students/{Coursesid?}")]
        public async Task<ActionResult<IEnumerable<VwDepartmentCourseCount>>> VwDepartmentCourseCount(int Coursesid)
        {
            var VwDepartmentCourseCount = await _context.VwDepartmentCourseCount
                .FromSqlInterpolated($"select * from VwDepartmentCourseCount ").ToListAsync();

            return VwDepartmentCourseCount;
        }



        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.DepartmentId == id);
        }
    }
}
