using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;

namespace WebApplication2.Controllers;

[ApiController]
[Route("[controller]")] // /students
public class StudentsController : ControllerBase
{
    public static List<Student> Students = [
        new Student()
        {
            Id = 1,
            FirstName = "John",
            LastName = "Smith",
            Birthday = new DateTime(1980, 01, 01),
        },
        new Student()
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Birthday = new DateTime(1981, 01, 01),
        }
    ];
    
    
    [HttpGet]
    [Route("hello-world")] // => /students/hello-world
    public IActionResult GetHelloWorld()
    {
        return Ok("Hello World");
    }

    [HttpGet] // /students
    public IActionResult GetStudents(string? fName)
    {
        if (fName != null)
        {
            return Ok(Students.Where(e => e.FirstName == fName));
        }
        
        return Ok(Students);
    }

    [HttpGet] // /students/1
    [Route("{id}")]
    public IActionResult GetStudent(int id)
    {
        var student = Students.FirstOrDefault(x => x.Id == id);

        if (student == null)
        {
            return NotFound($"Student with id: {id} not found");
        }
        
        return Ok(student);
    }

    [HttpPost] // /students
    public IActionResult CreateStudent(Student student)
    {
        var nextId = Students.Max(x => x.Id) + 1;
        
        student.Id = nextId;
        Students.Add(student);
        
        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }

    [HttpPut] // /students/1
    [Route("{id}")]
    public IActionResult ReplaceStudent(int id, Student student)
    {
        var studentToUpdate = Students.FirstOrDefault(x => x.Id == id);

        if (studentToUpdate == null)
        {
            return NotFound($"Student with id: {id} not found");
        }
        
        studentToUpdate.FirstName = student.FirstName;
        studentToUpdate.LastName = student.LastName;
        studentToUpdate.Birthday = student.Birthday;

        return NoContent();
    }

    [HttpDelete] // /students/1
    [Route("{id}")]
    public IActionResult DeleteStudent(int id)
    {
        var studentToDelete = Students.FirstOrDefault(x => x.Id == id);

        if (studentToDelete == null)
        {
            return NotFound($"Student with id: {id} not found");
        }
        
        Students.Remove(studentToDelete);

        return NoContent();
    }
    
}