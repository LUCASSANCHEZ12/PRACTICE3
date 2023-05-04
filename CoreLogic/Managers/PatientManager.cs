using UPB.CoreLogic.Models;
using System;

namespace UPB.CoreLogic.Managers;

public class PatientManager
{
    private string _path;
    
    public PatientManager(string path)
    {
        _path = path;
    }

    public List<Patients> GetAll()
    {
        List<Patients> patients = new List<Patients>();
        using (StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            Patients newPatient;
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                datos = line.Split(',');
                newPatient = new Patients() 
                {
                    Name = datos[0],
                    LastName = datos[1],
                    BloodGroup = datos[2],
                    Age = int.Parse(datos[3]),
                    CI = int.Parse(datos[4])
                };
                patients.Add(newPatient);
            }   
        }
        return patients;
    }

    public Patients GetByCI(int ci)
    {
        if (ci < 0)
        {
            throw new Exception("CI invalido");
        }

        Patients patientfound = Find(patient => int.Parse(patient[2]) == ci);
        if(patientfound == null)
        {
            throw new Exception("Error, Patient not found");
        }
        return patientfound;
    }

    public Patients Update(int ci, string name, string lastname)
    {
        if (ci < 0)
        {
            throw new Exception("CI invalido");
        }

        Patients patientfound = GetByCI(ci);

        if (patientfound == null)
        {
            throw new Exception("Error, Patient not found");
        }

        int indexLine = FindIndex(patient => int.Parse(patient[2]) == ci);

        List<string> lines = File.ReadAllLines(_path).ToList();

        if (lines.Count >= indexLine)
        {
            patientfound.Name = name;
            patientfound.LastName =  lastname;
            lines[indexLine] = patientfound.Name + ", " + patientfound.LastName + ", " + patientfound.CI.ToString() + ", " + patientfound.BloodGroup + "," + patientfound.Age.ToString();
        }

        using (StreamWriter writer = new StreamWriter(_path))
        {
            foreach (string linea in lines)
            {
                writer.WriteLine(linea);
            }
        }

        return patientfound;
    }
    public Patients Create(string name, string lastname, int age, int ci)
    {
        if(Find(patient => int.Parse(patient[2]) == ci) != null)
        {
            throw new Exception("Error, CI existente");
        }

        if(ci < 0)
        {
            throw new Exception("Error, CI no valido");
        }

        Random rnd = new Random();
        string[] BloodGroup = new string[] {"A+","A-","B+","B-","AB+","AB-","O-","O+"};
        Patients createdPatient = new Patients()
        {
            Name = name,
            LastName = lastname,
            Age = age,
            CI = ci,
            BloodGroup = BloodGroup[rnd.Next(0,7)]
        };
        Add(createdPatient);
        return createdPatient;
    }

    public Patients Delete(int ci)
    {
        Patients patientToDelete = GetByCI(ci);
        int indexLine = FindIndex(patient => int.Parse(patient[2]) == ci);

        if (patientToDelete == null)
        {
            throw new Exception("Error, Patient not found");
        }
        List<string> lines = File.ReadAllLines(_path).ToList();
        if (lines.Count >= indexLine)
        {
            lines.RemoveAt(indexLine);
        }
        using (StreamWriter writer = new StreamWriter(_path))
        {
            foreach (string linea in lines)
            {
                writer.WriteLine(linea);
            }
        }
        return patientToDelete;
    }

    public Patients Find(Predicate<string[]> predicate)
    {
        Patients patient = null;
        StreamReader reader = new StreamReader(@_path);
        string line;
        string[] datos;
        while (!reader.EndOfStream)
        {
            line = reader.ReadLine();
            if(line != null)
            {
                datos = line.Split(",");
                if (predicate(datos))
                {
                    patient = new Patients()
                    {
                        Name = datos[0],
                        LastName = datos[1],
                        CI = int.Parse(datos[2]),
                        BloodGroup = datos[3],
                        Age = int.Parse(datos[4])
                    };
                    
                    break;
                }
            }
        }
        reader.Close();
        return patient;
    }

    public void Add(Patients patient)
    {
        string paciente = patient.Name + ", " + patient.LastName + ", " + patient.CI.ToString() + ", " + patient.BloodGroup + "," + patient.Age.ToString();
        try
        {
            using (FileStream fs = new FileStream(@_path, FileMode.Append))
            {
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(paciente);
                sw.Close();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error: "+ e.Message);
        }
    }

    public int FindIndex(Predicate<string[]> predicate)
    {
        int indexLine = 0;
        using (StreamReader reader = new StreamReader(_path))
        {
            string line;
            string[] datos;
            while(!reader.EndOfStream)
            {
                line = reader.ReadLine();
                datos = line.Split(',');
                if(predicate(datos))
                {
                    break;
                }
                indexLine++;
            }
        }
        return indexLine;
    }    
}