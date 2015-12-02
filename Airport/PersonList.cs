using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PersonList  {

	static List<Person> personList;
	static List<Employee> employeeList;

	public static void Setup()
	{
		personList = new List<Person>();
		employeeList = new List<Employee> ();
	}

	public static void AddPerson(Person person)
	{
		personList.Add (person);

		if (person is Employee) {
			employeeList.Add ((Employee) person);
		}
	}

	public static List<Employee> GetEmployeeList()
	{
		return employeeList;
	}

}
