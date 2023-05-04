using Microsoft.AspNetCore.Mvc;
using UPB.CoreLogic.Models;
using UPB.CoreLogic.Managers;

namespace UPB.PracticeThree.Controllers;

[ApiController]
[Route("patients")]
public class PatientsController : ControllerBase
{
   private readonly PatientManager _patientsmanager;

   public PatientsController(PatientManager patientmanager)
   {
      _patientsmanager = patientmanager;
   }

   [HttpGet]
   public List<Patients> Get()
   {
      return _patientsmanager.GetAll();
   }

   [HttpGet]
   [Route("{ci}")]
   public Patients GetByCI([FromRoute] int ci)
   {
      return _patientsmanager.GetByCI(ci);
   }

   [HttpPut]
   [Route("{ci}")]
   public Patients Put([FromRoute] int ci, [FromBody] Patients patientToUpdate)
   {
      return _patientsmanager.Update(ci, patientToUpdate.Name, patientToUpdate.LastName);
   }

   [HttpPost]
   public Patients Post([FromBody]Patients patientToCreate)
   {
      return _patientsmanager.Create(patientToCreate.Name, patientToCreate.LastName, patientToCreate.Age, patientToCreate.CI);
   }

   [HttpDelete]
   [Route("{ci}")]
   public Patients Delete([FromRoute] int ci)
   {
      return _patientsmanager.Delete(ci);
   }
}
