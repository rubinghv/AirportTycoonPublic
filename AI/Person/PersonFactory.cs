using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersonFactory : MonoBehaviour {

	public List<Person> personList = new List<Person>();
	public Transform parentObject;

	public Person PlacePerson(Vector3 position, string personType)
	{
		foreach(Person person in personList) {
			if (person.GetType() == personType) {

				return person.CreatePerson(parentObject, position);

			}
		}

		print("could not find right passenger");
		return null;
	}





}
