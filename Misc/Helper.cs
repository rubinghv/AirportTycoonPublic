using UnityEngine;
using System.Collections;

public static class Helper {

	
	public static Vector2[] ResizeArray(Vector2[] vector_array, int size) {
		Vector2[] return_vector_array = new Vector2[size];
		
		for (int i = 0; i < size; i++)
			return_vector_array[i] = vector_array[i];
		
		return return_vector_array;
	}

	public static Vector2[] InvertArray(Vector2[] copy_from_array)
	{
		Vector2[] return_vector_array = new Vector2[copy_from_array.Length];
		int reverseIndex = copy_from_array.Length - 1;
		for (int i = 0; i < copy_from_array.Length; i++, reverseIndex--) {
			return_vector_array[reverseIndex] = copy_from_array[i];
		}

		return return_vector_array;
	}

	public static void PrintArray(Vector2[] vector_array) {
		for (int i = 0; i < vector_array.Length; i++)
			Debug.Log("[" + i + "] = " + vector_array[i]);
		
	}
	/*
	public static Vector2[] CopyArray(Vector2[] copy_from_array, Vector2[] copy_to_array) {		
		for (int i = 0; i < copy_from_array.Length; i++)
			copy_to_array[i] = copy_from_array[i];
		
		return copy_to_array;
	}*/

	public static Vector2[] CopyArrayVector2(Transform[] copy_from_array) {	
		return CopyArrayVector2(copy_from_array, copy_from_array.Length, 0);	
	}

	public static Vector2[] CopyArrayVector2(Transform[] copy_from_array, int size) {	
		return CopyArrayVector2(copy_from_array, size, 0);	
	}

	public static Vector2[] CopyArrayVector2(Transform[] copy_from_array, int size, int offset) {	
		Vector2[] return_vector_array = new Vector2[size];
	
		for (int i = 0; i < copy_from_array.Length; i++) {
			return_vector_array[i + offset] = new Vector2(copy_from_array[i].position.x, copy_from_array[i].position.z);
		}
			
		return return_vector_array;
		
	}


}
